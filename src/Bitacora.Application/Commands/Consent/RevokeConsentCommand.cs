using Mediator;
using NuestrasCuentitas.Bitacora.Application.Common;
using NuestrasCuentitas.Bitacora.Application.Interfaces;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Transactions;
using NuestrasCuentitas.Bitacora.Domain.Entities;
using NuestrasCuentitas.Bitacora.Domain.Enums;

namespace NuestrasCuentitas.Bitacora.Application.Commands.Consent;

public readonly record struct RevokeConsentCommand(
    Guid PatientId,
    Guid ActorId,
    Guid TraceId,
    bool Confirmed) : ICommand<RevokeConsentResponse>;

public sealed record RevokeConsentResponse(string Status, DateTime RevokedAtUtc);

public sealed class RevokeConsentCommandHandler(
    IConsentGrantRepository consentGrantRepository,
    IAccessAuditRepository accessAuditRepository,
    IBitacoraUnitOfWork unitOfWork,
    IPseudonymizationService pseudonymizationService)
    : ICommandHandler<RevokeConsentCommand, RevokeConsentResponse>
{
    public async ValueTask<RevokeConsentResponse> Handle(RevokeConsentCommand command, CancellationToken cancellationToken)
    {
        if (!command.Confirmed)
        {
            throw new BitacoraException("CONFIRMED_FALSE", "Necesitás confirmar la revocación para continuar.", 422);
        }

        var current = await consentGrantRepository.GetActiveByPatientAsync(command.PatientId, cancellationToken)
            ?? throw new BitacoraException("NO_ACTIVE_CONSENT", "No hay un consentimiento activo para revocar.", 404);

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var revokedAtUtc = DateTime.UtcNow;
            current.Revoke(revokedAtUtc);
            consentGrantRepository.Update(current);

            await accessAuditRepository.AddAsync(
                AccessAudit.Create(
                    command.TraceId,
                    command.ActorId,
                    pseudonymizationService.CreatePseudonym(command.ActorId),
                    AuditActionType.Revoke,
                    "consent_grant",
                    current.ConsentGrantId,
                    command.PatientId,
                    AuditOutcome.Ok,
                    revokedAtUtc),
                cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new RevokeConsentResponse("revoked", revokedAtUtc);
        }
        catch (BitacoraException)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new BitacoraException("REVOCATION_FAILED", "No pudimos revocar el consentimiento de forma segura.", 500);
        }
    }
}
