using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Correlate.AspNetCore;
using Correlate.DependencyInjection;
using Scalar.AspNetCore;
using System.Threading.RateLimiting;
using NuestrasCuentitas.Bitacora.Api.Extensions;
using NuestrasCuentitas.Bitacora.Api.Endpoints.Analytics;
using NuestrasCuentitas.Bitacora.Api.Endpoints.Auth;
using NuestrasCuentitas.Bitacora.Api.Endpoints.Consent;
using NuestrasCuentitas.Bitacora.Api.Endpoints.Export;
using NuestrasCuentitas.Bitacora.Api.Endpoints.Registro;
using NuestrasCuentitas.Bitacora.Api.Endpoints.Visualizacion;
using NuestrasCuentitas.Bitacora.Api.Endpoints.Vinculos;
using NuestrasCuentitas.Bitacora.Api.Endpoints.Telegram;
using NuestrasCuentitas.Bitacora.Api.Workers;
using NuestrasCuentitas.Bitacora.Infrastructure.Options;
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
using System.IO;

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
builder.Services.AddScoped<CurrentAuthenticatedProfessionalResolver>();
builder.Services.AddScoped<ProfessionalDataAccessAuthorizer>();
builder.Services.AddScoped<ReadinessProbe>();
builder.Services.Configure<TelemetryOptions>(builder.Configuration.GetSection(TelemetryOptions.SectionName));
builder.Services.Configure<ReminderConfig>(builder.Configuration.GetSection(ReminderConfig.SectionName));
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.AllowDuplicateProperties = false;
});

builder.Services.AddDataAccess(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddSetupEventBus(builder.Configuration);
builder.Services.AddHostedService<ReminderWorker>();
var zitadelAuthentication = ZitadelAuthenticationOptions.FromConfiguration(builder.Configuration);
builder.Services.AddSingleton(zitadelAuthentication);

// ── Rate Limiting ────────────────────────────────────────────────────────────
// Fail-closed: requests that exceed the limit get a 429 before reaching handlers.
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Policy "auth": protects /api/v1/auth/bootstrap and auth-adjacent paths.
    // 10 requests per IP per minute. Token bucket so bursts up to 10 are allowed.
    options.AddPolicy("auth", context =>
        RateLimitPartition.GetTokenBucketLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new TokenBucketRateLimiterOptions
            {
                TokenLimit = 10,
                ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                TokensPerPeriod = 10,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));

    // Policy "write": all POST/PATCH/DELETE from authenticated users.
    // 30 requests per user per minute. Fixed window resets at minute boundary.
    options.AddPolicy("write", context =>
    {
        var userId = context.User.Identity?.IsAuthenticated == true
            ? context.User.FindFirst("sub")?.Value
              ?? context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
              ?? "anonymous"
            : context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: userId,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 30,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            });
    });

    // Policy "webhook": Telegram webhook only. 20 req/IP/min.
    options.AddPolicy("webhook", context =>
        RateLimitPartition.GetTokenBucketLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new TokenBucketRateLimiterOptions
            {
                TokenLimit = 20,
                ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                TokensPerPeriod = 20,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";
        context.HttpContext.Response.Headers.RetryAfter = "60";
        await context.HttpContext.Response.WriteAsync(
            """{"error":"RATE_LIMIT_EXCEEDED","message":"Demasiadas solicitudes. Intentá de nuevo en un minuto.","retryAfter":60}""",
            cancellationToken);
    };
});

// ── Request Size Limits ──────────────────────────────────────────────────────
// Default: 64 KiB body for all endpoints.
// Telegram webhook: 256 KiB (Update payload can be larger).
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 1024 * 64; // 64 KiB
});
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 1024 * 64; // 64 KiB default
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.Authority = zitadelAuthentication.Authority;
        if (!string.IsNullOrWhiteSpace(zitadelAuthentication.MetadataAddress))
        {
            options.MetadataAddress = zitadelAuthentication.MetadataAddress;
        }
        options.RequireHttpsMetadata = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = zitadelAuthentication.Authority,
            ValidateAudience = true,
            ValidAudience = zitadelAuthentication.Audience,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            ValidAlgorithms = [SecurityAlgorithms.RsaSha256]
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var authSubject = context.Principal?.GetAuthSubject();
                if (string.IsNullOrWhiteSpace(authSubject))
                {
                    context.Fail("Missing sub claim.");
                    return;
                }

                using var scope = context.HttpContext.RequestServices.CreateScope();
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var currentUser = await userRepository.GetByAuthSubjectAsync(authSubject, context.HttpContext.RequestAborted);
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

// ── CORS ──────────────────────────────────────────────────────────────────────
// Explicit allowlist — no wildcard. Add your frontend origin(s) to AllowedOrigins.
builder.Services.AddCors(options =>
{
    options.AddPolicy("BitacoraFrontend", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        if (allowedOrigins.Length == 0)
        {
            // Fail-closed in production; permissive in development only.
            if (builder.Environment.IsDevelopment())
            {
                policy.SetIsOriginAllowed(_ => true);
            }
            // else: empty allowlist means no CORS policy is active — fail-closed.
        }
        else
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowCredentials()
                  .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
                  .WithHeaders("Authorization", "Content-Type", "X-Correlation-ID")
                  .WithExposedHeaders("X-Trace-Id")
                  .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
        }
    });
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

// ── Security Headers + HSTS ───────────────────────────────────────────────────
app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        if (!context.Response.HasStarted)
        {
            // Security headers (OWASP Top 10 / SOC2 baseline)
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("X-Frame-Options", "DENY");
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
            context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
            context.Response.Headers.Append("Content-Security-Policy",
                "default-src 'none'; frame-ancestors 'none'; base-uri 'self'");
            context.Response.Headers.Append("Permissions-Policy", "accelerometer=(), camera=(), geolocation=(), microphone=()");

            // HSTS (production only, after HTTPS)
            if (app.Environment.IsProduction())
            {
                context.Response.Headers.Append("Strict-Transport-Security",
                    "max-age=31536000; includeSubDomains; preload");
            }
        }
        return Task.CompletedTask;
    });
    await next();
});

// ── CORS ───────────────────────────────────────────────────────────────────────
app.UseCors("BitacoraFrontend");

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Title = "Bitacora API";
    options.OpenApiRoutePattern = "/openapi/{documentName}.json";
});

app.MapGet("/", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();
app.MapGet("/swagger", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();
app.MapGet("/health", () => Results.Ok(new { status = "ok" })).ExcludeFromDescription();

// ── Rate Limiter ──────────────────────────────────────────────────────────────
app.UseRateLimiter();

// Health/ready mapped AFTER rate limiter so it is protected by rate limiting
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
app.MapVinculosEndpoints();
app.MapVisualizacionEndpoints();
app.MapExportEndpoints();
app.MapTelegramEndpoints();
app.MapAnalyticsEndpoints();

#endregion Middleware Pipeline

app.Run();

namespace NuestrasCuentitas.Bitacora.Api
{
    public partial class Program
    {
    }
}
