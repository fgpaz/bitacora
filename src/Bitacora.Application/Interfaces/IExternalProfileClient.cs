namespace NuestrasCuentitas.Bitacora.Application.Interfaces;

public interface IExternalProfileClient
{
    ValueTask<string> GetProfileHealthAsync(CancellationToken cancellationToken = default);
}
