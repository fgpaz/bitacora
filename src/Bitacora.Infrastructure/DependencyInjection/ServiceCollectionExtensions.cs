using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NuestrasCuentitas.Bitacora.Application.Interfaces;
using NuestrasCuentitas.Bitacora.Infrastructure.Options;
using NuestrasCuentitas.Bitacora.Infrastructure.Services;

namespace NuestrasCuentitas.Bitacora.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ExecutionHttpOptions>(configuration.GetSection("ExecutionHttp"));

        services.AddHttpClient("external-default", (provider, client) =>
        {
            var options = provider.GetRequiredService<IOptions<ExecutionHttpOptions>>().Value;
            client.Timeout = TimeSpan.FromSeconds(Math.Clamp(options.DefaultTimeoutSeconds, 1, 120));
            client.BaseAddress = new Uri("https://localhost");
        });

        services.AddScoped<IExternalProfileClient, ExternalProfileClient>();
        services.AddScoped<IEncryptionService, AesEncryptionService>();
        services.AddScoped<IPseudonymizationService, PseudonymizationService>();

        return services;
    }
}
