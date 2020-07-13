using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using MassTransit.MultiBus;
using Messaging.InterfacesConstants.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrdersApi.Messages.Consumers;
using OrdersApi.Persistence;
using OrdersApi.Persistence.Repository;
using OrdersApi.Services;

namespace OrdersApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<OrderDbContext>(options =>
            {
                options.UseLazyLoadingProxies();
                options.UseSqlServer(Configuration.GetConnectionString("OrderMicroServiceDb"));
            });
            services.AddControllers().AddNewtonsoftJson(options=>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
            services.AddHttpClient();
            services.AddMassTransit(cfg =>
            {
                cfg.AddConsumer<RegisterOrderCommandConsumer>();
            });
            services.AddSingleton(providers => Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host("localhost", "/", h => { });
                cfg.ReceiveEndpoint(RabbitMqMassTransitContstants.RegisterOrderCommandQueue, e =>
                 {
                     e.PrefetchCount = 16;
                     e.UseMessageRetry(x => x.Interval(2, TimeSpan.FromSeconds(10)));
                     e.Consumer<RegisterOrderCommandConsumer>(providers);
                 });
                cfg.ConfigureEndpoints(providers.GetService<IBusRegistrationContext>()); ;
            }));
            services.AddSingleton<IHostedService, BusService>();
            services.AddCors(corsOptions =>
            {
                corsOptions.AddPolicy("corspolicy", builder =>
                {
                    builder.AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed(host => true).AllowCredentials();
                });
            });
            services.AddTransient<IOrderRepository, OrderRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("corspolicy");
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
