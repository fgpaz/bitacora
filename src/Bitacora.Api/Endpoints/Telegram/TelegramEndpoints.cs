using System.Globalization;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NuestrasCuentitas.Bitacora.Api.Endpoints.Extensions;
using NuestrasCuentitas.Bitacora.Api.Extensions;
using NuestrasCuentitas.Bitacora.Api.Security;
using NuestrasCuentitas.Bitacora.Application.Common;
using NuestrasCuentitas.Bitacora.Application.Commands.Telegram;
using NuestrasCuentitas.Bitacora.Application.Queries.Telegram;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using Shared.Contract.Telegram;

namespace NuestrasCuentitas.Bitacora.Api.Endpoints.Telegram;

public static class TelegramEndpoints
{
    private const string Tag = "Telegram";

    public static void MapTelegramEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/telegram")
            .WithTags(Tag);

        // POST /api/v1/telegram/pairing — generates a pairing code for the authenticated patient (RF-TG-001)
        group.MapPost("/pairing", async Task<IResult>(
                HttpContext httpContext,
                [FromServices] CurrentAuthenticatedPatientResolver currentPatientResolver,
                [FromServices] IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var currentPatient = await currentPatientResolver.ResolveAsync(httpContext, cancellationToken);
                var command = new GeneratePairingCodeCommand(
                    currentPatient.User.UserId,
                    httpContext.GetTraceId());

                var result = await mediator.Send(command, cancellationToken);
                return Results.Ok(result);
            })
            .RequireRateLimiting("write")
            .WithName("GenerateTelegramPairingCode")
            .WithTags(Tag)
            .WithSummary("Genera un codigo de vinculacion para Telegram")
            .WithDescription("Requiere usuario autenticado con consentimiento activo. Devuelve BIT-XXXXX con TTL 15 min.")
            .Produces<GeneratePairingCodeResponse>(StatusCodes.Status200OK)
            .Produces<BitacoraException>(StatusCodes.Status400BadRequest)
            .Produces<BitacoraException>(StatusCodes.Status403Forbidden);

        // GET /api/v1/telegram/session — returns current Telegram session status for the authenticated patient
        group.MapGet("/session", async Task<IResult>(
                HttpContext httpContext,
                [FromServices] CurrentAuthenticatedPatientResolver currentPatientResolver,
                [FromServices] IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var currentPatient = await currentPatientResolver.ResolveAsync(httpContext, cancellationToken);
                var session = await mediator.Send(
                    new GetTelegramSessionQuery(currentPatient.User.UserId, httpContext.GetTraceId()),
                    cancellationToken);

                return Results.Ok(session with { ChatId = null });
            })
            .RequireAuthorization()
            .WithName("GetTelegramSession")
            .WithTags(Tag)
            .WithSummary("Estado de vinculacion Telegram del paciente autenticado")
            .Produces<TelegramSessionResponse>(StatusCodes.Status200OK);

        // Webhook endpoint — Telegram sends POST with update payload
        // Rate-limited per IP (webhook policy); secret-token validated before dispatch.
        group.MapPost("/webhook", async Task<IResult>(
            [FromBody] TelegramWebhookRequest request,
            [FromHeader(Name = "X-Telegram-Webhook-Secret")] string? webhookSecret,
            [FromServices] IMediator mediator,
            [FromServices] ILogger<Program> logger,
            [FromServices] IConfiguration configuration,
            CancellationToken cancellationToken) =>
        {
            // Explicit secret-token validation — fail-closed.
            var expectedToken = configuration["Telegram:WebhookSecretToken"];
            if (!string.IsNullOrWhiteSpace(expectedToken))
            {
            // Intentional 200 return: Telegram stops re-delivery only on HTTP 2xx.
            // Fail-closed denies (no bot message) return 200 with null BotMessage,
            // which is indistinguishable from a silent success from Telegram's perspective.
            if (!expectedToken.Equals(webhookSecret ?? string.Empty, StringComparison.Ordinal))
            {
                logger.LogWarning("TelegramWebhook: invalid or missing secret token");
                return Results.Ok(new TelegramWebhookResponse(
                    Accepted: false,
                    ErrorCode: "FORBIDDEN",
                    BotMessage: null));
            }
            }

            var command = new HandleWebhookUpdateCommand(
                Payload: request.Update,
                ChatId: request.ChatId,
                TraceId: request.TraceId,
                CallbackQueryId: request.CallbackQueryId);

            var result = await mediator.Send(command, cancellationToken);

            // Always return 200 to Telegram to stop re-delivery.
            // BotMessage is null for fail-closed denies (silent, no leak).
            return Results.Ok(new TelegramWebhookResponse(
                Accepted: result.Accepted,
                ErrorCode: result.ErrorCode,
                BotMessage: result.BotMessage));
        })
        .RequireRateLimiting("webhook")
        .WithName("TelegramWebhook")
        .WithTags(Tag)
        .WithSummary("Webhook para actualizaciones entrantes del bot de Telegram")
        .WithDescription("Receives Telegram updates (e.g. /start CODE, mood input). Always returns 200 to Telegram.")
        .Produces<TelegramWebhookResponse>(StatusCodes.Status200OK);
    }
}

public sealed record TelegramWebhookRequest(
    string? Update,
    string? ChatId,
    Guid TraceId,
    string? CallbackQueryId);

public sealed record TelegramWebhookResponse(
    bool Accepted,
    string? ErrorCode,
    string? BotMessage);
