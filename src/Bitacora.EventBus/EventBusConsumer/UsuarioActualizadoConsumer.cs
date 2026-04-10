using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contract;

namespace NuestrasCuentitas.Bitacora.EventBus.EventBusConsumer;

public sealed class UsuarioActualizadoConsumer(ILogger<UsuarioActualizadoConsumer> logger)
    : IConsumer<UsuarioActualizadoEvent>
{
    public Task Consume(ConsumeContext<UsuarioActualizadoEvent> context)
    {
        logger.LogInformation(
            "UsuarioActualizadoEvent consumed. UsuarioId={UsuarioId} CorrelationId={CorrelationId}",
            context.Message.UsuarioId,
            context.Message.CorrelationId);

        return Task.CompletedTask;
    }
}
