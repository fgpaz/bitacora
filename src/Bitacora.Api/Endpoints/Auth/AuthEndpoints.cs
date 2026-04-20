using System.Net.Http.Headers;
using System.Text.Json;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using NuestrasCuentitas.Bitacora.Api.Endpoints.Extensions;
using NuestrasCuentitas.Bitacora.Api.Extensions;
using NuestrasCuentitas.Bitacora.Api.Options;
using NuestrasCuentitas.Bitacora.Application.Commands.Auth;
using NuestrasCuentitas.Bitacora.Application.Common;

namespace NuestrasCuentitas.Bitacora.Api.Endpoints.Auth;

public static class AuthEndpoints
{
    private const string Tag = "Auth";

    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/auth/bootstrap", async Task<IResult>(
                HttpContext httpContext,
                [FromServices] IMediator mediator,
                [FromServices] IHttpClientFactory httpClientFactory,
                [FromServices] ZitadelAuthenticationOptions zitadelAuthentication,
                [FromQuery(Name = "invite_token")] string? inviteToken,
                CancellationToken cancellationToken) =>
            {
                var authSubject = httpContext.User.GetAuthSubject();
                var email = await ResolveEmailAsync(
                    httpContext,
                    httpClientFactory,
                    zitadelAuthentication,
                    authSubject,
                    cancellationToken);

                var response = await mediator.Send(
                    new BootstrapPatientCommand(
                        authSubject,
                        email,
                        inviteToken,
                        httpContext.GetTraceId()),
                    cancellationToken);

                return Results.Ok(response);
            })
            .RequireRateLimiting("auth")
            .RequireAuthorization()
            .Produces<BootstrapPatientResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithCommonOpenApi("BootstrapPatient", Tag);
    }

    private static async ValueTask<string> ResolveEmailAsync(
        HttpContext httpContext,
        IHttpClientFactory httpClientFactory,
        ZitadelAuthenticationOptions zitadelAuthentication,
        string authSubject,
        CancellationToken cancellationToken)
    {
        var email = httpContext.User.GetEmailOrDefault();
        if (!string.IsNullOrWhiteSpace(email))
        {
            return email;
        }

        var accessToken = GetBearerToken(httpContext);
        var userInfoUri = new Uri(new Uri(zitadelAuthentication.Authority), "/oidc/v1/userinfo");
        using var request = new HttpRequestMessage(HttpMethod.Get, userInfoUri);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var client = httpClientFactory.CreateClient();
        using var response = await client.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new BitacoraException("ONB_001_JWT_INVALID", "No se pudo resolver el email del token autenticado.", StatusCodes.Status401Unauthorized);
        }

        await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(responseStream, cancellationToken: cancellationToken);
        var root = document.RootElement;

        if (!root.TryGetProperty("sub", out var subElement) ||
            !string.Equals(subElement.GetString(), authSubject, StringComparison.Ordinal))
        {
            throw new BitacoraException("ONB_001_JWT_INVALID", "UserInfo no coincide con el token autenticado.", StatusCodes.Status401Unauthorized);
        }

        if (root.TryGetProperty("email", out var emailElement))
        {
            var userInfoEmail = emailElement.GetString();
            if (!string.IsNullOrWhiteSpace(userInfoEmail))
            {
                return userInfoEmail;
            }
        }

        throw new BitacoraException("ONB_001_JWT_INVALID", "El token autenticado no tiene email resoluble.", StatusCodes.Status401Unauthorized);
    }

    private static string GetBearerToken(HttpContext httpContext)
    {
        var authorization = httpContext.Request.Headers.Authorization.ToString();
        const string bearerPrefix = "Bearer ";
        if (authorization.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return authorization[bearerPrefix.Length..].Trim();
        }

        throw new BitacoraException("UNAUTHORIZED", "Falta el bearer token autenticado.", StatusCodes.Status401Unauthorized);
    }
}
