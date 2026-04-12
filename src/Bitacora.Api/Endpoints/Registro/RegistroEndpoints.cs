using Mediator;
using Microsoft.AspNetCore.Mvc;
using NuestrasCuentitas.Bitacora.Api.Endpoints.Extensions;
using NuestrasCuentitas.Bitacora.Api.Extensions;
using NuestrasCuentitas.Bitacora.Api.Security;
using NuestrasCuentitas.Bitacora.Application.Common;
using NuestrasCuentitas.Bitacora.Application.Commands.Registro;

namespace NuestrasCuentitas.Bitacora.Api.Endpoints.Registro;

public static class RegistroEndpoints
{
    private const string Tag = "Registro";

    public static void MapRegistroEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/mood-entries", async Task<IResult>(
                HttpContext httpContext,
                [FromServices] CurrentAuthenticatedPatientResolver currentPatientResolver,
                [FromServices] IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var request = await httpContext.Request.ReadFromJsonAsync<CreateMoodEntryRequest>(cancellationToken)
                    ?? throw new BitacoraException("INVALID_BODY", "El cuerpo del estado de animo es obligatorio.", StatusCodes.Status400BadRequest);
                var currentPatient = await currentPatientResolver.ResolveAsync(httpContext, cancellationToken);
                var response = await mediator.Send(
                    new CreateMoodEntryCommand(
                        currentPatient.User.UserId,
                        currentPatient.User.UserId,
                        httpContext.GetTraceId(),
                        request.Score,
                        "api"),
                    cancellationToken);

                return response.IsDuplicate
                    ? Results.Ok(response)
                    : Results.Json(response, statusCode: StatusCodes.Status201Created);
            })
            .RequireAuthorization("write")
            .Accepts<CreateMoodEntryRequest>("application/json")
            .Produces<CreateMoodEntryResponse>(StatusCodes.Status200OK)
            .Produces<CreateMoodEntryResponse>(StatusCodes.Status201Created)
            .WithCommonOpenApi("CreateMoodEntry", Tag);

        app.MapPost("/api/v1/daily-checkins", async Task<IResult>(
                HttpContext httpContext,
                [FromServices] CurrentAuthenticatedPatientResolver currentPatientResolver,
                [FromServices] IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var request = await httpContext.Request.ReadFromJsonAsync<UpsertDailyCheckinRequest>(cancellationToken)
                    ?? throw new BitacoraException("INVALID_BODY", "El cuerpo del check-in diario es obligatorio.", StatusCodes.Status400BadRequest);
                var currentPatient = await currentPatientResolver.ResolveAsync(httpContext, cancellationToken);
                var response = await mediator.Send(
                    new UpsertDailyCheckinCommand(
                        currentPatient.User.UserId,
                        currentPatient.User.UserId,
                        httpContext.GetTraceId(),
                        request.SleepHours,
                        request.PhysicalActivity,
                        request.SocialActivity,
                        request.Anxiety,
                        request.Irritability,
                        request.MedicationTaken,
                        request.MedicationTime),
                    cancellationToken);

                return response.UpdatedExisting
                    ? Results.Ok(response)
                    : Results.Json(response, statusCode: StatusCodes.Status201Created);
            })
            .RequireAuthorization("write")
            .Accepts<UpsertDailyCheckinRequest>("application/json")
            .Produces<UpsertDailyCheckinResponse>(StatusCodes.Status200OK)
            .Produces<UpsertDailyCheckinResponse>(StatusCodes.Status201Created)
            .WithCommonOpenApi("UpsertDailyCheckin", Tag);
    }

    public sealed record CreateMoodEntryRequest(int Score);

    public sealed record UpsertDailyCheckinRequest(
        decimal SleepHours,
        bool PhysicalActivity,
        bool SocialActivity,
        bool Anxiety,
        bool Irritability,
        bool MedicationTaken,
        TimeOnly? MedicationTime);
}
