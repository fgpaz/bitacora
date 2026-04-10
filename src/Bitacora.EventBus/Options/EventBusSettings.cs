namespace NuestrasCuentitas.Bitacora.EventBus.Options;

public sealed class EventBusSettings
{
    public string HostAddress { get; set; } = string.Empty;
    public string User { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string QueueSuffix { get; set; } = string.Empty;
}
