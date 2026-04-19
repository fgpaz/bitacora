using System.Text;
using Microsoft.EntityFrameworkCore;
using NuestrasCuentitas.Bitacora.Api.Options;
using NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Persistence;

namespace NuestrasCuentitas.Bitacora.Api.Health;

public sealed class ReadinessProbe(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
{
    public async Task<ReadinessProbeResponse> CheckAsync(CancellationToken cancellationToken)
    {
        var checks = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var connectionString = configuration.GetConnectionString("BitacoraDb");
        checks["connection_string"] = string.IsNullOrWhiteSpace(connectionString) ? "missing" : "ok";

        var zitadel = ZitadelAuthenticationOptions.FromConfiguration(configuration);
        checks["zitadel_authority"] = string.IsNullOrWhiteSpace(zitadel.Authority) ? "missing" : "ok";
        checks["zitadel_audience"] = string.IsNullOrWhiteSpace(zitadel.Audience) ? "missing" : "ok";
        checks["zitadel_metadata"] = string.IsNullOrWhiteSpace(zitadel.MetadataAddress) ? "missing" : "ok";

        checks["encryption_key"] = ValidateEncryptionKey();

        var pseudonymSalt = configuration["BITACORA_PSEUDONYM_SALT"] ?? configuration["Pseudonymization:Salt"];
        checks["pseudonym_salt"] = string.IsNullOrWhiteSpace(pseudonymSalt) ? "missing" : "ok";

        if (checks.Values.Any(value => !string.Equals(value, "ok", StringComparison.Ordinal)))
        {
            checks["database"] = "blocked_by_config";
            return new ReadinessProbeResponse("not_ready", checks);
        }

        try
        {
            using var scope = serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            checks["database"] = await dbContext.Database.CanConnectAsync(cancellationToken) ? "ok" : "unreachable";
        }
        catch (Exception)
        {
            checks["database"] = "unreachable";
        }

        var status = checks.Values.All(value => string.Equals(value, "ok", StringComparison.Ordinal))
            ? "ready"
            : "not_ready";

        return new ReadinessProbeResponse(status, checks);
    }

    private string ValidateEncryptionKey()
    {
        var configured = configuration["BITACORA_ENCRYPTION_KEY"] ?? configuration["Encryption:Key"];
        if (string.IsNullOrWhiteSpace(configured))
        {
            return "missing";
        }

        if (TryReadBase64(configured, out var base64Bytes))
        {
            return base64Bytes.Length == 32 ? "ok" : "invalid_length";
        }

        return Encoding.UTF8.GetByteCount(configured) == 32 ? "ok" : "invalid_length";
    }

    private static bool TryReadBase64(string configured, out byte[] bytes)
    {
        try
        {
            bytes = Convert.FromBase64String(configured);
            return true;
        }
        catch (FormatException)
        {
            bytes = [];
            return false;
        }
    }
}

public sealed record ReadinessProbeResponse(string Status, IReadOnlyDictionary<string, string> Checks);
