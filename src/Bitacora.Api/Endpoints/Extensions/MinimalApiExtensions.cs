namespace NuestrasCuentitas.Bitacora.Api.Endpoints.Extensions;

public static class MinimalApiExtensions
{
    public static RouteHandlerBuilder WithCommonOpenApi(this RouteHandlerBuilder builder, string name, string tag)
    {
        return builder
            .WithName(name)
            .WithTags(tag)
            .WithSummary($"{tag} - {name}")
            .WithDescription($"Operacion {name} del dominio {tag}.");
    }
}
