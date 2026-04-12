using NuestrasCuentitas.Bitacora.Domain.Enums;

namespace NuestrasCuentitas.Bitacora.Domain.Entities;

/// <summary>
/// Represents a linked Telegram session between a patient and a Telegram chat_id (RF-TG-001..003).
/// One chat_id can only be linked to one patient at a time.
/// </summary>
public sealed class TelegramSession
{
    public Guid TelegramSessionId { get; private set; }
    public Guid PatientId { get; private set; }
    public string ChatId { get; private set; } = string.Empty;
    public TelegramSessionStatus Status { get; private set; }
    public DateTime LinkedAtUtc { get; private set; }
    public DateTime? UnlinkedAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private TelegramSession()
    {
    }

    private TelegramSession(
        Guid telegramSessionId,
        Guid patientId,
        string chatId,
        TelegramSessionStatus status,
        DateTime linkedAtUtc,
        DateTime? unlinkedAtUtc,
        DateTime createdAtUtc)
    {
        TelegramSessionId = telegramSessionId;
        PatientId = patientId;
        ChatId = chatId;
        Status = status;
        LinkedAtUtc = linkedAtUtc;
        UnlinkedAtUtc = unlinkedAtUtc;
        CreatedAtUtc = createdAtUtc;
    }

    /// <summary>
    /// Creates a new linked Telegram session (RF-TG-002).
    /// </summary>
    public static TelegramSession CreateLinked(
        Guid patientId,
        string chatId,
        DateTime nowUtc)
    {
        if (patientId == Guid.Empty)
        {
            throw new ArgumentException("Patient id is required.", nameof(patientId));
        }

        if (string.IsNullOrWhiteSpace(chatId))
        {
            throw new ArgumentException("Chat id is required.", nameof(chatId));
        }

        return new TelegramSession(
            Guid.NewGuid(),
            patientId,
            chatId.Trim(),
            TelegramSessionStatus.Linked,
            linkedAtUtc: nowUtc,
            unlinkedAtUtc: null,
            createdAtUtc: nowUtc);
    }

    /// <summary>
    /// Unlinks the session (RF-TG-003). Immediately stops reminders per CT-TELEGRAM-RUNTIME invariant.
    /// </summary>
    public void Unlink(DateTime unlinkedAtUtc)
    {
        if (Status != TelegramSessionStatus.Linked)
        {
            throw new InvalidOperationException("Can only unlink a linked session.");
        }

        Status = TelegramSessionStatus.Unlinked;
        UnlinkedAtUtc = unlinkedAtUtc;
    }
}
