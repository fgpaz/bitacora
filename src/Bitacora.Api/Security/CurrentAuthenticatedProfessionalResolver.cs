using NuestrasCuentitas.Bitacora.Api.Extensions;
using NuestrasCuentitas.Bitacora.Application.Common;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.Domain.Entities;
using NuestrasCuentitas.Bitacora.Domain.Enums;

namespace NuestrasCuentitas.Bitacora.Api.Security;

public sealed class CurrentAuthenticatedProfessionalResolver(IUserRepository userRepository)
{
    public async ValueTask<ResolvedProfessionalContext> ResolveAsync(HttpContext httpContext, CancellationToken cancellationToken)
    {
        if (httpContext.User.Identity?.IsAuthenticated != true)
        {
            throw new BitacoraException("UNAUTHORIZED", "Necesitás autenticarte para continuar.", StatusCodes.Status401Unauthorized);
        }

        var authSubject = httpContext.User.GetAuthSubject();
        var user = await userRepository.GetByAuthSubjectAsync(authSubject, cancellationToken)
            ?? throw new BitacoraException("PROFESSIONAL_NOT_FOUND", "No encontramos el profesional autenticado.", StatusCodes.Status404NotFound);

        if (user.Role != UserRole.Professional)
        {
            throw new BitacoraException("FORBIDDEN", "Este endpoint solo admite profesionales.", StatusCodes.Status403Forbidden);
        }

        return new ResolvedProfessionalContext(user);
    }
}

public sealed record ResolvedProfessionalContext(User User);
