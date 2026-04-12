using Mediator;
using NuestrasCuentitas.Bitacora.Application.Common;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;

namespace NuestrasCuentitas.Bitacora.Application.Queries.Vinculos;

/// <summary>
/// Returns all care links for a patient, regardless of status.
/// </summary>
public readonly record struct GetCareLinksByPatientQuery(
    Guid PatientId,
    Guid ActorId,
    Guid TraceId) : IQuery<GetCareLinksResponse>;

public sealed record GetCareLinksResponse(
    IReadOnlyList<CareLinkSummary> Links);

public sealed record CareLinkSummary(
    Guid CareLinkId,
    Guid ProfessionalId,
    string Status,
    bool CanViewData,
    DateTime InvitedAt,
    DateTime? AcceptedAt,
    DateTime? RevokedAt);

public sealed class GetCareLinksByPatientQueryHandler(
    ICareLinkRepository careLinkRepository)
    : IQueryHandler<GetCareLinksByPatientQuery, GetCareLinksResponse>
{
    public async ValueTask<GetCareLinksResponse> Handle(GetCareLinksByPatientQuery query, CancellationToken cancellationToken)
    {
        var links = await careLinkRepository.GetByPatientIdAsync(query.PatientId, cancellationToken);

        var summaries = links.Select(x => new CareLinkSummary(
            x.CareLinkId,
            x.ProfessionalId,
            x.Status.ToString().ToLowerInvariant(),
            x.CanViewData,
            x.InvitedAt,
            x.AcceptedAt,
            x.RevokedAt)).ToList();

        return new GetCareLinksResponse(summaries);
    }
}
