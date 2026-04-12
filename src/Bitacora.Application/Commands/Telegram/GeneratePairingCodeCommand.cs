using Mediator;
using Microsoft.Extensions.Logging;
using NuestrasCuentitas.Bitacora.Application.Common;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Transactions;
using NuestrasCuentitas.Bitacora.Domain.Entities;

namespace NuestrasCuentitas.Bitacora.Application.Commands.Telegram;

/// <summary>
/// Generates a new Telegram pairing code for the authenticated patient (RF-TG-001).
/// Requires active consent. Invalidates any existing active code for the patient.
/// Format: BIT-XXXXX (5 alphanumeric chars). TTL: 15 minutes.
/// </summary>
public readonly record struct GeneratePairingCodeCommand(
    Guid PatientId,
    Guid TraceId) : ICommand<GeneratePairingCodeResponse>;

public sealed record GeneratePairingCodeResponse(
    string Code,
    int ExpiresInSeconds,
    DateTime ExpiresAt);

public sealed class GeneratePairingCodeCommandHandler(
    ITelegramPairingCodeRepository pairingCodeRepository,
    IConsentGrantRepository consentGrantRepository,
    IBitacoraUnitOfWork unitOfWork,
    ILogger<GeneratePairingCodeCommandHandler> logger)
    : ICommandHandler<GeneratePairingCodeCommand, GeneratePairingCodeResponse>
{
    private const int TtlSeconds = 900; // 15 minutes

    public async ValueTask<GeneratePairingCodeResponse> Handle(GeneratePairingCodeCommand command, CancellationToken cancellationToken)
    {
        if (command.PatientId == Guid.Empty)
        {
            throw new BitacoraException("PATIENT_ID_REQUIRED", "El identificador del paciente es obligatorio.", 400);
        }

        // Consent gate (RF-TG-001)
        var consent = await consentGrantRepository.GetActiveByPatientAsync(command.PatientId, cancellationToken);
        if (consent == null)
        {
            throw new BitacoraException("TG_001_CONSENT_REQUIRED", "Consentimiento no vigente.", 403);
        }

        var nowUtc = DateTime.UtcNow;

        // Invalidate any existing active code for this patient
        var existingCode = await pairingCodeRepository.FindActiveByPatientIdAsync(command.PatientId, cancellationToken);
        if (existingCode != null)
        {
            // Mark as used to invalidate (it's already expired conceptually, but we mark it used)
            existingCode.MarkAsUsed(nowUtc.AddSeconds(-1)); // backdate to ensure it's invalid
            await pairingCodeRepository.UpdateAsync(existingCode, cancellationToken);
        }

        // Generate unique code with collision retry (max 5 attempts)
        string code;
        for (var attempt = 0; attempt < 5; attempt++)
        {
            code = GenerateSecureCode();
            var existing = await pairingCodeRepository.FindValidByCodeAsync(code, cancellationToken);
            if (existing == null)
            {
                var pairingCode = TelegramPairingCode.Create(code, command.PatientId, nowUtc);
                await pairingCodeRepository.AddAsync(pairingCode, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                logger.LogInformation("TelegramPairingCode {PairingCodeId} generated for patient {PatientId}, expires at {ExpiresAt}",
                    pairingCode.TelegramPairingCodeId, command.PatientId, pairingCode.ExpiresAt);

                return new GeneratePairingCodeResponse(
                    pairingCode.Code,
                    TtlSeconds,
                    pairingCode.ExpiresAt);
            }
        }

        throw new BitacoraException("PAIRING_CODE_GENERATION_FAILED", "No se pudo generar un codigo unico tras varios intentos.", 500);
    }

    private static string GenerateSecureCode()
    {
        // Format: BIT-XXXXX (5 alphanumeric chars)
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // no I, O, 0, 1 to avoid confusion
        var segments = new char[5];
        for (var i = 0; i < 5; i++)
        {
            segments[i] = chars[Random.Shared.Next(chars.Length)];
        }
        return $"BIT-{new string(segments)}";
    }
}
