namespace NuestrasCuentitas.Bitacora.Infrastructure.Options;

public sealed class ExecutionHttpOptions
{
    public int DefaultTimeoutSeconds { get; set; } = 8;
    public int AuthTimeoutSeconds { get; set; } = 5;
}
