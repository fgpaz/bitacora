using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Correlate.AspNetCore;
using Correlate.DependencyInjection;
using Scalar.AspNetCore;
using NuestrasCuentitas.Bitacora.Api.Extensions;
using NuestrasCuentitas.Bitacora.Api.Endpoints.Auth;
using NuestrasCuentitas.Bitacora.Api.Endpoints.Consent;
using NuestrasCuentitas.Bitacora.Api.Endpoints.Registro;
using NuestrasCuentitas.Bitacora.Api.Health;
using NuestrasCuentitas.Bitacora.Api.Middleware;
using NuestrasCuentitas.Bitacora.Api.Security;
using NuestrasCuentitas.Bitacora.Api.Options;
using NuestrasCuentitas.Bitacora.Application.DependencyInjection;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.DependencyInjection;
using NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Persistence;
using NuestrasCuentitas.Bitacora.EventBus;
using NuestrasCuentitas.Bitacora.Infrastructure.DependencyInjection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region Configuration

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

#endregion Configuration

#region Logging

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
if (builder.Environment.IsDevelopment())
{
    builder.Logging.AddDebug();
}

#endregion Logging

#region Service Registration

builder.Services.AddCorrelate(options => options.RequestHeaders = ["X-Correlation-ID"]);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CurrentAuthenticatedPatientResolver>();
builder.Services.AddScoped<ReadinessProbe>();
builder.Services.Configure<TelemetryOptions>(builder.Configuration.GetSection(TelemetryOptions.SectionName));
builder.Services.ConfigureHttpJsonOptions(options =>
{
    // Baseline JSON policy: reject duplicate properties while preserving compatibility.
    options.SerializerOptions.AllowDuplicateProperties = false;
});

builder.Services.AddDataAccess(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddSetupEventBus(builder.Configuration);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;

        var jwtSecret = builder.Configuration["Supabase__JwtSecret"] ?? builder.Configuration["SUPABASE_JWT_SECRET"];
        jwtSecret ??= builder.Configuration["Supabase:JwtSecret"];
        if (string.IsNullOrWhiteSpace(jwtSecret))
        {
            throw new InvalidOperationException("Supabase JWT secret is required.");
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var supabaseUserId = context.Principal?.GetSupabaseUserId();
                if (string.IsNullOrWhiteSpace(supabaseUserId))
                {
                    context.Fail("Missing sub claim.");
                    return;
                }

                using var scope = context.HttpContext.RequestServices.CreateScope();
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var currentUser = await userRepository.GetBySupabaseUserIdAsync(supabaseUserId, context.HttpContext.RequestAborted);
                if (currentUser?.SessionsRevokedAt is null)
                {
                    return;
                }

                if (!long.TryParse(context.Principal?.FindFirst("iat")?.Value, out var issuedAtUnix))
                {
                    return;
                }

                var issuedAtUtc = DateTimeOffset.FromUnixTimeSeconds(issuedAtUnix).UtcDateTime;
                if (issuedAtUtc < currentUser.SessionsRevokedAt.Value)
                {
                    context.Fail("Token has been revoked.");
                }
            }
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(options =>
{
    options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_1;
    options.ShouldInclude = _ => true;
});

var telemetryOptions = builder.Configuration
                           .GetSection(TelemetryOptions.SectionName)
                           .Get<TelemetryOptions>() ?? new TelemetryOptions();
telemetryOptions.Normalize();

if (telemetryOptions.Enabled)
{
    var telemetryServiceName = builder.Environment.ApplicationName.ToTelemetryServiceName();

    builder.Services
        .AddOpenTelemetry()
        .ConfigureResource(resource =>
        {
            resource.AddService(telemetryServiceName);
        })
        .WithTracing(tracing =>
        {
            tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation();

            if (telemetryOptions.Otlp?.Enabled == true &&
                !string.IsNullOrWhiteSpace(telemetryOptions.Otlp.Endpoint))
            {
                tracing.AddOtlpExporter(options => options.Endpoint = new Uri(telemetryOptions.Otlp.Endpoint));
            }
        })
        .WithMetrics(metrics =>
        {
            metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation();

            if (telemetryOptions.Otlp?.Enabled == true &&
                !string.IsNullOrWhiteSpace(telemetryOptions.Otlp.Endpoint))
            {
                metrics.AddOtlpExporter(options => options.Endpoint = new Uri(telemetryOptions.Otlp.Endpoint));
            }
        });
}

#endregion Service Registration

var app = builder.Build();

#region Middleware Pipeline

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Title = "Bitacora API";
    options.OpenApiRoutePattern = "/openapi/{documentName}.json";
});

app.MapGet("/", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();
app.MapGet("/swagger", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();
app.MapGet("/health", () => Results.Ok(new { status = "ok" })).ExcludeFromDescription();
app.MapGet("/health/ready", async Task<IResult>(ReadinessProbe readinessProbe, CancellationToken cancellationToken) =>
    {
        var report = await readinessProbe.CheckAsync(cancellationToken);
        return report.Status == "ready"
            ? Results.Ok(report)
            : Results.Json(report, statusCode: StatusCodes.Status503ServiceUnavailable);
    })
    .ExcludeFromDescription();

app.UseMiddleware<TraceIdMiddleware>();
app.UseMiddleware<ApiExceptionMiddleware>();
app.UseCorrelate();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ConsentRequiredMiddleware>();

if (app.Environment.IsDevelopment() &&
    app.Configuration.GetValue("DataAccess:ApplyMigrationsOnStartup", false))
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.MapAuthEndpoints();
app.MapConsentEndpoints();
app.MapRegistroEndpoints();

#endregion Middleware Pipeline

app.Run();

namespace NuestrasCuentitas.Bitacora.Api
{
    public partial class Program
    {
    }
}
