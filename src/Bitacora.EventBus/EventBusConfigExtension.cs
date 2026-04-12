using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NuestrasCuentitas.Bitacora.Application.Interfaces;
using NuestrasCuentitas.Bitacora.EventBus.EventBusConsumer;
using NuestrasCuentitas.Bitacora.EventBus.EventBusServices;
using NuestrasCuentitas.Bitacora.EventBus.Options;

namespace NuestrasCuentitas.Bitacora.EventBus;

public static class EventBusConfigExtension
{
    public static IServiceCollection AddSetupEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EventBusSettings>(configuration.GetSection("EventBusSettings"));
        services.AddSingleton<IIntegrationEventPublisher, NoOpIntegrationEventPublisher>();

        var hostAddress = configuration.GetValue<string>("EventBusSettings:HostAddress");
        if (string.IsNullOrWhiteSpace(hostAddress))
        {
            return services;
        }

        services.AddScoped<IIntegrationEventPublisher, EventBusServicePublisher>();

        services.Configure<MassTransitHostOptions>(options =>
        {
            options.WaitUntilStarted = false;
            options.StartTimeout = TimeSpan.FromSeconds(15);
            options.StopTimeout = TimeSpan.FromMinutes(1);
        });

        services.AddMassTransit(registration =>
        {
            ConfigureConsumerManager.AddConsumers(registration);

            registration.UsingRabbitMq((context, cfg) =>
            {
                var settings = context.GetRequiredService<IOptions<EventBusSettings>>().Value;

                cfg.Host(new Uri(settings.HostAddress), host =>
                {
                    host.Username(settings.User);
                    host.Password(settings.Password);
                });

                foreach (var (queueName, queueConfigurator) in ConfigureConsumerManager.GetList())
                {
                    cfg.ReceiveEndpoint(queueName, endpoint => queueConfigurator(endpoint, context));
                }
            });
        });

        return services;
    }
}
