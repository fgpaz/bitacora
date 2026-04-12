using Mediator;
using NuestrasCuentitas.Bitacora.Application.Common;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;

namespace NuestrasCuentitas.Bitacora.Application.Queries.Vinculos;

/// <summary>
/// Returns all active care links where the patient has granted view-data permission.
/// Used by the API layer to build the professional access list shown to the patient.
/// </summary>
public readonly record struct GetActiveCareLinksWithViewPermissionQuery(
    Guid PatientId,
    Guid ActorId,
    Guid TraceId) : IQuery<GetActiveCareLinksResponse>;

public sealed record GetActiveCareLinksResponse(
    IReadOnlyList<ActiveCareLinkSummary> Links);

public sealed record ActiveCareLinkSummary(
    Guid CareLinkId,
    Guid ProfessionalId,
    bool CanViewData,
    DateTime AcceptedAtUtc);

public sealed class GetActiveCareLinksWithViewPermissionQueryHandler(
    ICareLinkRepository careLinkRepository)
    : IQueryHandler<GetActiveCareLinksWithViewPermissionQuery, GetActiveCareLinksResponse>
{
    public async ValueTask<GetActiveCareLinksResponse> Handle(
        GetActiveCareLinksWithViewPermissionQuery query,
        CancellationToken cancellationToken)
    {
        var allLinks = await careLinkRepository.GetByPatientIdAsync(query.PatientId, cancellationToken);

        var activeWithView = allLinks
            .Where(x => x.Status == Domain.Enums.CareLinkStatus.Active && x.CanViewData)
            .Select(x => new ActiveCareLinkSummary(x.CareLinkId, x.ProfessionalId, x.CanViewData, x.AcceptedAt!.Value))
            .ToList();

        return new GetActiveCareLinksResponse(activeWithView);
    }
}
