using Mediator;
using Microsoft.Extensions.Configuration;
using NuestrasCuentitas.Bitacora.Application.Common;
using NuestrasCuentitas.Bitacora.Application.Interfaces;
using NuestrasCuentitas.Bitacora.Application.Queries.Consent;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Transactions;
using NuestrasCuentitas.Bitacora.Domain.Entities;
using NuestrasCuentitas.Bitacora.Domain.Enums;

namespace NuestrasCuentitas.Bitacora.Application.Commands.Consent;

public readonly record struct GrantConsentCommand(
    Guid PatientId,
    Guid ActorId,
    Guid TraceId,
    string Version,
    bool Accepted) : ICommand<GrantConsentResponse>;

public sealed record GrantConsentResponse(
    Guid ConsentGrantId,
    string Status,
    DateTime GrantedAtUtc,
    bool NeedsFirstEntry,
    bool ResumePendingInvite);

public sealed class GrantConsentCommandHandler(
    IConfiguration configuration,
    IUserRepository userRepository,
    IConsentGrantRepository consentGrantRepository,
    IAccessAuditRepository accessAuditRepository,
    IBitacoraUnitOfWork unitOfWork,
    IPseudonymizationService pseudonymizationService)
    : ICommandHandler<GrantConsentCommand, GrantConsentResponse>
{
    public async ValueTask<GrantConsentResponse> Handle(GrantConsentCommand command, CancellationToken cancellationToken)
    {
        if (!command.Accepted)
        {
            throw new BitacoraException("ACCEPTED_FALSE", "Necesitás confirmar el consentimiento para continuar.", 422);
        }

        var consentConfiguration = GetCurrentConsentQueryHandler.ReadConsentConfiguration(configuration);
        if (!string.Equals(command.Version, consentConfiguration.ActiveVersion, StringComparison.Ordinal))
        {
            throw new BitacoraException("CONSENT_VERSION_MISMATCH", "La versión del consentimiento ya cambió.", 409);
        }

        var patient = await userRepository.GetByIdAsync(command.PatientId, cancellationToken)
            ?? throw new BitacoraException("PATIENT_NOT_FOUND", "No encontramos el paciente autenticado.", 404);

        var current = await consentGrantRepository.GetActiveByPatientAsync(command.PatientId, cancellationToken);
        if (current is not null && current.ConsentVersion == consentConfiguration.ActiveVersion)
        {
            throw new BitacoraException("CONSENT_ALREADY_GRANTED", "El consentimiento activo ya fue otorgado.", 409);
        }

        var grantedAtUtc = DateTime.UtcNow;
        var consentGrant = ConsentGrant.CreateGranted(command.PatientId, consentConfiguration.ActiveVersion, grantedAtUtc);
        patient.MarkConsentGranted();

        await consentGrantRepository.AddAsync(consentGrant, cancellationToken);
        userRepository.Update(patient);
        await accessAuditRepository.AddAsync(
            AccessAudit.Create(
                command.TraceId,
                command.ActorId,
                pseudonymizationService.CreatePseudonym(command.ActorId),
                AuditActionType.Grant,
                "consent_grant",
                consentGrant.ConsentGrantId,
                command.PatientId,
                AuditOutcome.Ok,
                grantedAtUtc),
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new GrantConsentResponse(
            consentGrant.ConsentGrantId,
            "consent_granted",
            grantedAtUtc,
            true,
            false);
    }
}
