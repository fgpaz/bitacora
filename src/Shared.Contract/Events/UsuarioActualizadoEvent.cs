using MassTransit;

namespace Shared.Contract;

[EntityName("UsuarioActualizado")]
public sealed record UsuarioActualizadoEvent(
    Guid UsuarioId,
    string Nombre,
    string Email,
    Guid CorrelationId,
    DateTimeOffset OcurridoEnUtc
);
