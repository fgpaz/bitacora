using Microsoft.Extensions.Logging;
using NuestrasCuentitas.Bitacora.Application.Interfaces;

namespace NuestrasCuentitas.Bitacora.EventBus.EventBusServices;

public sealed class NoOpIntegrationEventPublisher(ILogger<NoOpIntegrationEventPublisher> logger) : IIntegrationEventPublisher
{
    public ValueTask PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        logger.LogDebug("EventBus disabled. Event {EventType} was ignored.", typeof(TEvent).Name);
        return ValueTask.CompletedTask;
    }
}
