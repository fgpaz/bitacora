using System.Globalization;
using System.Text;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using NuestrasCuentitas.Bitacora.Api.Endpoints.Extensions;
using NuestrasCuentitas.Bitacora.Api.Extensions;
using NuestrasCuentitas.Bitacora.Api.Security;
using NuestrasCuentitas.Bitacora.Application.Common;
using NuestrasCuentitas.Bitacora.Application.Queries.Export;
using Shared.Contract.Export;

namespace NuestrasCuentitas.Bitacora.Api.Endpoints.Export;

public static class ExportEndpoints
{
    private const string Tag = "Export";

    public static void MapExportEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/export/patient-summary", async Task<IResult>(
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
                    new ExportPatientSummaryQuery(
                        currentPatient.User.UserId,
                        from,
                        to,
                        currentPatient.User.UserId,
                        httpContext.GetTraceId()),
                    cancellationToken);

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .Produces<PatientExportDto>(StatusCodes.Status200OK)
            .Produces<BitacoraException>(StatusCodes.Status400BadRequest)
            .WithCommonOpenApi("ExportPatientSummary", Tag);

        // Owner-only CSV export (RF-EXP-002)
        // GET /api/v1/export/{patientId}/constraints — returns export eligibility for a patient
        // Auth: any authenticated user (professional or patient)
        // Professional: always returns allowed=false with reason (export is owner-only)
        // Patient: returns allowed=true if viewing own data
        app.MapGet("/api/v1/export/{patientId}/constraints", async Task<IResult>(
                HttpContext httpContext,
                [FromRoute] Guid patientId,
                [FromServices] CurrentAuthenticatedPatientResolver currentPatientResolver,
                [FromServices] CurrentAuthenticatedProfessionalResolver currentProfessionalResolver,
                [FromServices] ProfessionalDataAccessAuthorizer authorizer,
                [FromServices] IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                ResolvedPatientContext patientCtx;
                try
                {
                    patientCtx = await currentPatientResolver.ResolveAsync(httpContext, cancellationToken);
                }
                catch (BitacoraException ex) when (ex.Code == "FORBIDDEN")
                {
                    // Professional access — authorize before revealing any patient data exists
                    var professionalCtx = await currentProfessionalResolver.ResolveAsync(httpContext, cancellationToken);
                    await authorizer.AuthorizeAsync(professionalCtx.User.UserId, patientId, cancellationToken);

                    var response = await mediator.Send(
                        new GetExportConstraintsQuery(
                            patientId,
                            professionalCtx.User.UserId,
                            IsProfessional: true,
                            httpContext.GetTraceId()),
                        cancellationToken);
                    return Results.Ok(response);
                }

                var result = await mediator.Send(
                    new GetExportConstraintsQuery(
                        patientId,
                        patientCtx.User.UserId,
                        IsProfessional: false,
                        httpContext.GetTraceId()),
                    cancellationToken);
                return Results.Ok(result);
            })
            .RequireAuthorization()
            .Produces<ExportConstraintDto>(StatusCodes.Status200OK)
            .WithCommonOpenApi("GetExportConstraints", Tag);

        app.MapGet("/api/v1/export/patient-summary/csv", async Task<IResult>(
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
                var export = await mediator.Send(
                    new ExportPatientSummaryQuery(
                        currentPatient.User.UserId,
                        from,
                        to,
                        currentPatient.User.UserId,
                        httpContext.GetTraceId()),
                    cancellationToken);

                var sb = new StringBuilder();
                sb.AppendLine("fecha,mood_score,sleep_hours,physical_activity,social_activity,anxiety,irritability,medication_taken");

                foreach (var entry in export.Entries)
                {
                    var mood = entry.Mood?.Score?.ToString(CultureInfo.InvariantCulture) ?? "";
                    var sleep = entry.Checkin?.SleepHours?.ToString(CultureInfo.InvariantCulture) ?? "";
                    var physical = entry.Checkin?.PhysicalActivity?.ToString(CultureInfo.InvariantCulture) ?? "";
                    var social = entry.Checkin?.SocialActivity?.ToString(CultureInfo.InvariantCulture) ?? "";
                    var anxiety = entry.Checkin?.Anxiety?.ToString(CultureInfo.InvariantCulture) ?? "";
                    var irritability = entry.Checkin?.Irritability?.ToString(CultureInfo.InvariantCulture) ?? "";
                    var medication = entry.Checkin?.MedicationTaken?.ToString(CultureInfo.InvariantCulture) ?? "";
                    sb.AppendLine($"{entry.Date:yyyy-MM-dd},{mood},{sleep},{physical},{social},{anxiety},{irritability},{medication}");
                }

                var bytes = Encoding.UTF8.GetBytes(sb.ToString());
                return Results.File(bytes, "text/csv", $"bitacora-export-{from:yyyyMMdd}-{to:yyyyMMdd}.csv");
            })
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK, contentType: "text/csv")
            .Produces<BitacoraException>(StatusCodes.Status400BadRequest)
            .WithCommonOpenApi("ExportPatientSummaryCsv", Tag);
    }
}