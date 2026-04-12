using Mediator;
using Microsoft.AspNetCore.Mvc;
using NuestrasCuentitas.Bitacora.Api.Endpoints.Extensions;
using NuestrasCuentitas.Bitacora.Api.Extensions;
using NuestrasCuentitas.Bitacora.Api.Security;
using NuestrasCuentitas.Bitacora.Application.Common;
using NuestrasCuentitas.Bitacora.Application.Commands.Consent;
using NuestrasCuentitas.Bitacora.Application.Queries.Consent;

namespace NuestrasCuentitas.Bitacora.Api.Endpoints.Consent;

public static class ConsentEndpoints
{
    private const string Tag = "Consent";

    public static void MapConsentEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/consent/current", async Task<IResult>(
                HttpContext httpContext,
                [FromServices] CurrentAuthenticatedPatientResolver currentPatientResolver,
                [FromServices] IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var currentPatient = await currentPatientResolver.ResolveAsync(httpContext, cancellationToken);
                var response = await mediator.Send(
                    new GetCurrentConsentQuery(currentPatient.User.UserId, currentPatient.User.UserId, httpContext.GetTraceId()),
                    cancellationToken);

                return Results.Ok(response);
            })
            .RequireAuthorization("write")
            .Produces<GetCurrentConsentResponse>(StatusCodes.Status200OK)
            .WithCommonOpenApi("GetCurrentConsent", Tag);

        app.MapPost("/api/v1/consent", async Task<IResult>(
                HttpContext httpContext,
                [FromServices] CurrentAuthenticatedPatientResolver currentPatientResolver,
                [FromServices] IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var request = await httpContext.Request.ReadFromJsonAsync<GrantConsentRequest>(cancellationToken)
                    ?? throw new BitacoraException("INVALID_BODY", "El cuerpo del consentimiento es obligatorio.", StatusCodes.Status400BadRequest);
                var currentPatient = await currentPatientResolver.ResolveAsync(httpContext, cancellationToken);
                var response = await mediator.Send(
                    new GrantConsentCommand(
                        currentPatient.User.UserId,
                        currentPatient.User.UserId,
                        httpContext.GetTraceId(),
                        request.Version,
                        request.Accepted),
                    cancellationToken);

                return Results.Json(response, statusCode: StatusCodes.Status201Created);
            })
            .RequireAuthorization("write")
            .Accepts<GrantConsentRequest>("application/json")
            .Produces<GrantConsentResponse>(StatusCodes.Status201Created)
            .WithCommonOpenApi("GrantConsent", Tag);

        app.MapDelete("/api/v1/consent/current", async Task<IResult>(
                HttpContext httpContext,
                [FromServices] CurrentAuthenticatedPatientResolver currentPatientResolver,
                [FromServices] IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var request = await httpContext.Request.ReadFromJsonAsync<RevokeConsentRequest>(cancellationToken)
                    ?? throw new BitacoraException("INVALID_BODY", "La confirmacion de revocacion es obligatoria.", StatusCodes.Status400BadRequest);
                var currentPatient = await currentPatientResolver.ResolveAsync(httpContext, cancellationToken);
                var response = await mediator.Send(
                    new RevokeConsentCommand(
                        currentPatient.User.UserId,
                        currentPatient.User.UserId,
                        httpContext.GetTraceId(),
                        request.Confirmed),
                    cancellationToken);

                return Results.Ok(response);
            })
            .RequireAuthorization("write")
            .Accepts<RevokeConsentRequest>("application/json")
            .Produces<RevokeConsentResponse>(StatusCodes.Status200OK)
            .WithCommonOpenApi("RevokeConsent", Tag);
    }

    public sealed record GrantConsentRequest(string Version, bool Accepted);

    public sealed record RevokeConsentRequest(bool Confirmed);
}
