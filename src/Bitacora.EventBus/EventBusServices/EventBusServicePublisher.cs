using MassTransit;
using Microsoft.Extensions.Logging;
using NuestrasCuentitas.Bitacora.Application.Interfaces;

namespace NuestrasCuentitas.Bitacora.EventBus.EventBusServices;

public sealed class EventBusServicePublisher(
    IPublishEndpoint publishEndpoint,
    ILogger<EventBusServicePublisher> logger) : IIntegrationEventPublisher
{
    public async ValueTask PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        try
        {
            await publishEndpoint.Publish(@event, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Event publish failed for {EventType}.", typeof(TEvent).Name);
        }
    }
}
