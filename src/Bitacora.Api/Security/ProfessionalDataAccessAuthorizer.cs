using Microsoft.AspNetCore.Http;
using NuestrasCuentitas.Bitacora.Application.Common;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;

namespace NuestrasCuentitas.Bitacora.Api.Security;

/// <summary>
/// Authorizes professional data access via CareLink + can_view_data (RF-VIN-023).
/// Fail-closed: if no active CareLink with can_view_data=true exists, throws BitacoraException(403).
/// </summary>
public sealed class ProfessionalDataAccessAuthorizer(ICareLinkRepository careLinkRepository)
{
    /// <summary>
    /// Verifies the professional has an active CareLink with can_view_data=true for the given patient.
    /// Throws BitacoraException(403) if authorization fails.
    /// </summary>
    public async ValueTask AuthorizeAsync(
        Guid professionalId,
        Guid patientId,
        CancellationToken cancellationToken)
    {
        var careLink = await careLinkRepository.FindActiveByPatientAndProfessionalAsync(
            patientId, professionalId, cancellationToken);

        if (careLink == null || !careLink.CanViewData)
        {
            throw new BitacoraException(
                "PROFESSIONAL_ACCESS_DENIED",
                "No tenes permiso para ver los datos de este paciente.",
                StatusCodes.Status403Forbidden);
        }
    }
}
