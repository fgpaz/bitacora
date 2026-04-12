using Mediator;
using Microsoft.AspNetCore.Mvc;
using NuestrasCuentitas.Bitacora.Api.Endpoints.Extensions;
using NuestrasCuentitas.Bitacora.Api.Extensions;
using NuestrasCuentitas.Bitacora.Api.Security;
using NuestrasCuentitas.Bitacora.Application.Common;
using NuestrasCuentitas.Bitacora.Application.Interfaces;
using NuestrasCuentitas.Bitacora.Application.Commands.Vinculos;
using NuestrasCuentitas.Bitacora.Application.Queries.Vinculos;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Transactions;
using NuestrasCuentitas.Bitacora.Domain.Entities;
using NuestrasCuentitas.Bitacora.Domain.Enums;

namespace NuestrasCuentitas.Bitacora.Api.Endpoints.Vinculos;

public static class VinculosEndpoints
{
    private const string Tag = "Vinculos";

    public static void MapVinculosEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/vinculos", async Task<IResult>(
                HttpContext httpContext,
                [FromServices] CurrentAuthenticatedPatientResolver currentPatientResolver,
                [FromServices] IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var currentPatient = await currentPatientResolver.ResolveAsync(httpContext, cancellationToken);
                var response = await mediator.Send(
                    new GetCareLinksByPatientQuery(
                        currentPatient.User.UserId,
                        currentPatient.User.UserId,
                        httpContext.GetTraceId()),
                    cancellationToken);

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .Produces<GetCareLinksResponse>(StatusCodes.Status200OK)
            .WithCommonOpenApi("GetVinculos", Tag);

