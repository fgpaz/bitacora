using System.Security.Claims;
using System.Text.Json;
using NuestrasCuentitas.Bitacora.Application.Common;

namespace NuestrasCuentitas.Bitacora.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public const string ZitadelProjectRolesClaim = "urn:zitadel:iam:org:project:roles";

    public static string GetAuthSubject(this ClaimsPrincipal principal)
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

    public static IReadOnlySet<string> GetZitadelProjectRoles(this ClaimsPrincipal principal)
    {
        var claim = principal.FindFirstValue(ZitadelProjectRolesClaim);
        if (string.IsNullOrWhiteSpace(claim))
        {
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        try
        {
            using var document = JsonDocument.Parse(claim);
            return document.RootElement.ValueKind switch
            {
                JsonValueKind.Object => document.RootElement.EnumerateObject()
                    .Select(property => property.Name)
                    .Where(role => !string.IsNullOrWhiteSpace(role))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase),
                JsonValueKind.Array => document.RootElement.EnumerateArray()
                    .Where(item => item.ValueKind == JsonValueKind.String)
                    .Select(item => item.GetString())
                    .Where(role => !string.IsNullOrWhiteSpace(role))
                    .Select(role => role!)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase),
                JsonValueKind.String => new HashSet<string>(
                    [document.RootElement.GetString() ?? string.Empty],
                    StringComparer.OrdinalIgnoreCase),
                _ => new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            };
        }
        catch (JsonException)
        {
            return claim.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }
    }

    public static bool HasZitadelProjectRole(this ClaimsPrincipal principal, string role)
    {
        return principal.GetZitadelProjectRoles().Contains(role);
    }
}
