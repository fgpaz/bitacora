using Mediator;
using Microsoft.Extensions.Configuration;
using NuestrasCuentitas.Bitacora.Application.Common;
using NuestrasCuentitas.Bitacora.Application.Interfaces;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Transactions;
using NuestrasCuentitas.Bitacora.Domain.Entities;
using NuestrasCuentitas.Bitacora.Domain.Enums;

namespace NuestrasCuentitas.Bitacora.Application.Queries.Consent;

public readonly record struct GetCurrentConsentQuery(Guid PatientId, Guid ActorId, Guid TraceId) : IQuery<GetCurrentConsentResponse>;

public sealed record GetCurrentConsentResponse(
    string Version,
    string Text,
    IReadOnlyList<ConsentSection> Sections,
    string PatientStatus);

public sealed class GetCurrentConsentQueryHandler(
    IConfiguration configuration,
    IUserRepository userRepository,
    IConsentGrantRepository consentGrantRepository,
    IAccessAuditRepository accessAuditRepository,
    IBitacoraUnitOfWork unitOfWork,
    IPseudonymizationService pseudonymizationService)
    : IQueryHandler<GetCurrentConsentQuery, GetCurrentConsentResponse>
{
    public async ValueTask<GetCurrentConsentResponse> Handle(GetCurrentConsentQuery query, CancellationToken cancellationToken)
    {
        var consentConfiguration = ReadConsentConfiguration(configuration);
        var patient = await userRepository.GetByIdAsync(query.PatientId, cancellationToken)
            ?? throw new BitacoraException("PATIENT_NOT_FOUND", "No encontramos el paciente autenticado.", 404);

        var latest = await consentGrantRepository.GetLatestByPatientAndVersionAsync(query.PatientId, consentConfiguration.ActiveVersion, cancellationToken);
        var patientStatus = latest?.Status switch
        {
            ConsentStatus.Granted => "granted",
            ConsentStatus.Revoked => "revoked",
            ConsentStatus.Pending => "pending",
            _ => "none"
        };

        await accessAuditRepository.AddAsync(
            AccessAudit.Create(
                query.TraceId,
                query.ActorId,
                pseudonymizationService.CreatePseudonym(query.ActorId),
                AuditActionType.Read,
                "consent_grant",
                latest?.ConsentGrantId,
                patient.UserId,
                AuditOutcome.Ok,
                DateTime.UtcNow),
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new GetCurrentConsentResponse(
            consentConfiguration.ActiveVersion,
            consentConfiguration.ActiveText,
            consentConfiguration.Sections,
            patientStatus);
    }

    internal static ConsentConfiguration ReadConsentConfiguration(IConfiguration configuration)
    {
        var activeVersion = configuration["Consent:ActiveVersion"];
        var activeText = configuration["Consent:ActiveText"];
        var sections = configuration.GetSection("Consent:Sections").Get<List<ConsentSection>>();

        if (string.IsNullOrWhiteSpace(activeVersion) || string.IsNullOrWhiteSpace(activeText) || sections is null || sections.Count == 0)
        {
            throw new BitacoraException("NO_CONSENT_CONFIG", "No hay un consentimiento activo configurado.", 503);
        }

        return new ConsentConfiguration
        {
            ActiveVersion = activeVersion.Trim(),
            ActiveText = activeText.Trim(),
            Sections = sections
        };
    }
}
