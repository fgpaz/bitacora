namespace NuestrasCuentitas.Bitacora.Api.Options;

public sealed class ZitadelAuthenticationOptions
{
    public const string SectionName = "Zitadel";
    public const string DefaultAuthority = "https://id.nuestrascuentitas.com";
    public const string DefaultProjectAudience = "369306332534145382";

    public required string Authority { get; init; }
    public required string Audience { get; init; }
    public string? MetadataAddress { get; init; }

    public static ZitadelAuthenticationOptions FromConfiguration(IConfiguration configuration)
    {
        var authority = Resolve(configuration, "ZITADEL_AUTHORITY", "Zitadel:Authority")
            ?? Resolve(configuration, "ZITADEL_ISSUER", "Zitadel:Issuer")
            ?? Resolve(configuration, "ZITADEL_EXTERNALDOMAIN", "Zitadel:ExternalDomain")
            ?? DefaultAuthority;

        authority = NormalizeAuthority(authority);

        var audience = Resolve(configuration, "ZITADEL_AUDIENCE", "Zitadel:Audience")
            ?? Resolve(configuration, "ZITADEL_PROJECT_BITACORA_ID", "Zitadel:ProjectId")
            ?? DefaultProjectAudience;

        var metadataAddress = Resolve(configuration, "ZITADEL_METADATA_ADDRESS", "Zitadel:MetadataAddress")
            ?? $"{authority}/.well-known/openid-configuration";

        return new ZitadelAuthenticationOptions
        {
            Authority = authority,
            Audience = audience.Trim(),
            MetadataAddress = metadataAddress.Trim()
        };
    }

    private static string? Resolve(IConfiguration configuration, string environmentKey, string configurationKey)
    {
        var value = configuration[environmentKey] ?? configuration[configurationKey];
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static string NormalizeAuthority(string authority)
    {
        var trimmed = authority.Trim().TrimEnd('/');
        if (trimmed.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            trimmed.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return trimmed;
        }

        return $"https://{trimmed}";
    }
}
