using NuestrasCuentitas.Bitacora.Domain.Enums;

namespace NuestrasCuentitas.Bitacora.Domain.Entities;

public sealed class AccessAudit
{
    public Guid AuditId { get; private set; }
    public Guid TraceId { get; private set; }
    public Guid ActorId { get; private set; }
    public string PseudonymId { get; private set; } = string.Empty;
    public AuditActionType ActionType { get; private set; }
    public string ResourceType { get; private set; } = string.Empty;
    public Guid? ResourceId { get; private set; }
    public Guid? PatientId { get; private set; }
    public AuditOutcome Outcome { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private AccessAudit()
    {
    }

    private AccessAudit(
        Guid traceId,
        Guid actorId,
        string pseudonymId,
        AuditActionType actionType,
        string resourceType,
        Guid? resourceId,
        Guid? patientId,
        AuditOutcome outcome,
        DateTime createdAtUtc)
    {
        AuditId = Guid.NewGuid();
        TraceId = traceId;
        ActorId = actorId;
        PseudonymId = pseudonymId;
        ActionType = actionType;
        ResourceType = resourceType;
        ResourceId = resourceId;
        PatientId = patientId;
        Outcome = outcome;
        CreatedAtUtc = createdAtUtc;
    }

    public static AccessAudit Create(
        Guid traceId,
        Guid actorId,
        string pseudonymId,
        AuditActionType actionType,
        string resourceType,
        Guid? resourceId,
        Guid? patientId,
        AuditOutcome outcome,
        DateTime createdAtUtc)
    {
        if (traceId == Guid.Empty)
        {
            throw new ArgumentException("Trace id is required.", nameof(traceId));
        }

        if (actorId == Guid.Empty)
        {
            throw new ArgumentException("Actor id is required.", nameof(actorId));
        }

        if (string.IsNullOrWhiteSpace(pseudonymId))
        {
            throw new ArgumentException("Pseudonym id is required.", nameof(pseudonymId));
        }

        if (string.IsNullOrWhiteSpace(resourceType))
        {
            throw new ArgumentException("Resource type is required.", nameof(resourceType));
        }

        return new AccessAudit(
            traceId,
            actorId,
            pseudonymId.Trim(),
            actionType,
            resourceType.Trim(),
            resourceId,
            patientId,
            outcome,
            createdAtUtc);
    }
}
