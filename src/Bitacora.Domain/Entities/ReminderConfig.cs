namespace NuestrasCuentitas.Bitacora.Domain.Entities;

/// <summary>
/// Reminder schedule configuration per patient for Telegram reminders (RF-TG-010..012).
/// Persisted to support multi-instance hosted service scheduling.
/// </summary>
public sealed class ReminderConfig
{
    public const string DefaultTimezone = "America/Argentina/Buenos_Aires";

    public Guid ReminderConfigId { get; private set; }
    public Guid PatientId { get; private set; }
    public int HourUtc { get; private set; }
    public int MinuteUtc { get; private set; }
    public bool Enabled { get; private set; }
    public string ReminderTimezone { get; private set; } = DefaultTimezone;
    public DateTime? NextFireAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? DisabledAtUtc { get; private set; }

    private ReminderConfig()
    {
    }

    private ReminderConfig(
        Guid reminderConfigId,
        Guid patientId,
        int hourUtc,
        int minuteUtc,
        bool enabled,
        string? reminderTimezone,
        DateTime? nextFireAtUtc,
        DateTime createdAtUtc,
        DateTime? disabledAtUtc)
    {
        ReminderConfigId = reminderConfigId;
        PatientId = patientId;
        HourUtc = hourUtc;
        MinuteUtc = minuteUtc;
        Enabled = enabled;
        ReminderTimezone = NormalizeTimezone(reminderTimezone);
        NextFireAtUtc = nextFireAtUtc;
        CreatedAtUtc = createdAtUtc;
        DisabledAtUtc = disabledAtUtc;
    }

    public static ReminderConfig CreateDefault(Guid patientId, int defaultHourUtc, int defaultMinuteUtc, string? timezone = null)
    {
        if (patientId == Guid.Empty)
        {
            throw new ArgumentException("Patient id is required.", nameof(patientId));
        }

        ValidateSchedule(defaultHourUtc, defaultMinuteUtc);

        var nowUtc = DateTime.UtcNow;
        var nextFire = CalculateNextFireUtc(nowUtc, defaultHourUtc, defaultMinuteUtc);

        return new ReminderConfig(
            Guid.NewGuid(),
            patientId,
            defaultHourUtc,
            defaultMinuteUtc,
            enabled: true,
            reminderTimezone: timezone,
            nextFireAtUtc: nextFire,
            createdAtUtc: nowUtc,
            disabledAtUtc: null);
    }

    public void Disable(DateTime nowUtc)
    {
        if (!Enabled)
        {
            return;
        }

        Enabled = false;
        DisabledAtUtc = nowUtc;
        NextFireAtUtc = null;
    }

    public void Reschedule(int hourUtc, int minuteUtc, string? timezone, DateTime nowUtc)
    {
        ConfigureSchedule(hourUtc, minuteUtc, timezone, nowUtc);
    }

    public void ConfigureSchedule(int hourUtc, int minuteUtc, string? timezone, DateTime nowUtc)
    {
        ValidateSchedule(hourUtc, minuteUtc);

        HourUtc = hourUtc;
        MinuteUtc = minuteUtc;
        Enabled = true;
        DisabledAtUtc = null;
        ReminderTimezone = NormalizeTimezone(timezone);
        NextFireAtUtc = CalculateNextFireUtc(nowUtc, hourUtc, minuteUtc);
    }

    public void AdvanceNextFire(DateTime nowUtc)
    {
        if (!Enabled || NextFireAtUtc == null)
        {
            return;
        }

        NextFireAtUtc = CalculateNextFireUtc(nowUtc, HourUtc, MinuteUtc);
    }

    private static DateTime CalculateNextFireUtc(DateTime nowUtc, int hourUtc, int minuteUtc)
    {
        ValidateSchedule(hourUtc, minuteUtc);

        var today = new DateTime(nowUtc.Year, nowUtc.Month, nowUtc.Day, hourUtc, minuteUtc, 0, DateTimeKind.Utc);
        return today > nowUtc ? today : today.AddDays(1);
    }

    private static void ValidateSchedule(int hourUtc, int minuteUtc)
    {
        if (hourUtc is < 0 or > 23)
        {
            throw new ArgumentOutOfRangeException(nameof(hourUtc), "Reminder hour must be between 0 and 23.");
        }

        if (minuteUtc is not (0 or 30))
        {
            throw new ArgumentOutOfRangeException(nameof(minuteUtc), "Reminder minute must be 0 or 30.");
        }
    }

    private static string NormalizeTimezone(string? timezone)
    {
        return string.IsNullOrWhiteSpace(timezone)
            ? DefaultTimezone
            : timezone.Trim();
    }
}
