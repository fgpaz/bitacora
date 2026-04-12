using Mediator;
using Microsoft.Extensions.Logging;
using NuestrasCuentitas.Bitacora.Application.Common;
using NuestrasCuentitas.Bitacora.Application.Interfaces;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Transactions;
using NuestrasCuentitas.Bitacora.Domain.Enums;
using Shared.Contract.Events;

namespace NuestrasCuentitas.Bitacora.Application.Commands.Vinculos;

/// <summary>
/// Patient updates can_view_data on an active care link.
/// Only the patient owner may update this flag (RF-VIN-023).
/// Fail-closed: if ownership or link status is unclear, the operation is rejected.
/// </summary>
public readonly record struct UpdateCareLinkCanViewDataCommand(
    Guid CareLinkId,
    Guid PatientId,
    Guid ActorId,
    Guid TraceId,
    bool CanViewData) : ICommand<UpdateCareLinkCanViewDataResponse>;

public sealed record UpdateCareLinkCanViewDataResponse(
    Guid CareLinkId,
    bool CanViewData,
    DateTime UpdatedAtUtc);

public sealed class UpdateCareLinkCanViewDataCommandHandler(
    ICareLinkRepository careLinkRepository,
    IBitacoraUnitOfWork unitOfWork,
    ILogger<UpdateCareLinkCanViewDataCommandHandler> logger)
    : ICommandHandler<UpdateCareLinkCanViewDataCommand, UpdateCareLinkCanViewDataResponse>
{
    public async ValueTask<UpdateCareLinkCanViewDataResponse> Handle(
        UpdateCareLinkCanViewDataCommand command,
        CancellationToken cancellationToken)
    {
        if (command.CareLinkId == Guid.Empty)
        {
            throw new BitacoraException("CARE_LINK_ID_REQUIRED", "El identificador del enlace es obligatorio.", 400);
        }

        if (command.PatientId == Guid.Empty)
        {
            throw new BitacoraException("PATIENT_ID_REQUIRED", "El identificador del paciente es obligatorio.", 400);
        }

        var careLink = await careLinkRepository.GetByIdAsync(command.CareLinkId, cancellationToken)
            ?? throw new BitacoraException("CARE_LINK_NOT_FOUND", "No se encontro el enlace de cuidado.", 404);

        if (careLink.PatientId != command.PatientId)
        {
            throw new BitacoraException("NOT_YOUR_CARE_LINK", "Este enlace no te pertenece.", 403);
        }

        if (careLink.Status != CareLinkStatus.Active)
        {
            throw new BitacoraException("CARE_LINK_NOT_ACTIVE",
                "Solo puedes actualizar el acceso sobre un enlace activo.", 422);
        }

        try
        {
            var updatedAtUtc = DateTime.UtcNow;
            careLink.UpdateCanViewData(command.CanViewData, updatedAtUtc);
            await careLinkRepository.UpdateAsync(careLink, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "CareLink {CareLinkId} can_view_data updated to {CanViewData} by patient {PatientId}",
                command.CareLinkId, command.CanViewData, command.PatientId);

            return new UpdateCareLinkCanViewDataResponse(command.CareLinkId, command.CanViewData, updatedAtUtc);
        }
        catch (InvalidOperationException ex)
        {
            throw new BitacoraException("UPDATE_CAN_VIEW_DATA_FAILED", ex.Message, 422);
        }
    }
}
