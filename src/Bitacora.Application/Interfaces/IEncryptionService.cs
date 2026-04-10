namespace NuestrasCuentitas.Bitacora.Application.Interfaces;

public interface IEncryptionService
{
    byte[] EncryptObject<T>(T payload);
    byte[] EncryptString(string value);
    string ComputeSha256(string value);
    int GetActiveKeyVersion();
}
