using System.Text.RegularExpressions;
using MassTransit;

namespace NuestrasCuentitas.Bitacora.EventBus.EventBusConsumer;

public static class ConfigureConsumerManager
{
    private const string QueueArgument = "x-queue-type";

    public static List<Tuple<string, Action<IReceiveEndpointConfigurator, IRegistrationContext>>> GetList()
    {
        return
        [
            new Tuple<string, Action<IReceiveEndpointConfigurator, IRegistrationContext>>(
                GetQueueName("UsuarioActualizado"),
                ConfigureUsuarioActualizadoConsumer)
        ];
    }

    public static void AddConsumers(IRegistrationConfigurator config)
    {
        config.AddConsumer<UsuarioActualizadoConsumer>();
    }

    private static void ConfigureUsuarioActualizadoConsumer(IReceiveEndpointConfigurator endpoint, IRegistrationContext context)
    {
        if (endpoint is IRabbitMqReceiveEndpointConfigurator rabbitMqEndpoint)
        {
            rabbitMqEndpoint.SetQueueArgument(QueueArgument, "quorum");
        }

        endpoint.PrefetchCount = 16;
        endpoint.UseMessageRetry(retry => retry.Intervals(500, 1500, 3000));
        endpoint.ConfigureConsumer<UsuarioActualizadoConsumer>(context);
    }

    private static string GetQueueName(string baseName)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Local";
        var suffix = NormalizeSuffix(Environment.GetEnvironmentVariable("RABBITMQ_QUEUE_SUFFIX"));

        return string.IsNullOrWhiteSpace(suffix)
            ? $"{baseName}.{environment}"
            : $"{baseName}.{environment}.{suffix}";
    }

    private static string NormalizeSuffix(string? suffix)
    {
        if (string.IsNullOrWhiteSpace(suffix))
        {
            return string.Empty;
        }

        return Regex.Replace(suffix.Trim(), "[^A-Za-z0-9_.-]", "-");
    }
}
