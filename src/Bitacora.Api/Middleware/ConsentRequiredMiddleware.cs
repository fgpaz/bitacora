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
    private static readonly HashSet<string> ProtectedPaths = new(StringComparer.OrdinalIgnoreCase)
    {
        "/api/v1/mood-entries",
        "/api/v1/daily-checkins"
    };

    public async Task InvokeAsync(
        HttpContext context,
        IUserRepository userRepository,
        IConsentGrantRepository consentGrantRepository,
        IAccessAuditRepository accessAuditRepository,
        IBitacoraUnitOfWork unitOfWork,
        IPseudonymizationService pseudonymizationService)
    {
        if (!HttpMethods.IsPost(context.Request.Method) || !ProtectedPaths.Contains(context.Request.Path))
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
