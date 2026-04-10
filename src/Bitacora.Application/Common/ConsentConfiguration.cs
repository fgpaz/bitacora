namespace NuestrasCuentitas.Bitacora.Application.Common;

public sealed class ConsentConfiguration
{
    public string ActiveVersion { get; init; } = string.Empty;
    public string ActiveText { get; init; } = string.Empty;
    public IReadOnlyList<ConsentSection> Sections { get; init; } = [];
}

public sealed record ConsentSection(string Id, string Title, string Body);
