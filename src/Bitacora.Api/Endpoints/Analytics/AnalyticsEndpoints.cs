using System.Text.Json;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using NuestrasCuentitas.Bitacora.Api.Endpoints.Extensions;
using NuestrasCuentitas.Bitacora.Api.Extensions;
using NuestrasCuentitas.Bitacora.Api.Security;
using NuestrasCuentitas.Bitacora.Application.Common;
using NuestrasCuentitas.Bitacora.Application.Commands.Analytics;

namespace NuestrasCuentitas.Bitacora.Api.Endpoints.Analytics;

public static class AnalyticsEndpoints
{
    private const string Tag = "Analytics";

    public static void MapAnalyticsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/analytics/events", async Task<IResult>(
                HttpContext httpContext,
                [FromServices] CurrentAuthenticatedPatientResolver currentPatientResolver,
                [FromServices] IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var request = await httpContext.Request.ReadFromJsonAsync<TrackAnalyticsEventRequest>(cancellationToken)
                    ?? throw new BitacoraException("INVALID_BODY", "El cuerpo del evento es obligatorio.", StatusCodes.Status400BadRequest);

                if (string.IsNullOrWhiteSpace(request.Event))
                {
                    throw new BitacoraException("EVENT_NAME_REQUIRED", "El nombre del evento es obligatorio.", StatusCodes.Status400BadRequest);
                }

                var currentPatient = await currentPatientResolver.ResolveAsync(httpContext, cancellationToken);
                var response = await mediator.Send(
                    new TrackAnalyticsEventCommand(
                        currentPatient.User.UserId,
                        httpContext.GetTraceId(),
                        request.Event,
                        request.Props),
                    cancellationToken);

                return Results.Accepted(value: response);
            })
            .RequireRateLimiting("write")
            .Accepts<TrackAnalyticsEventRequest>("application/json")
            .Produces<TrackAnalyticsEventResponse>(StatusCodes.Status202Accepted)
            .WithCommonOpenApi("TrackAnalyticsEvent", Tag);
    }

    public sealed record TrackAnalyticsEventRequest(string Event, JsonElement? Props);
}
