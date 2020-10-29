using System;
using MassTransit;
using MassTransit.Definition;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NewsPaper.MassTransit.Configuration
{
    public static class ConfigureServicesMassTransit
    {
        public static void ConfigureServices(
            IServiceCollection services,
            IConfiguration configuration,
            MassTransitConfiguration massTransitConfiguration)
        {
            if (massTransitConfiguration == null || massTransitConfiguration.IsDebug)
            {
                return;
            }

            var massTransitSection = configuration.GetSection("MassTransit");
            var url = massTransitSection.GetValue<string>("Url");
            var host = massTransitSection.GetValue<string>("Host");
            if (massTransitSection == null || url == null || host == null)
            {
                throw new Exception("Section 'mass-transit' configuration settings are not found in appSettings.json");
            }

            services.AddMassTransit(x =>
            {
                x.AddBus(busFactory =>
                {
                    var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
                    {
                        cfg.Host($"rabbitmq://{url}/{host}", configurator =>
                        {
                            configurator.Username("guest");
                            configurator.Password("guest");
                        });

                        cfg.ConfigureEndpoints(busFactory, KebabCaseEndpointNameFormatter.Instance);

                        cfg.UseJsonSerializer();
                    });
                    massTransitConfiguration.BusControl?.Invoke(bus, services.BuildServiceProvider());
                    return bus;
                });
                massTransitConfiguration.Configurator?.Invoke(x);
                services.AddMassTransitHostedService();
            });
        }
    
    }
}