# NewsPaper.MassTransit.Configuration
 
This ASP.NET Core 3.1module is responsible for configuring RabbitMQ and MassTransit.

## Description

The AppSettings.json of each autonomous microservice contains configuration options for the "mass-transit" section, and each standalone microservice contains the consumer and RequestClients configuration.

```C#
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
```

## Links to project repositories
- :white_check_mark:[NewsPaper Review](https://github.com/PKravchenko-ki16/NewsPaper)
- :white_check_mark:NewsPaper.MassTransit.Configuration
- :white_check_mark:[NewsPaper.MassTransit.Contracts](https://github.com/PKravchenko-ki16/NewsPaper.MassTransit.Contracts)
- :black_square_button:[NewsPaper.IdentityServer]()
- :white_check_mark:[Newspaper.GateWay](https://github.com/PKravchenko-ki16/Newspaper.GateWay)
- :white_check_mark:[NewsPaper.Accounts](https://github.com/PKravchenko-ki16/NewsPaper.Accounts)
- :white_check_mark:[NewsPaper.Articles](https://github.com/PKravchenko-ki16/NewsPaper.Articles)
- :white_check_mark:[NewsPaper.GatewayClientApi](https://github.com/PKravchenko-ki16/NewsPaper.GatewayClientApi)
- :white_check_mark:[NewsPaper.Client.Mvc](https://github.com/PKravchenko-ki16/NewsPaper.Client.Mvc)