        app.MapGet("/api/v1/vinculos/active", async Task<IResult>(
                HttpContext httpContext,
                [FromServices] CurrentAuthenticatedPatientResolver currentPatientResolver,
                [FromServices] IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var currentPatient = await currentPatientResolver.ResolveAsync(httpContext, cancellationToken);
                var response = await mediator.Send(
                    new GetActiveCareLinksWithViewPermissionQuery(
                        currentPatient.User.UserId,
                        currentPatient.User.UserId,
                        httpContext.GetTraceId()),
                    cancellationToken);

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .Produces<GetActiveCareLinksResponse>(StatusCodes.Status200OK)
            .WithCommonOpenApi("GetActiveVinculosWithViewPermission", Tag);

        app.MapPost("/api/v1/vinculos/accept", async Task<IResult>(
                HttpContext httpContext,
                [FromServices] CurrentAuthenticatedPatientResolver currentPatientResolver,
                [FromServices] IMediator mediator,
                [FromServices] IAccessAuditRepository accessAuditRepository,
                [FromServices] IPseudonymizationService pseudonymizationService,
                [FromServices] IBitacoraUnitOfWork unitOfWork,
                CancellationToken cancellationToken) =>
            {
                var request = await httpContext.Request.ReadFromJsonAsync<AcceptBindingCodeRequest>(cancellationToken)
                    ?? throw new BitacoraException("INVALID_BODY", "El cuerpo es obligatorio.", StatusCodes.Status400BadRequest);

                var currentPatient = await currentPatientResolver.ResolveAsync(httpContext, cancellationToken);
                var traceId = httpContext.GetTraceId();

                try
                {
                    var response = await mediator.Send(
                        new AcceptCareLinkCommand(
                            request.BindingCode,
                            currentPatient.User.UserId,
                            currentPatient.User.UserId,
                            traceId),
                        cancellationToken);

                    await AuditAsync(accessAuditRepository, pseudonymizationService, unitOfWork,
                        traceId, currentPatient.User.UserId, "care_link", response.CareLinkId,
                        AuditActionType.Create, AuditOutcome.Ok, cancellationToken);

                    return Results.Json(response, statusCode: StatusCodes.Status201Created);
                }
                catch (BitacoraException ex) when (ex.StatusCode is 409 or 410 or 404)
                {
                    await AuditAsync(accessAuditRepository, pseudonymizationService, unitOfWork,
                        traceId, currentPatient.User.UserId, "care_link", null,
                        AuditActionType.Create, AuditOutcome.Denied, cancellationToken);

                    throw;
                }
            })
            .RequireAuthorization("write")
            .Accepts<AcceptBindingCodeRequest>("application/json")
            .Produces<AcceptCareLinkResponse>(StatusCodes.Status201Created)
            .Produces<BitacoraException>(StatusCodes.Status409Conflict)
            .Produces<BitacoraException>(StatusCodes.Status410Gone)
            .Produces<BitacoraException>(StatusCodes.Status404NotFound)
            .WithCommonOpenApi("AcceptVinculo", Tag);

        app.MapDelete("/api/v1/vinculos/{id:guid}", async Task<IResult>(
                Guid id,
                HttpContext httpContext,
                [FromServices] CurrentAuthenticatedPatientResolver currentPatientResolver,
                [FromServices] IMediator mediator,
                [FromServices] IAccessAuditRepository accessAuditRepository,
                [FromServices] IPseudonymizationService pseudonymizationService,
                [FromServices] IBitacoraUnitOfWork unitOfWork,
                CancellationToken cancellationToken) =>
            {
                var request = await httpContext.Request.ReadFromJsonAsync<RevokeVinculoRequest>(cancellationToken)
                    ?? throw new BitacoraException("INVALID_BODY", "El cuerpo es obligatorio.", StatusCodes.Status400BadRequest);

                var currentPatient = await currentPatientResolver.ResolveAsync(httpContext, cancellationToken);
                var traceId = httpContext.GetTraceId();

                try
                {
                    var response = await mediator.Send(
                        new RevokeCareLinkCommand(
                            id,
                            currentPatient.User.UserId,
                            currentPatient.User.UserId,
                            traceId,
                            request.Confirmed),
                        cancellationToken);

                    await AuditAsync(accessAuditRepository, pseudonymizationService, unitOfWork,
                        traceId, currentPatient.User.UserId, "care_link", id,
                        AuditActionType.Revoke, AuditOutcome.Ok, cancellationToken);

                    return Results.Ok(response);
                }
                catch (BitacoraException ex) when (ex.StatusCode is 403 or 404 or 422)
                {
                    await AuditAsync(accessAuditRepository, pseudonymizationService, unitOfWork,
                        traceId, currentPatient.User.UserId, "care_link", id,
                        AuditActionType.Revoke, AuditOutcome.Denied, cancellationToken);

                    throw;
                }
            })
            .RequireAuthorization("write")
            .Accepts<RevokeVinculoRequest>("application/json")
            .Produces<RevokeCareLinkResponse>(StatusCodes.Status200OK)
            .Produces<BitacoraException>(StatusCodes.Status403Forbidden)
            .Produces<BitacoraException>(StatusCodes.Status404NotFound)
            .Produces<BitacoraException>(StatusCodes.Status422UnprocessableEntity)
            .WithCommonOpenApi("RevokeVinculo", Tag);

        app.MapPatch("/api/v1/vinculos/{id:guid}/view-data", async Task<IResult>(
                Guid id,
                HttpContext httpContext,
                [FromServices] CurrentAuthenticatedPatientResolver currentPatientResolver,
                [FromServices] IMediator mediator,
                [FromServices] IAccessAuditRepository accessAuditRepository,
                [FromServices] IPseudonymizationService pseudonymizationService,
                [FromServices] IBitacoraUnitOfWork unitOfWork,
                CancellationToken cancellationToken) =>
            {
                var request = await httpContext.Request.ReadFromJsonAsync<UpdateCanViewDataRequest>(cancellationToken)
                    ?? throw new BitacoraException("INVALID_BODY", "El cuerpo es obligatorio.", StatusCodes.Status400BadRequest);

                var currentPatient = await currentPatientResolver.ResolveAsync(httpContext, cancellationToken);
                var traceId = httpContext.GetTraceId();

                try
                {
                    var response = await mediator.Send(
                        new UpdateCareLinkCanViewDataCommand(
                            id,
                            currentPatient.User.UserId,
                            currentPatient.User.UserId,
                            traceId,
                            request.CanViewData),
                        cancellationToken);

                    await AuditAsync(accessAuditRepository, pseudonymizationService, unitOfWork,
                        traceId, currentPatient.User.UserId, "care_link", id,
                        AuditActionType.Update, AuditOutcome.Ok, cancellationToken);

                    return Results.Ok(response);
                }
                catch (BitacoraException ex) when (ex.StatusCode is 403 or 404 or 422)
                {
                    await AuditAsync(accessAuditRepository, pseudonymizationService, unitOfWork,
                        traceId, currentPatient.User.UserId, "care_link", id,
                        AuditActionType.Update, AuditOutcome.Denied, cancellationToken);

                    throw;
                }
            })
            .RequireAuthorization("write")
            .Accepts<UpdateCanViewDataRequest>("application/json")
            .Produces<UpdateCareLinkCanViewDataResponse>(StatusCodes.Status200OK)
            .Produces<BitacoraException>(StatusCodes.Status403Forbidden)
            .Produces<BitacoraException>(StatusCodes.Status404NotFound)
            .Produces<BitacoraException>(StatusCodes.Status422UnprocessableEntity)
            .WithCommonOpenApi("UpdateVinculoCanViewData", Tag);

        // Professional-facing endpoints

        app.MapPost("/api/v1/professional/invites", async Task<IResult>(
                HttpContext httpContext,
                [FromServices] CurrentAuthenticatedProfessionalResolver currentProfessionalResolver,
                [FromServices] IMediator mediator,
                [FromServices] IAccessAuditRepository accessAuditRepository,
                [FromServices] IPseudonymizationService pseudonymizationService,
                [FromServices] IBitacoraUnitOfWork unitOfWork,
                CancellationToken cancellationToken) =>
            {
                var request = await httpContext.Request.ReadFromJsonAsync<CreateInviteRequest>(cancellationToken)
                    ?? throw new BitacoraException("INVALID_BODY", "El cuerpo es obligatorio.", StatusCodes.Status400BadRequest);

                var currentProfessional = await currentProfessionalResolver.ResolveAsync(httpContext, cancellationToken);
                var traceId = httpContext.GetTraceId();

                try
                {
                    var response = await mediator.Send(
                        new CreatePendingInviteCommand(
                            currentProfessional.User.UserId,
                            request.EmailHash,
                            currentProfessional.User.UserId,
                            traceId),
                        cancellationToken);

                    await AuditAsync(accessAuditRepository, pseudonymizationService, unitOfWork,
                        traceId, currentProfessional.User.UserId, "pending_invite", response.PendingInviteId,
                        AuditActionType.Create, AuditOutcome.Ok, cancellationToken);

                    return Results.Json(response, statusCode: StatusCodes.Status201Created);
                }
                catch (BitacoraException ex) when (ex.StatusCode is 400 or 409)
                {
                    await AuditAsync(accessAuditRepository, pseudonymizationService, unitOfWork,
                        traceId, currentProfessional.User.UserId, "pending_invite", null,
                        AuditActionType.Create, AuditOutcome.Denied, cancellationToken);

                    throw;
                }
            })
            .RequireAuthorization("write")
            .Accepts<CreateInviteRequest>("application/json")
            .Produces<CreatePendingInviteResponse>(StatusCodes.Status201Created)
            .Produces<BitacoraException>(StatusCodes.Status400BadRequest)
            .Produces<BitacoraException>(StatusCodes.Status409Conflict)
            .WithCommonOpenApi("CreateInvite", Tag);

        app.MapGet("/api/v1/professional/patients", async Task<IResult>(
                HttpContext httpContext,
                [FromServices] CurrentAuthenticatedProfessionalResolver currentProfessionalResolver,
                [FromServices] IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var currentProfessional = await currentProfessionalResolver.ResolveAsync(httpContext, cancellationToken);
                var response = await mediator.Send(
                    new GetProfessionalPatientsQuery(
                        currentProfessional.User.UserId,
                        currentProfessional.User.UserId,
                        httpContext.GetTraceId()),
                    cancellationToken);

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .Produces<GetProfessionalPatientsResponse>(StatusCodes.Status200OK)
            .WithCommonOpenApi("GetProfessionalPatients", Tag);
    }

    private static async Task AuditAsync(
        IAccessAuditRepository accessAuditRepository,
        IPseudonymizationService pseudonymizationService,
        IBitacoraUnitOfWork unitOfWork,
        Guid traceId,
        Guid actorId,
        string resourceType,
        Guid? resourceId,
        AuditActionType actionType,
        AuditOutcome outcome,
        CancellationToken cancellationToken)
    {
        await accessAuditRepository.AddAsync(
            AccessAudit.Create(
                traceId,
                actorId,
                pseudonymizationService.CreatePseudonym(actorId),
                actionType,
                resourceType,
                resourceId,
                actorId,
                outcome,
                DateTime.UtcNow),
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public sealed record AcceptBindingCodeRequest(string BindingCode);

    public sealed record RevokeVinculoRequest(bool Confirmed);

    public sealed record UpdateCanViewDataRequest(bool CanViewData);

    public sealed record CreateInviteRequest(string EmailHash);
}
