using NuestrasCuentitas.Bitacora.Api.Extensions;
using NuestrasCuentitas.Bitacora.Application.Common;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.Domain.Entities;
using NuestrasCuentitas.Bitacora.Domain.Enums;

namespace NuestrasCuentitas.Bitacora.Api.Security;

public sealed class CurrentAuthenticatedPatientResolver(IUserRepository userRepository)
{
    public async ValueTask<ResolvedPatientContext> ResolveAsync(HttpContext httpContext, CancellationToken cancellationToken)
    {
        if (httpContext.User.Identity?.IsAuthenticated != true)
        {
            throw new BitacoraException("UNAUTHORIZED", "Necesitás autenticarte para continuar.", StatusCodes.Status401Unauthorized);
        }

        var authSubject = httpContext.User.GetAuthSubject();
        var user = await userRepository.GetByAuthSubjectAsync(authSubject, cancellationToken)
            ?? throw new BitacoraException("PATIENT_NOT_FOUND", "No encontramos el paciente autenticado.", StatusCodes.Status404NotFound);

        if (user.Role != UserRole.Patient)
        {
            throw new BitacoraException("FORBIDDEN", "Este endpoint solo admite pacientes.", StatusCodes.Status403Forbidden);
        }

        return new ResolvedPatientContext(user);
    }
}

public sealed record ResolvedPatientContext(User User);
