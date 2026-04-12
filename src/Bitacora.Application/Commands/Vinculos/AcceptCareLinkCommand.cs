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
/// Patient accepts a BindingCode, creating or activating a CareLink.
/// If the code is unused, the link is created Active immediately (auto-binding per RF-VIN-012).
/// If the code is already used or expired, the operation fails (fail-closed).
/// </summary>
public readonly record struct AcceptCareLinkCommand(
    string BindingCode,
    Guid PatientId,
    Guid ActorId,
    Guid TraceId) : ICommand<AcceptCareLinkResponse>;

public sealed record AcceptCareLinkResponse(
    Guid CareLinkId,
    string Status,
    bool CanViewData,
    DateTime AcceptedAtUtc,
    bool IsNewLink);

public sealed class AcceptCareLinkCommandHandler(
    IBindingCodeRepository bindingCodeRepository,
    ICareLinkRepository careLinkRepository,
    IBitacoraUnitOfWork unitOfWork,
    ILogger<AcceptCareLinkCommandHandler> logger)
    : ICommandHandler<AcceptCareLinkCommand, AcceptCareLinkResponse>
{
    public async ValueTask<AcceptCareLinkResponse> Handle(AcceptCareLinkCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.BindingCode))
        {
            throw new BitacoraException("BINDING_CODE_REQUIRED", "El codigo de vinculacion es obligatorio.", 400);
        }

        if (command.PatientId == Guid.Empty)
        {
            throw new BitacoraException("PATIENT_ID_REQUIRED", "El identificador del paciente es obligatorio.", 400);
        }

        var code = command.BindingCode.Trim().ToUpperInvariant();
        var bindingCode = await bindingCodeRepository.FindByCodeAsync(code, cancellationToken);

        if (bindingCode is null || bindingCode.Used || bindingCode.ExpiresAt <= DateTime.UtcNow)
        {
            throw new BitacoraException("BINDING_CODE_INVALID_OR_EXPIRED",
                "El codigo es invalido o ya fue utilizado.", 410);
        }

        var existingLink = await careLinkRepository.FindActiveByPatientAndProfessionalAsync(
            command.PatientId, bindingCode.ProfessionalId, cancellationToken);

        if (existingLink is not null)
        {
            throw new BitacoraException("CARE_LINK_ALREADY_EXISTS",
                "Ya tienes un enlace activo con este profesional.", 409);
        }

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var nowUtc = DateTime.UtcNow;

            bindingCode.MarkAsUsed();
            await bindingCodeRepository.UpdateAsync(bindingCode, cancellationToken);

            var careLink = CareLink.CreateActive(bindingCode.ProfessionalId, command.PatientId, nowUtc, nowUtc);
            await careLinkRepository.AddAsync(careLink, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            logger.LogInformation("CareLink {CareLinkId} activated via binding code for patient {PatientId}, professional {ProfessionalId}",
                careLink.CareLinkId, command.PatientId, bindingCode.ProfessionalId);

            return new AcceptCareLinkResponse(careLink.CareLinkId, "active", careLink.CanViewData, nowUtc, IsNewLink: true);
        }
        catch (BitacoraException)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(ex, "AcceptCareLink failed for binding code {Code}", code);
            throw new BitacoraException("ACCEPT_CARE_LINK_FAILED", "No pudimos procesar el codigo de vinculacion.", 500);
        }
    }
}
