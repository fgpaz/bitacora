using Mediator;
using NuestrasCuentitas.Bitacora.Application.Common;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using Shared.Contract.Export;

namespace NuestrasCuentitas.Bitacora.Application.Queries.Export;

public readonly record struct GetExportConstraintsQuery(
    Guid PatientId,
    Guid ActorId,
    bool IsProfessional,
    Guid TraceId) : IQuery<ExportConstraintDto>;

public sealed class GetExportConstraintsQueryHandler(
    ICareLinkRepository careLinkRepository)
    : IQueryHandler<GetExportConstraintsQuery, ExportConstraintDto>
{
    public async ValueTask<ExportConstraintDto> Handle(
        GetExportConstraintsQuery query,
        CancellationToken cancellationToken)
    {
        if (!query.IsProfessional)
        {
            return new ExportConstraintDto(
                ExportType.Full,
                Allowed: true,
                Reason: null);
        }

        var careLink = await careLinkRepository.FindActiveByPatientAndProfessionalAsync(
            query.PatientId,
            query.ActorId,
            cancellationToken);

        if (careLink is null || !careLink.CanViewData)
        {
            return new ExportConstraintDto(
                ExportType.Full,
                Allowed: false,
                Reason: "No tenes acceso a los datos de este paciente.");
        }

        return new ExportConstraintDto(
            ExportType.Full,
            Allowed: false,
            Reason: "La exportacion de datos es solo para el paciente propietario. Solo el paciente puede descargar sus propios registros.");
    }
}
