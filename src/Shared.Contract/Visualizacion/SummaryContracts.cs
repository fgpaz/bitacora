namespace Shared.Contract.Visualizacion;

/// <summary>
/// Aggregated summary of a patient's mood and check-in data over a date range.
/// </summary>
public sealed record PatientSummaryDto(
    Guid PatientId,
    DateOnly From,
    DateOnly To,
    int TotalDays,
    int DaysWithMoodEntry,
    int DaysWithCheckin,
    decimal? AverageMoodScore,
    decimal? AverageSleepHours,
    int? AnxietyDays,
    int? IrritabilityDays,
    int? MedicationTakenDays);