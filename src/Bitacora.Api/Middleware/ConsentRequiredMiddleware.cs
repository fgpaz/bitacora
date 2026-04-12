using NuestrasCuentitas.Bitacora.Api.Extensions;
using NuestrasCuentitas.Bitacora.Application.Common;
using NuestrasCuentitas.Bitacora.Application.Interfaces;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Transactions;
using NuestrasCuentitas.Bitacora.Domain.Entities;
using NuestrasCuentitas.Bitacora.Domain.Enums;

namespace NuestrasCuentitas.Bitacora.Api.Middleware;

public sealed class ConsentRequiredMiddleware(RequestDelegate next)
{
    /// <summary>
    /// Clinical entity path segments that require active consent for write operations.
    /// Pattern: /api/v1/{clinical-entity} — all POST requests to these paths are protected.
    /// </summary>
    private static readonly HashSet<string> ClinicalEntityPaths = new(StringComparer.OrdinalIgnoreCase)
    {
        "/api/v1/mood-entries",
        "/api/v1/daily-checkins"
    };

    private bool IsProtectedClinicalWrite(HttpRequest request)
    {
        if (!HttpMethods.IsPost(request.Method))
            return false;

        if (!request.Path.StartsWithSegments("/api/v1", StringComparison.OrdinalIgnoreCase))
            return false;

        // Protect known clinical entity paths
        if (ClinicalEntityPaths.Contains(request.Path.Value ?? string.Empty, StringComparer.OrdinalIgnoreCase))
            return true;

        // Future-proof: protect any /api/v1/<entity> POST that writes clinical data.
        // Heuristic: path matches /api/v1/<segment> where segment is not an auth/resource path.
        // Explicit deny-list for known non-clinical paths.
        var nonClinicalSegments = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "/api/v1/auth",
            "/api/v1/telegram",
            "/api/v1/export"
        };

        foreach (var nc in nonClinicalSegments)
        {
            if (request.Path.StartsWithSegments(nc, StringComparison.OrdinalIgnoreCase))
                return false;
        }

        // All other /api/v1/ POST paths are assumed to write clinical data until proven otherwise.
        return true;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IUserRepository userRepository,
        IConsentGrantRepository consentGrantRepository,
        IAccessAuditRepository accessAuditRepository,
        IBitacoraUnitOfWork unitOfWork,
        IPseudonymizationService pseudonymizationService)
    {
        if (!IsProtectedClinicalWrite(context.Request))
        {
            await next(context);
            return;
        }

        if (context.User.Identity?.IsAuthenticated != true)
        {
            throw new BitacoraException("UNAUTHORIZED", "Necesitás autenticarte para continuar.", StatusCodes.Status401Unauthorized);
        }

        var supabaseUserId = context.User.GetSupabaseUserId();
        var currentUser = await userRepository.GetBySupabaseUserIdAsync(supabaseUserId, context.RequestAborted);
        if (currentUser is null)
        {
            throw new BitacoraException("CONSENT_REQUIRED", "Debés completar tu consentimiento antes de registrar datos.", StatusCodes.Status403Forbidden);
        }

        var activeConsent = await consentGrantRepository.GetActiveByPatientAsync(currentUser.UserId, context.RequestAborted);
        if (activeConsent is not null)
        {
            await next(context);
            return;
        }

        await accessAuditRepository.AddAsync(
            AccessAudit.Create(
                context.GetTraceId(),
                currentUser.UserId,
                pseudonymizationService.CreatePseudonym(currentUser.UserId),
                AuditActionType.Read,
                "consent_grant",
                null,
                currentUser.UserId,
                AuditOutcome.Denied,
                DateTime.UtcNow),
            context.RequestAborted);

        await unitOfWork.SaveChangesAsync(context.RequestAborted);

        throw new BitacoraException("CONSENT_REQUIRED", "Debés aceptar el consentimiento informado antes de registrar datos.", StatusCodes.Status403Forbidden);
    }
}
