using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using NuestrasCuentitas.Bitacora.Application.Interfaces;

namespace NuestrasCuentitas.Bitacora.Infrastructure.Services;

public sealed class AesEncryptionService(IConfiguration configuration) : IEncryptionService
{
    private const int NonceSize = 12;
    private const int TagSize = 16;

    public byte[] EncryptObject<T>(T payload)
    {
        var json = JsonSerializer.Serialize(payload);
        return EncryptString(json);
    }

    public byte[] EncryptString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException("Encryption input cannot be empty.");
        }

        var plaintext = Encoding.UTF8.GetBytes(value);
        var nonce = RandomNumberGenerator.GetBytes(NonceSize);
        var ciphertext = new byte[plaintext.Length];
        var tag = new byte[TagSize];

        using var aesGcm = new AesGcm(GetKeyMaterial(), TagSize);
        aesGcm.Encrypt(nonce, plaintext, ciphertext, tag);

        var payload = new byte[NonceSize + TagSize + ciphertext.Length];
        Buffer.BlockCopy(nonce, 0, payload, 0, NonceSize);
        Buffer.BlockCopy(tag, 0, payload, NonceSize, TagSize);
        Buffer.BlockCopy(ciphertext, 0, payload, NonceSize + TagSize, ciphertext.Length);
        return payload;
    }

    public string ComputeSha256(string value)
    {
        var normalized = value.Trim().ToLowerInvariant();
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(normalized));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    public int GetActiveKeyVersion()
    {
        return configuration.GetValue<int?>("Encryption:ActiveKeyVersion") ?? 1;
    }

    private byte[] GetKeyMaterial()
    {
        var configured = configuration["BITACORA_ENCRYPTION_KEY"] ?? configuration["Encryption:Key"];
        if (string.IsNullOrWhiteSpace(configured))
        {
            throw new InvalidOperationException("BITACORA_ENCRYPTION_KEY is required.");
        }

        if (TryReadBase64(configured, out var base64Bytes))
        {
            return EnsureKeyLength(base64Bytes);
        }

        return EnsureKeyLength(Encoding.UTF8.GetBytes(configured));
    }

    private static bool TryReadBase64(string configured, out byte[] bytes)
    {
        try
        {
            bytes = Convert.FromBase64String(configured);
            return true;
        }
        catch (FormatException)
        {
            bytes = [];
            return false;
        }
    }

    private static byte[] EnsureKeyLength(byte[] keyMaterial)
    {
        if (keyMaterial.Length != 32)
        {
            throw new InvalidOperationException("BITACORA_ENCRYPTION_KEY must resolve to exactly 32 bytes for AES-256-GCM.");
        }

        return keyMaterial;
    }
}
