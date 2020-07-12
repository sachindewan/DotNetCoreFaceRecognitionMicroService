using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Faces.WebMvc.RestClients;
using Faces.WebMvc.Services;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Faces.WebMvc
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
            services.AddMassTransit();
            services.AddSingleton(provider => Bus.Factory.CreateUsingRabbitMq(cnf =>
            {
               cnf.Host("localhost", "/", h => { });
               services.AddSingleton(provider => provider.GetRequiredService<IBusControl>());
               services.AddSingleton<IHostedService, BusService>();
            }));
            services.AddHttpClient<IOrderManagementApi, OrderApiManagement>();
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
