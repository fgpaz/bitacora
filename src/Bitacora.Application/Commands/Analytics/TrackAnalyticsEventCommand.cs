using System.Text.Json;
using Mediator;
using NuestrasCuentitas.Bitacora.Application.Common;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Transactions;
using NuestrasCuentitas.Bitacora.Domain.Entities;

namespace NuestrasCuentitas.Bitacora.Application.Commands.Analytics;

public readonly record struct TrackAnalyticsEventCommand(
    Guid PatientId,
    Guid TraceId,
    string EventName,
    JsonElement? Props) : ICommand<TrackAnalyticsEventResponse>;

public sealed record TrackAnalyticsEventResponse(Guid AnalyticsEventId, DateTime CreatedAtUtc);

public sealed class TrackAnalyticsEventCommandHandler(
    IAnalyticsEventRepository analyticsEventRepository,
    IBitacoraUnitOfWork unitOfWork)
    : ICommandHandler<TrackAnalyticsEventCommand, TrackAnalyticsEventResponse>
{
    private static readonly HashSet<string> AllowedEvents =
    [
        "time_to_cta_ready",
        "ctr_rail_vs_checkin",
        "logout_accidental_rate",
        "decline_consent_rate",
    ];

    public async ValueTask<TrackAnalyticsEventResponse> Handle(
        TrackAnalyticsEventCommand command,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.EventName))
        {
            throw new BitacoraException("EVENT_NAME_REQUIRED", "El nombre del evento es obligatorio.", 400);
        }

        if (!AllowedEvents.Contains(command.EventName))
        {
            throw new BitacoraException("EVENT_NAME_UNKNOWN", "El nombre del evento no está permitido.", 400);
        }

        string? propsJson = null;
        if (command.Props.HasValue && command.Props.Value.ValueKind != JsonValueKind.Undefined
            && command.Props.Value.ValueKind != JsonValueKind.Null)
        {
            propsJson = command.Props.Value.GetRawText();
            if (propsJson.Length > 2048)
            {
                throw new BitacoraException("PROPS_TOO_LARGE", "Los props del evento superan el máximo permitido (2048 chars).", 400);
            }
        }

        var analyticsEvent = AnalyticsEvent.Create(
            command.PatientId,
            command.EventName,
            propsJson,
            command.TraceId,
            DateTime.UtcNow);

        await analyticsEventRepository.AddAsync(analyticsEvent, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new TrackAnalyticsEventResponse(analyticsEvent.AnalyticsEventId, analyticsEvent.CreatedAtUtc);
    }
}
