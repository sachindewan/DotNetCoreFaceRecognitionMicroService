using EmailService;
using GreenPipes;
using MassTransit;
using Messaging.InterfacesConstants.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using notificationService.Consumer;
using System;
using System.IO;
using System.Threading.Tasks;

namespace notificationService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
             host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
             .ConfigureHostConfiguration(configHost =>
             {
                 configHost.SetBasePath(Directory.GetCurrentDirectory());
                 configHost.AddJsonFile($"appsettings.json", optional: false);
                 configHost.AddEnvironmentVariables();
                 configHost.AddCommandLine(args);
             })
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json",
                        optional: true);
                })
                 .ConfigureServices((hostContext, services) =>
                 {
                     var emailConfig = hostContext.Configuration
                  .GetSection("EmailConfiguration")
                  .Get<EmailConfig>();
                     services.AddSingleton(emailConfig);
                     services.AddScoped<IEmailSender, EmailSender>();
                     services.AddMassTransit(cfg =>
                     {
                         cfg.AddConsumer<OrderProcessedEventConsumer>();
                     });
                     services.AddSingleton(providers => Bus.Factory.CreateUsingRabbitMq(cfg =>
                     {
                         cfg.Host("localhost", "/", h => { });
                         cfg.ReceiveEndpoint(RabbitMqMassTransitContstants.NotificationServiceQueue, e =>
                         {
                             e.PrefetchCount = 16;
                             e.UseMessageRetry(x => x.Interval(2, TimeSpan.FromSeconds(10)));
                             e.Consumer<OrderProcessedEventConsumer>(providers);
                         });
                         cfg.ConfigureEndpoints(providers.GetService<IBusRegistrationContext>()); ;
                     }));
                     services.AddSingleton<IHostedService, BusService>();
                 });
    }
}
