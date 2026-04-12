using NuestrasCuentitas.Bitacora.Domain.Enums;

namespace NuestrasCuentitas.Bitacora.Domain.Entities;

/// <summary>
/// Reminder schedule configuration per patient for Telegram reminders (RF-TG-010..012).
/// Persisted to support multi-instance hosted service scheduling.
/// </summary>
public sealed class ReminderConfig
{
    public Guid ReminderConfigId { get; private set; }
    public Guid PatientId { get; private set; }
    public int HourUtc { get; private set; }
    public int MinuteUtc { get; private set; }
    public bool Enabled { get; private set; }
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
        DateTime? nextFireAtUtc,
        DateTime createdAtUtc,
        DateTime? disabledAtUtc)
    {
        ReminderConfigId = reminderConfigId;
        PatientId = patientId;
        HourUtc = hourUtc;
        MinuteUtc = minuteUtc;
        Enabled = enabled;
        NextFireAtUtc = nextFireAtUtc;
        CreatedAtUtc = createdAtUtc;
        DisabledAtUtc = disabledAtUtc;
    }

    public static ReminderConfig CreateDefault(Guid patientId, int defaultHourUtc, int defaultMinuteUtc)
    {
        if (patientId == Guid.Empty)
        {
            throw new ArgumentException("Patient id is required.", nameof(patientId));
        }

        var nowUtc = DateTime.UtcNow;
        var nextFire = CalculateNextFireUtc(nowUtc, defaultHourUtc, defaultMinuteUtc);

        return new ReminderConfig(
            Guid.NewGuid(),
            patientId,
            defaultHourUtc,
            defaultMinuteUtc,
            enabled: true,
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

    public void Reschedule(int hourUtc, int minuteUtc, DateTime nowUtc)
    {
        if (!Enabled)
        {
            throw new InvalidOperationException("Cannot reschedule a disabled reminder config.");
        }

        HourUtc = hourUtc;
        MinuteUtc = minuteUtc;
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
        var today = new DateTime(nowUtc.Year, nowUtc.Month, nowUtc.Day, hourUtc, minuteUtc, 0, DateTimeKind.Utc);
        return today > nowUtc ? today : today.AddDays(1);
    }
}