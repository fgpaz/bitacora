using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using NuestrasCuentitas.Bitacora.Application.Interfaces;

namespace NuestrasCuentitas.Bitacora.Infrastructure.Services;

public sealed class PseudonymizationService(IConfiguration configuration) : IPseudonymizationService
{
    public string CreatePseudonym(Guid actorId)
    {
        var salt = configuration["BITACORA_PSEUDONYM_SALT"] ?? configuration["Pseudonymization:Salt"];
        if (string.IsNullOrWhiteSpace(salt))
        {
            throw new InvalidOperationException("BITACORA_PSEUDONYM_SALT is required.");
        }

        var payload = $"{actorId:D}:{salt.Trim()}";
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(payload));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
