namespace Shared.Contract.Export;

/// <summary>
/// Structured export of a patient's mood and check-in data for a date range.
/// Privacy-aware: only safe projections (no encrypted payloads).
/// </summary>
public sealed record PatientExportDto(
    Guid PatientId,
    DateOnly From,
    DateOnly To,
    DateTime ExportedAtUtc,
    PatientExportSummaryDto Summary,
    IReadOnlyList<PatientExportEntryDto> Entries);

public sealed record PatientExportSummaryDto(
    int TotalDays,
    int DaysWithMoodEntry,
    int DaysWithCheckin,
    decimal? AverageMoodScore,
    decimal? AverageSleepHours,
    int? AnxietyDays,
    int? IrritabilityDays,
    int? MedicationTakenDays);

public sealed record PatientExportEntryDto(
    DateOnly Date,
    MoodExportDto? Mood,
    CheckinExportDto? Checkin);

public sealed record MoodExportDto(
    int? Score,
    DateTime RecordedAtUtc);

public sealed record CheckinExportDto(
    decimal? SleepHours,
    bool? PhysicalActivity,
    bool? SocialActivity,
    bool? Anxiety,
    bool? Irritability,
    bool? MedicationTaken,
    TimeOnly? MedicationTime);