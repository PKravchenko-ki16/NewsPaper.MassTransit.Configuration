using System;
using Microsoft.Extensions.DependencyInjection;

namespace NewsPaper.MassTransit.Configuration
{
    public static class MassTransitServiceConfigurationExtension
    {
        public static void Configure(
            this IServiceCollection services,
            Action<MassTransitConfiguration> configuration,
            string serviceName)
        {
            var transitConfiguration = new MassTransitConfiguration();
            if (configuration == null)
            {
                throw new Exception(nameof(configuration));
            }

            configuration(transitConfiguration);

            if (string.IsNullOrWhiteSpace(transitConfiguration.ServiceName))
            {
                throw new Exception(transitConfiguration.ServiceName);
            }

            transitConfiguration.ServiceName = serviceName;
            services.AddSingleton(transitConfiguration);
        }
    }
}