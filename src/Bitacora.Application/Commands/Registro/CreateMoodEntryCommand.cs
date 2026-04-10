using System.Text.Json;
using Mediator;
using NuestrasCuentitas.Bitacora.Application.Common;
using NuestrasCuentitas.Bitacora.Application.Interfaces;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Transactions;
using NuestrasCuentitas.Bitacora.Domain.Entities;
using NuestrasCuentitas.Bitacora.Domain.Enums;

namespace NuestrasCuentitas.Bitacora.Application.Commands.Registro;

public readonly record struct CreateMoodEntryCommand(
    Guid PatientId,
    Guid ActorId,
    Guid TraceId,
    int Score,
    string Channel) : ICommand<CreateMoodEntryResponse>;

public sealed record CreateMoodEntryResponse(
    Guid MoodEntryId,
    int MoodScore,
    string Channel,
    DateTime CreatedAtUtc,
    bool IsDuplicate);

public sealed class CreateMoodEntryCommandHandler(
    IUserRepository userRepository,
    IMoodEntryRepository moodEntryRepository,
    IAccessAuditRepository accessAuditRepository,
    IBitacoraUnitOfWork unitOfWork,
    IEncryptionService encryptionService,
    IPseudonymizationService pseudonymizationService)
    : ICommandHandler<CreateMoodEntryCommand, CreateMoodEntryResponse>
{
    public async ValueTask<CreateMoodEntryResponse> Handle(CreateMoodEntryCommand command, CancellationToken cancellationToken)
    {
        if (command.Score is < -3 or > 3)
        {
            throw new BitacoraException("INVALID_SCORE", "El score debe estar entre -3 y +3.", 422);
        }

        var sinceUtc = DateTime.UtcNow.AddMinutes(-1);
        var duplicate = await moodEntryRepository.FindDuplicateAsync(command.PatientId, command.Score, sinceUtc, cancellationToken);
        if (duplicate is not null)
        {
            using var duplicateProjection = JsonDocument.Parse(duplicate.SafeProjection);
            return new CreateMoodEntryResponse(
                duplicate.MoodEntryId,
                duplicateProjection.RootElement.GetProperty("mood_score").GetInt32(),
                duplicateProjection.RootElement.GetProperty("channel").GetString() ?? command.Channel,
                duplicate.CreatedAtUtc,
                true);
        }

        var nowUtc = DateTime.UtcNow;
        var payload = new
        {
            mood_score = command.Score,
            channel = command.Channel.Trim().ToLowerInvariant(),
            created_at = nowUtc
        };

        var safeProjection = JsonSerializer.Serialize(new
        {
            mood_score = command.Score,
            channel = command.Channel.Trim().ToLowerInvariant(),
            created_at = nowUtc
        });

        var entry = MoodEntry.Create(
            command.PatientId,
            encryptionService.EncryptObject(payload),
            safeProjection,
            encryptionService.GetActiveKeyVersion(),
            nowUtc,
            nowUtc);

        var patient = await userRepository.GetByIdAsync(command.PatientId, cancellationToken)
            ?? throw new BitacoraException("PATIENT_NOT_FOUND", "No encontramos el paciente autenticado.", 404);

        patient.MarkActive();
        userRepository.Update(patient);

        await moodEntryRepository.AddAsync(entry, cancellationToken);
        await accessAuditRepository.AddAsync(
            AccessAudit.Create(
                command.TraceId,
                command.ActorId,
                pseudonymizationService.CreatePseudonym(command.ActorId),
                AuditActionType.Create,
                "mood_entry",
                entry.MoodEntryId,
                command.PatientId,
                AuditOutcome.Ok,
                nowUtc),
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateMoodEntryResponse(entry.MoodEntryId, command.Score, command.Channel.Trim().ToLowerInvariant(), nowUtc, false);
    }
}
