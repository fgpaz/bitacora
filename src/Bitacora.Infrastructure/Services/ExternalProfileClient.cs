using Microsoft.Extensions.Logging;
using NuestrasCuentitas.Bitacora.Application.Interfaces;

namespace NuestrasCuentitas.Bitacora.Infrastructure.Services;

public sealed class ExternalProfileClient(
    IHttpClientFactory httpClientFactory,
    ILogger<ExternalProfileClient> logger) : IExternalProfileClient
{
    public async ValueTask<string> GetProfileHealthAsync(CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        var client = httpClientFactory.CreateClient("external-default");

        try
        {
            using var response = await client.SendAsync(request, cancellationToken);
            return response.IsSuccessStatusCode ? "healthy" : "degraded";
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "External profile health check failed.");
            return "unreachable";
        }
    }
}
