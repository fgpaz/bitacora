namespace NuestrasCuentitas.Bitacora.Application.Common;

public sealed class BitacoraException(string code, string message, int statusCode) : Exception(message)
{
    public string Code { get; } = code;
    public int StatusCode { get; } = statusCode;
}
