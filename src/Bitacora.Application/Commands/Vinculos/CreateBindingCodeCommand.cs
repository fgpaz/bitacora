using Mediator;
using Microsoft.Extensions.Logging;
using NuestrasCuentitas.Bitacora.Application.Common;
using NuestrasCuentitas.Bitacora.Application.Interfaces;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Transactions;
using NuestrasCuentitas.Bitacora.Domain.Entities;

namespace NuestrasCuentitas.Bitacora.Application.Commands.Vinculos;

/// <summary>
/// Creates a new binding code for a professional to share with a patient.
/// </summary>
public readonly record struct CreateBindingCodeCommand(
    Guid ProfessionalId,
    string TtlPreset,
    Guid TraceId) : ICommand<CreateBindingCodeResponse>;

public sealed record CreateBindingCodeResponse(
    Guid BindingCodeId,
    string Code,
    DateTime ExpiresAt);

public sealed class CreateBindingCodeCommandHandler(
    IBindingCodeRepository bindingCodeRepository,
    IBitacoraUnitOfWork unitOfWork,
    ILogger<CreateBindingCodeCommandHandler> logger)
    : ICommandHandler<CreateBindingCodeCommand, CreateBindingCodeResponse>
{
    private static readonly Dictionary<string, int> TtlMinutes = new()
    {
        ["15min"] = 15,
        ["1h"] = 60,
        ["24h"] = 1440
    };

    public async ValueTask<CreateBindingCodeResponse> Handle(CreateBindingCodeCommand command, CancellationToken cancellationToken)
    {
        if (command.ProfessionalId == Guid.Empty)
        {
            throw new BitacoraException("PROFESSIONAL_ID_REQUIRED", "El identificador del profesional es obligatorio.", 400);
        }

        if (string.IsNullOrWhiteSpace(command.TtlPreset) || !TtlMinutes.TryGetValue(command.TtlPreset, out var minutes))
        {
            throw new BitacoraException("INVALID_TTL_PRESET", $"El preset '{command.TtlPreset}' no es valido. Usa 15min, 1h o 24h.", 400);
        }

        var code = GenerateSecureCode();
        var nowUtc = DateTime.UtcNow;
        var expiresAt = nowUtc.AddMinutes(minutes);

        var bindingCode = BindingCode.Create(code, command.ProfessionalId, command.TtlPreset, expiresAt, nowUtc);

        await bindingCodeRepository.AddAsync(bindingCode, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("BindingCode {BindingCodeId} created for professional {ProfessionalId} with TTL {TtlPreset}",
            bindingCode.BindingCodeId, command.ProfessionalId, command.TtlPreset);

        return new CreateBindingCodeResponse(bindingCode.BindingCodeId, bindingCode.Code, expiresAt);
    }

    private static string GenerateSecureCode()
    {
        var segments = new string[3];
        var random = Random.Shared;
        for (var i = 0; i < 3; i++)
        {
            var value = random.Next(100, 1000);
            segments[i] = value.ToString();
        }
        return string.Join("-", segments);
    }
}
