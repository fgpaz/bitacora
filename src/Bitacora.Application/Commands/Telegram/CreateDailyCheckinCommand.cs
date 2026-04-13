using System.Text.Json;
using Mediator;
using Microsoft.Extensions.Logging;
using NuestrasCuentitas.Bitacora.Application.Common;
using NuestrasCuentitas.Bitacora.Application.Interfaces;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Transactions;
using NuestrasCuentitas.Bitacora.Domain.Entities;
using NuestrasCuentitas.Bitacora.Domain.Enums;

namespace NuestrasCuentitas.Bitacora.Application.Commands.Telegram;

/// <summary>
/// Creates or updates a DailyCheckin from the Telegram sequential factors flow (RF-REG-013).
/// Factors are collected incrementally via Telegram conversation and assembled here.
/// </summary>
public readonly record struct CreateDailyCheckinCommand(
    Guid PatientId,
    int MoodScore,
    decimal SleepHours,
    bool PhysicalActivity,
    bool SocialActivity,
    bool Anxiety,
    bool Irritability,
    bool MedicationTaken,
    TimeOnly? MedicationTime,
    Guid TraceId,
    string Channel) : ICommand<CreateDailyCheckinResponse>;

public sealed record CreateDailyCheckinResponse(
    Guid DailyCheckinId,
    DateOnly CheckinDate,
    bool Created,
    string Summary);

public sealed class CreateDailyCheckinCommandHandler(
    IDailyCheckinRepository dailyCheckinRepository,
    IAccessAuditRepository accessAuditRepository,
    IEncryptionService encryptionService,
    IPseudonymizationService pseudonymizationService,
    IBitacoraUnitOfWork unitOfWork,
    ILogger<CreateDailyCheckinCommandHandler> logger)
    : ICommandHandler<CreateDailyCheckinCommand, CreateDailyCheckinResponse>
{
    public async ValueTask<CreateDailyCheckinResponse> Handle(
        CreateDailyCheckinCommand command,
        CancellationToken cancellationToken)
    {
        var nowUtc = DateTime.UtcNow;
        var checkinDate = DateOnly.FromDateTime(nowUtc);

        // Build safe summary (no clinical details — used in Telegram reply)
        var summary = BuildSummary(command);

        // Payload for encryption (includes clinical data)
        var payload = new
        {
            mood_score = command.MoodScore,
            sleep_hours = command.SleepHours,
            physical_activity = command.PhysicalActivity,
            social_activity = command.SocialActivity,
            anxiety = command.Anxiety,
            irritability = command.Irritability,
            medication_taken = command.MedicationTaken,
            medication_time = command.MedicationTime?.ToString("HH:mm"),
            checkin_date = checkinDate.ToString("yyyy-MM-dd"),
            channel = command.Channel,
            created_at = nowUtc
        };

        var safeProjection = JsonSerializer.Serialize(new
        {
            mood_score = command.MoodScore,
            sleep_hours = command.SleepHours,
            has_physical = command.PhysicalActivity,
            has_social = command.SocialActivity,
            has_anxiety = command.Anxiety,
            has_irritability = command.Irritability,
            has_medication = command.MedicationTaken,
            checkin_date = checkinDate.ToString("yyyy-MM-dd")
        });

        var encryptedPayload = encryptionService.EncryptObject(payload);
        var keyVersion = encryptionService.GetActiveKeyVersion();

        var existing = await dailyCheckinRepository.GetByPatientAndDateAsync(
            command.PatientId, checkinDate, cancellationToken);

        bool created;
        if (existing is null)
        {
            existing = DailyCheckin.Create(
                command.PatientId,
                checkinDate,
                encryptedPayload,
                safeProjection,
                keyVersion,
                nowUtc,
                nowUtc);
            await dailyCheckinRepository.AddAsync(existing, cancellationToken);
            created = true;
        }
        else
        {
            existing.UpdatePayload(encryptedPayload, safeProjection, keyVersion, nowUtc, nowUtc);
            dailyCheckinRepository.Update(existing);
            created = false;
        }

        await accessAuditRepository.AddAsync(
            AccessAudit.Create(
                command.TraceId,
                command.PatientId,
                pseudonymizationService.CreatePseudonym(command.PatientId),
                created ? AuditActionType.Create : AuditActionType.Update,
                "daily_checkin_telegram",
                existing.DailyCheckinId,
                command.PatientId,
                AuditOutcome.Ok,
                nowUtc),
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Telegram DailyCheckin {DailyCheckinId} {Action} for patient {PatientId}, date {CheckinDate}, trace {TraceId}",
            existing.DailyCheckinId,
            created ? "created" : "updated",
            command.PatientId,
            checkinDate,
            command.TraceId);

        return new CreateDailyCheckinResponse(
            existing.DailyCheckinId,
            checkinDate,
            created,
            summary);
    }

    private static string BuildSummary(CreateDailyCheckinCommand command)
    {
        var parts = new List<string>();

        var moodEmoji = command.MoodScore switch
        {
            3 => "+3 :)",
            2 => "+2 :)",
            1 => "+1 :)",
            0 => "0",
            -1 => "-1 :(",
            -2 => "-2 :(",
            -3 => "-3 :(",
            _ => command.MoodScore.ToString()
        };
        parts.Add("Humor: " + moodEmoji);
        parts.Add("Sueno: " + command.SleepHours + "h");
        parts.Add("Actividad fisica: " + (command.PhysicalActivity ? "si" : "no"));
        parts.Add("Actividad social: " + (command.SocialActivity ? "si" : "no"));
        parts.Add("Ansiedad: " + (command.Anxiety ? "si" : "no"));
        parts.Add("Irritabilidad: " + (command.Irritability ? "si" : "no"));
        parts.Add("Medicacion: " + (command.MedicationTaken
            ? "si (" + command.MedicationTime?.ToString("HH:mm") + ")"
            : "no"));

        return string.Join(" | ", parts);
    }
}
