using Mediator;
using NuestrasCuentitas.Bitacora.Application.Common;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.Domain.Enums;

namespace NuestrasCuentitas.Bitacora.Application.Queries.Vinculos;

/// <summary>
/// Returns all patients with an active CareLink for a given professional
/// where can_view_data=true (fail-closed: returns empty list if none exist).
/// Gated by ProfessionalDataAccessAuthorizer for per-patient operations.
/// </summary>
public readonly record struct GetProfessionalPatientsQuery(
    Guid ProfessionalId,
    Guid ActorId,
    Guid TraceId) : IQuery<GetProfessionalPatientsResponse>;

public sealed record GetProfessionalPatientsResponse(
    IReadOnlyList<ProfessionalPatientSummary> Patients);

public sealed record ProfessionalPatientSummary(
    Guid CareLinkId,
    Guid PatientId,
    string Status,
    bool CanViewData,
    DateTime AcceptedAtUtc);

public sealed class GetProfessionalPatientsQueryHandler(
    ICareLinkRepository careLinkRepository)
    : IQueryHandler<GetProfessionalPatientsQuery, GetProfessionalPatientsResponse>
{
    public async ValueTask<GetProfessionalPatientsResponse> Handle(
        GetProfessionalPatientsQuery query,
        CancellationToken cancellationToken)
    {
        var allLinks = await careLinkRepository.GetByProfessionalIdAsync(query.ProfessionalId, cancellationToken);

        // Only return active links with can_view_data=true
        var patients = allLinks
            .Where(x => x.Status == CareLinkStatus.Active && x.CanViewData)
            .OrderByDescending(x => x.AcceptedAt)
            .Select(x => new ProfessionalPatientSummary(
                x.CareLinkId,
                x.PatientId,
                "active",
                x.CanViewData,
                x.AcceptedAt!.Value))
            .ToList();

        return new GetProfessionalPatientsResponse(patients);
    }
}