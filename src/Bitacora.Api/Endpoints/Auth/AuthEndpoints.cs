using Mediator;
using Microsoft.AspNetCore.Mvc;
using NuestrasCuentitas.Bitacora.Api.Endpoints.Extensions;
using NuestrasCuentitas.Bitacora.Api.Extensions;
using NuestrasCuentitas.Bitacora.Application.Commands.Auth;

namespace NuestrasCuentitas.Bitacora.Api.Endpoints.Auth;

public static class AuthEndpoints
{
    private const string Tag = "Auth";

    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/auth/bootstrap", async Task<IResult>(
                HttpContext httpContext,
                [FromServices] IMediator mediator,
                [FromQuery(Name = "invite_token")] string? inviteToken,
                CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(
                    new BootstrapPatientCommand(
                        httpContext.User.GetSupabaseUserId(),
                        httpContext.User.GetEmail(),
                        inviteToken,
                        httpContext.GetTraceId()),
                    cancellationToken);

                return Results.Ok(response);
            })
            .RequireAuthorization("write")
            .Produces<BootstrapPatientResponse>(StatusCodes.Status200OK)
            .WithCommonOpenApi("BootstrapPatient", Tag);
    }
}
