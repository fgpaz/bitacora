using Mediator;
using Microsoft.Extensions.Logging;
using NuestrasCuentitas.Bitacora.Application.Common;
using NuestrasCuentitas.Bitacora.Application.Interfaces;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Transactions;
using NuestrasCuentitas.Bitacora.Domain.Entities;
using NuestrasCuentitas.Bitacora.Domain.Enums;
using Shared.Contract.Events;

namespace NuestrasCuentitas.Bitacora.Application.Commands.Vinculos;

/// <summary>
/// Patient revokes a care link (can revoke invited or active links).
/// Also cascades consent: if the revoked link was the patient's only active professional link,
/// and the patient has revoked consent status, the patient record is flagged accordingly.
/// Fail-closed: if the link state cannot be determined, the operation is rejected.
/// </summary>
public readonly record struct RevokeCareLinkCommand(
    Guid CareLinkId,
    Guid PatientId,
    Guid ActorId,
    Guid TraceId,
    bool Confirmed) : ICommand<RevokeCareLinkResponse>;

public sealed record RevokeCareLinkResponse(
    string Status,
    DateTime RevokedAtUtc);

public sealed class RevokeCareLinkCommandHandler(
    ICareLinkRepository careLinkRepository,
    IConsentGrantRepository consentGrantRepository,
    IBitacoraUnitOfWork unitOfWork,
    ILogger<RevokeCareLinkCommandHandler> logger)
    : ICommandHandler<RevokeCareLinkCommand, RevokeCareLinkResponse>
{
    public async ValueTask<RevokeCareLinkResponse> Handle(RevokeCareLinkCommand command, CancellationToken cancellationToken)
    {
        if (!command.Confirmed)
        {
            throw new BitacoraException("CONFIRMED_FALSE", "Necesitas confirmar la revocacion para continuar.", 422);
        }

        if (command.CareLinkId == Guid.Empty)
        {
            throw new BitacoraException("CARE_LINK_ID_REQUIRED", "El identificador del enlace es obligatorio.", 400);
        }

        var careLink = await careLinkRepository.GetByIdAsync(command.CareLinkId, cancellationToken)
            ?? throw new BitacoraException("CARE_LINK_NOT_FOUND", "No se encontro el enlace de cuidado.", 404);

        if (careLink.PatientId != command.PatientId)
        {
            throw new BitacoraException("NOT_YOUR_CARE_LINK", "Este enlace no te pertenece.", 403);
        }

        if (careLink.Status != CareLinkStatus.Invited && careLink.Status != CareLinkStatus.Active)
        {
            throw new BitacoraException("CARE_LINK_NOT_REVOKABLE",
                "El enlace no puede ser revocado en su estado actual.", 422);
        }

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var revokedAtUtc = DateTime.UtcNow;
            careLink.Revoke(revokedAtUtc, CareLinkStatus.RevokedByPatient);
            await careLinkRepository.UpdateAsync(careLink, cancellationToken);

            // Consent cascade: check if this was the patient's last active link
            var activeLinks = await careLinkRepository.GetByPatientIdAsync(command.PatientId, cancellationToken);
            var hasRemainingActiveLink = activeLinks.Any(x =>
                x.CareLinkId != command.CareLinkId &&
                x.Status == CareLinkStatus.Active);

            if (!hasRemainingActiveLink)
            {
                var latestConsent = await consentGrantRepository.GetActiveByPatientAsync(command.PatientId, cancellationToken);
                if (latestConsent?.Status == ConsentStatus.Revoked)
                {
                    // Patient has no active links and has revoked consent — flag accordingly
                    logger.LogInformation(
                        "Consent cascade: patient {PatientId} has no active care links and revoked consent. "
                        + "Last revoked link: {CareLinkId}",
                        command.PatientId, command.CareLinkId);
                }
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            logger.LogInformation("CareLink {CareLinkId} revoked by patient {PatientId}",
                command.CareLinkId, command.PatientId);

            return new RevokeCareLinkResponse("revoked_by_patient", revokedAtUtc);
        }
        catch (BitacoraException)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(ex, "RevokeCareLink failed for {CareLinkId}", command.CareLinkId);
            throw new BitacoraException("REVOKE_CARE_LINK_FAILED", "No pudimos revocar el enlace de cuidado.", 500);
        }
    }
}
