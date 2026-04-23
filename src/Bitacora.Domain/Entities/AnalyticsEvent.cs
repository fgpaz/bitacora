namespace NuestrasCuentitas.Bitacora.Domain.Entities;

/// <summary>
/// Evento de analytics para medir UX impact del producto.
/// NO debe contener PII en <see cref="PropsJson"/>: solo labels, contadores, timestamps relativos.
/// El <see cref="PatientId"/> queda registrado como responsable del evento; el equipo puede
/// pseudonimizar en consulta si la retention policy lo exige.
/// </summary>
public sealed class AnalyticsEvent
{
    public Guid AnalyticsEventId { get; private set; }
    public Guid PatientId { get; private set; }
    public string EventName { get; private set; } = string.Empty;
    public string? PropsJson { get; private set; }
    public Guid TraceId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private AnalyticsEvent()
    {
    }

    private AnalyticsEvent(
        Guid patientId,
        string eventName,
        string? propsJson,
        Guid traceId,
        DateTime createdAtUtc)
    {
        AnalyticsEventId = Guid.NewGuid();
        PatientId = patientId;
        EventName = eventName;
        PropsJson = propsJson;
        TraceId = traceId;
        CreatedAtUtc = createdAtUtc;
    }

    public static AnalyticsEvent Create(
        Guid patientId,
        string eventName,
        string? propsJson,
        Guid traceId,
        DateTime createdAtUtc)
    {
        if (patientId == Guid.Empty)
        {
            throw new ArgumentException("Patient id is required.", nameof(patientId));
        }

        if (string.IsNullOrWhiteSpace(eventName))
        {
            throw new ArgumentException("Event name is required.", nameof(eventName));
        }

        if (eventName.Length > 64)
        {
            throw new ArgumentException("Event name must be at most 64 characters.", nameof(eventName));
        }

        if (propsJson is { Length: > 2048 })
        {
            throw new ArgumentException("Props payload must be at most 2048 characters.", nameof(propsJson));
        }

        if (traceId == Guid.Empty)
        {
            throw new ArgumentException("Trace id is required.", nameof(traceId));
        }

        return new AnalyticsEvent(
            patientId,
            eventName.Trim(),
            propsJson,
            traceId,
            createdAtUtc);
    }
}
