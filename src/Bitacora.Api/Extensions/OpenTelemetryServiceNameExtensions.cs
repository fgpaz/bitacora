namespace NuestrasCuentitas.Bitacora.Api.Extensions;

public static class OpenTelemetryServiceNameExtensions
{
    extension(string applicationName)
    {
        public string ToTelemetryServiceName()
        {
            if (string.IsNullOrWhiteSpace(applicationName))
            {
                return "template-service";
            }

            return applicationName
                .Trim()
                .Replace(' ', '-')
                .ToLowerInvariant();
        }
    }
}
