using Mediator;
using Microsoft.AspNetCore.Mvc;
using NuestrasCuentitas.Bitacora.Api.Endpoints.Extensions;
using NuestrasCuentitas.Bitacora.Api.Extensions;
using NuestrasCuentitas.Bitacora.Api.Security;
using NuestrasCuentitas.Bitacora.Application.Common;
using NuestrasCuentitas.Bitacora.Application.Queries.Visualizacion;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.Domain.Enums;

namespace NuestrasCuentitas.Bitacora.Api.Endpoints.Visualizacion;

public static class VisualizacionEndpoints
{
    private const string Tag = "Visualizacion";

    public static void MapVisualizacionEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/visualizacion/timeline", async Task<IResult>(
                HttpContext httpContext,
                [FromQuery] DateOnly from,
                [FromQuery] DateOnly to,
                [FromServices] CurrentAuthenticatedPatientResolver currentPatientResolver,
                [FromServices] IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                if (to < from)
                {
                    throw new BitacoraException("INVALID_DATE_RANGE", "La fecha 'to' debe ser mayor o igual a 'from'.", StatusCodes.Status400BadRequest);
                }

                var currentPatient = await currentPatientResolver.ResolveAsync(httpContext, cancellationToken);
                var response = await mediator.Send(
                    new GetPatientTimelineQuery(
                        currentPatient.User.UserId,
                        from,
                        to,
                        currentPatient.User.UserId,
                        httpContext.GetTraceId()),
                    cancellationToken);

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .Produces<GetPatientTimelineResponse>(StatusCodes.Status200OK)
            .Produces<BitacoraException>(StatusCodes.Status400BadRequest)
            .WithCommonOpenApi("GetPatientTimeline", Tag);

        app.MapGet("/api/v1/visualizacion/summary", async Task<IResult>(
                HttpContext httpContext,
                [FromQuery] DateOnly from,
                [FromQuery] DateOnly to,
                [FromServices] CurrentAuthenticatedPatientResolver currentPatientResolver,
                [FromServices] IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                if (to < from)
                {
                    throw new BitacoraException("INVALID_DATE_RANGE", "La fecha 'to' debe ser mayor o igual a 'from'.", StatusCodes.Status400BadRequest);
                }

                var currentPatient = await currentPatientResolver.ResolveAsync(httpContext, cancellationToken);
                var response = await mediator.Send(
                    new GetPatientSummaryQuery(
                        currentPatient.User.UserId,
                        from,
                        to,
                        currentPatient.User.UserId,
                        httpContext.GetTraceId()),
                    cancellationToken);

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .Produces<Shared.Contract.Visualizacion.PatientSummaryDto>(StatusCodes.Status200OK)
            .Produces<BitacoraException>(StatusCodes.Status400BadRequest)
            .WithCommonOpenApi("GetPatientSummary", Tag);

        // Professional endpoints — CareLink + can_view_data authorization required

        app.MapGet("/api/v1/professional/patients/{patientId:guid}/summary", async Task<IResult>(
                Guid patientId,
                HttpContext httpContext,
                [FromQuery] DateOnly from,
                [FromQuery] DateOnly to,
                [FromServices] ProfessionalDataAccessAuthorizer authorizer,
                [FromServices] CurrentAuthenticatedProfessionalResolver currentProfessionalResolver,
                [FromServices] IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                if (to < from)
                {
                    throw new BitacoraException("INVALID_DATE_RANGE", "La fecha 'to' debe ser mayor o igual a 'from'.", StatusCodes.Status400BadRequest);
                }

                var currentProfessional = await currentProfessionalResolver.ResolveAsync(httpContext, cancellationToken);
                await authorizer.AuthorizeAsync(currentProfessional.User.UserId, patientId, cancellationToken);

                var response = await mediator.Send(
                    new GetPatientSummaryQuery(
                        patientId,
                        from,
                        to,
                        currentProfessional.User.UserId,
                        httpContext.GetTraceId()),
                    cancellationToken);

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .Produces<Shared.Contract.Visualizacion.PatientSummaryDto>(StatusCodes.Status200OK)
            .Produces<BitacoraException>(StatusCodes.Status400BadRequest)
            .Produces<BitacoraException>(StatusCodes.Status403Forbidden)
            .WithCommonOpenApi("GetProfessionalPatientSummary", Tag);

        app.MapGet("/api/v1/professional/patients/{patientId:guid}/timeline", async Task<IResult>(
                Guid patientId,
                HttpContext httpContext,
                [FromQuery] DateOnly from,
                [FromQuery] DateOnly to,
                [FromServices] ProfessionalDataAccessAuthorizer authorizer,
                [FromServices] CurrentAuthenticatedProfessionalResolver currentProfessionalResolver,
                [FromServices] IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                if (to < from)
                {
                    throw new BitacoraException("INVALID_DATE_RANGE", "La fecha 'to' debe ser mayor o igual a 'from'.", StatusCodes.Status400BadRequest);
                }

                var currentProfessional = await currentProfessionalResolver.ResolveAsync(httpContext, cancellationToken);
                await authorizer.AuthorizeAsync(currentProfessional.User.UserId, patientId, cancellationToken);

                var response = await mediator.Send(
                    new GetPatientTimelineQuery(
                        patientId,
                        from,
                        to,
                        currentProfessional.User.UserId,
                        httpContext.GetTraceId()),
                    cancellationToken);

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .Produces<GetPatientTimelineResponse>(StatusCodes.Status200OK)
            .Produces<BitacoraException>(StatusCodes.Status400BadRequest)
            .Produces<BitacoraException>(StatusCodes.Status403Forbidden)
            .WithCommonOpenApi("GetProfessionalPatientTimeline", Tag);

        app.MapGet("/api/v1/professional/patients/{patientId:guid}/alerts", async Task<IResult>(
                Guid patientId,
                HttpContext httpContext,
                [FromQuery] DateOnly from,
                [FromQuery] DateOnly to,
                [FromServices] ProfessionalDataAccessAuthorizer authorizer,
                [FromServices] CurrentAuthenticatedProfessionalResolver currentProfessionalResolver,
                [FromServices] IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                if (to < from)
                {
                    throw new BitacoraException("INVALID_DATE_RANGE", "La fecha 'to' debe ser mayor o igual a 'from'.", StatusCodes.Status400BadRequest);
                }

                var currentProfessional = await currentProfessionalResolver.ResolveAsync(httpContext, cancellationToken);
                await authorizer.AuthorizeAsync(currentProfessional.User.UserId, patientId, cancellationToken);

                var response = await mediator.Send(
                    new GetPatientAlertsQuery(
                        patientId,
                        from,
                        to,
                        currentProfessional.User.UserId,
                        httpContext.GetTraceId()),
                    cancellationToken);

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .Produces<IReadOnlyList<Shared.Contract.Visualizacion.PatientAlertDto>>(StatusCodes.Status200OK)
            .Produces<BitacoraException>(StatusCodes.Status400BadRequest)
            .Produces<BitacoraException>(StatusCodes.Status403Forbidden)
            .WithCommonOpenApi("GetProfessionalPatientAlerts", Tag);
    }
}
