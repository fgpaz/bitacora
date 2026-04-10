using System.Text.Json;
using Mediator;
using NuestrasCuentitas.Bitacora.Application.Common;
using NuestrasCuentitas.Bitacora.Application.Interfaces;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Transactions;
using NuestrasCuentitas.Bitacora.Domain.Entities;
using NuestrasCuentitas.Bitacora.Domain.Enums;

namespace NuestrasCuentitas.Bitacora.Application.Commands.Registro;

public readonly record struct UpsertDailyCheckinCommand(
    Guid PatientId,
    Guid ActorId,
    Guid TraceId,
    decimal SleepHours,
    bool PhysicalActivity,
    bool SocialActivity,
    bool Anxiety,
    bool Irritability,
    bool MedicationTaken,
    TimeOnly? MedicationTime) : ICommand<UpsertDailyCheckinResponse>;

public sealed record UpsertDailyCheckinResponse(
    Guid DailyCheckinId,
    DateOnly CheckinDate,
    decimal SleepHours,
    bool HasPhysical,
    bool HasSocial,
    bool HasAnxiety,
    bool HasIrritability,
    bool HasMedication,
    bool UpdatedExisting);

public sealed class UpsertDailyCheckinCommandHandler(
    IDailyCheckinRepository dailyCheckinRepository,
    IAccessAuditRepository accessAuditRepository,
    IBitacoraUnitOfWork unitOfWork,
    IEncryptionService encryptionService,
    IPseudonymizationService pseudonymizationService)
    : ICommandHandler<UpsertDailyCheckinCommand, UpsertDailyCheckinResponse>
{
    public async ValueTask<UpsertDailyCheckinResponse> Handle(
        UpsertDailyCheckinCommand command,
        CancellationToken cancellationToken)
    {
        if (command.SleepHours is < 0 or > 24)
        {
            throw new BitacoraException("VALIDATION_ERROR", "Las horas de sueño deben estar entre 0 y 24.", 422);
        }

        if (command.MedicationTaken && command.MedicationTime is null)
        {
            throw new BitacoraException("VALIDATION_ERROR", "Si hubo medicación, hace falta informar un horario aproximado.", 422);
        }

        var normalizedMedicationTime = command.MedicationTaken && command.MedicationTime is not null
            ? NormalizeToQuarterHour(command.MedicationTime.Value)
            : (TimeOnly?)null;

        var nowUtc = DateTime.UtcNow;
        var checkinDate = DateOnly.FromDateTime(nowUtc);
        var payload = new
        {
            sleep_hours = command.SleepHours,
            physical_activity = command.PhysicalActivity,
            social_activity = command.SocialActivity,
            anxiety = command.Anxiety,
            irritability = command.Irritability,
            medication_taken = command.MedicationTaken,
            medication_time = normalizedMedicationTime?.ToString("HH:mm"),
            checkin_date = checkinDate
        };

        var safeProjection = JsonSerializer.Serialize(new
        {
            sleep_hours = command.SleepHours,
            has_physical = command.PhysicalActivity,
            has_social = command.SocialActivity,
            has_anxiety = command.Anxiety,
            has_irritability = command.Irritability,
            has_medication = command.MedicationTaken
        });

        var existing = await dailyCheckinRepository.GetByPatientAndDateAsync(command.PatientId, checkinDate, cancellationToken);
        var encryptedPayload = encryptionService.EncryptObject(payload);
        var keyVersion = encryptionService.GetActiveKeyVersion();

        if (existing is null)
        {
            existing = DailyCheckin.Create(command.PatientId, checkinDate, encryptedPayload, safeProjection, keyVersion, nowUtc, nowUtc);
            await dailyCheckinRepository.AddAsync(existing, cancellationToken);
        }
        else
        {
            existing.UpdatePayload(encryptedPayload, safeProjection, keyVersion, nowUtc, nowUtc);
            dailyCheckinRepository.Update(existing);
        }

        await accessAuditRepository.AddAsync(
            AccessAudit.Create(
                command.TraceId,
                command.ActorId,
                pseudonymizationService.CreatePseudonym(command.ActorId),
                existing.CreatedAtUtc == nowUtc ? AuditActionType.Create : AuditActionType.Update,
                "daily_checkin",
                existing.DailyCheckinId,
                command.PatientId,
                AuditOutcome.Ok,
                nowUtc),
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpsertDailyCheckinResponse(
            existing.DailyCheckinId,
            checkinDate,
            command.SleepHours,
            command.PhysicalActivity,
            command.SocialActivity,
            command.Anxiety,
            command.Irritability,
            command.MedicationTaken,
            existing.CreatedAtUtc != nowUtc);
    }

    private static TimeOnly NormalizeToQuarterHour(TimeOnly value)
    {
        var totalMinutes = (int)Math.Round(value.Minute / 15d, MidpointRounding.AwayFromZero) * 15;
        if (totalMinutes == 60)
        {
            return new TimeOnly((value.Hour + 1) % 24, 0);
        }

        return new TimeOnly(value.Hour, totalMinutes);
    }
}
