namespace NuestrasCuentitas.Bitacora.Application.Interfaces;

public interface IIntegrationEventPublisher
{
    ValueTask PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class;
}
