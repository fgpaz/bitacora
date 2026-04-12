namespace Shared.Contract.Visualizacion;

/// <summary>
/// A single mood entry as returned by the timeline.
/// </summary>
public sealed record MoodEntryDto(
    Guid MoodEntryId,
    int? MoodScore,
    DateTime CreatedAtUtc);

/// <summary>
/// A single daily check-in as returned by the timeline.
/// </summary>
public sealed record DailyCheckinDto(
    Guid DailyCheckinId,
    DateOnly CheckinDate,
    decimal? SleepHours,
    bool? PhysicalActivity,
    bool? SocialActivity,
    bool? Anxiety,
    bool? Irritability,
    bool? MedicationTaken);

/// <summary>
/// One day of combined timeline data.
/// </summary>
public sealed record TimelineDayDto(
    DateOnly Date,
    MoodEntryDto? MoodEntry,
    DailyCheckinDto? DailyCheckin);