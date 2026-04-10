namespace NuestrasCuentitas.Bitacora.Api.Options;

public sealed class TelemetryOptions
{
    public const string SectionName = "Telemetry";

    public bool Enabled { get; set; } = true;
    public OtlpOptions? Otlp { get; set; } = new();

    public void Normalize()
    {
        Otlp?.Endpoint = string.IsNullOrWhiteSpace(Otlp?.Endpoint)
            ? null
            : Otlp.Endpoint.Trim();
    }

    public sealed class OtlpOptions
    {
        public bool Enabled { get; set; } = false;
        public string? Endpoint { get; set; } = "http://localhost:4317";
    }
}
