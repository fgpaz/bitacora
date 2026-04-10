using System.Security.Claims;
using NuestrasCuentitas.Bitacora.Application.Common;

namespace NuestrasCuentitas.Bitacora.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetSupabaseUserId(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue("sub")
            ?? principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new BitacoraException("UNAUTHORIZED", "Falta el claim sub del token autenticado.", StatusCodes.Status401Unauthorized);
    }

    public static string GetEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(ClaimTypes.Email)
            ?? principal.FindFirstValue("email")
            ?? throw new BitacoraException("ONB_001_JWT_INVALID", "El token no contiene el email esperado.", StatusCodes.Status401Unauthorized);
    }
}
