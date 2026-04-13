using NuestrasCuentitas.Bitacora.Domain.Enums;

namespace NuestrasCuentitas.Bitacora.Domain.Entities;

/// <summary>
/// Represents a linked Telegram session between a patient and a Telegram chat_id (RF-TG-001..003).
/// One chat_id can only be linked to one patient at a time.
/// Supports sequential mood + factors flow via ConversationState (RF-REG-013).
/// </summary>
public sealed class TelegramSession
{
    public Guid TelegramSessionId { get; private set; }
    public Guid PatientId { get; private set; }
    public string ChatId { get; private set; } = string.Empty;
    public TelegramSessionStatus Status { get; private set; }
    public TelegramConversationState ConversationState { get; private set; }
    public int? PendingMoodScore { get; private set; }
    public string? PendingFactorsJson { get; private set; }
    public DateTime LinkedAtUtc { get; private set; }
    public DateTime? UnlinkedAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    private TelegramSession()
    {
    }

    private TelegramSession(
        Guid telegramSessionId,
        Guid patientId,
        string chatId,
        TelegramSessionStatus status,
        TelegramConversationState conversationState,
        int? pendingMoodScore,
        string? pendingFactorsJson,
        DateTime linkedAtUtc,
        DateTime? unlinkedAtUtc,
        DateTime createdAtUtc,
        DateTime? updatedAtUtc)
    {
        TelegramSessionId = telegramSessionId;
        PatientId = patientId;
        ChatId = chatId;
        Status = status;
        ConversationState = conversationState;
        PendingMoodScore = pendingMoodScore;
        PendingFactorsJson = pendingFactorsJson;
        LinkedAtUtc = linkedAtUtc;
        UnlinkedAtUtc = unlinkedAtUtc;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
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
            TelegramConversationState.Idle,
            pendingMoodScore: null,
            pendingFactorsJson: null,
            linkedAtUtc: nowUtc,
            unlinkedAtUtc: null,
            createdAtUtc: nowUtc,
            updatedAtUtc: null);
    }

    /// <summary>
    /// Advances the session to the awaiting-factors state after a mood score is recorded (RF-REG-013).
    /// </summary>
    public void AdvanceToAwaitingFactors(int moodScore, string factorsJson, DateTime nowUtc)
    {
        if (Status != TelegramSessionStatus.Linked)
        {
            throw new InvalidOperationException("Can only advance a linked session.");
        }

        ConversationState = TelegramConversationState.AwaitingFactorSleep;
        PendingMoodScore = moodScore;
        PendingFactorsJson = factorsJson;
        UpdatedAtUtc = nowUtc;
    }

    /// <summary>
    /// Advances the conversation state to the next factor after one is collected (RF-REG-013).
    /// </summary>
    public void AdvanceToNextFactor(string updatedFactorsJson, DateTime nowUtc)
    {
        ConversationState = GetNextConversationState(ConversationState);
        PendingFactorsJson = updatedFactorsJson;
        UpdatedAtUtc = nowUtc;
    }

    /// <summary>
    /// Resets to idle after a full check-in is complete or when patient cancels (RF-REG-013).
    /// </summary>
    public void ResetToIdle(DateTime nowUtc)
    {
        ConversationState = TelegramConversationState.Idle;
        PendingMoodScore = null;
        PendingFactorsJson = null;
        UpdatedAtUtc = nowUtc;
    }

    private static TelegramConversationState GetNextConversationState(TelegramConversationState current)
    {
        return current switch
        {
            TelegramConversationState.AwaitingFactorSleep => TelegramConversationState.AwaitingFactorPhysical,
            TelegramConversationState.AwaitingFactorPhysical => TelegramConversationState.AwaitingFactorSocial,
            TelegramConversationState.AwaitingFactorSocial => TelegramConversationState.AwaitingFactorAnxiety,
            TelegramConversationState.AwaitingFactorAnxiety => TelegramConversationState.AwaitingFactorIrritability,
            TelegramConversationState.AwaitingFactorIrritability => TelegramConversationState.AwaitingFactorMedication,
            TelegramConversationState.AwaitingFactorMedication => TelegramConversationState.AwaitingFactorMedicationTime,
            TelegramConversationState.AwaitingFactorMedicationTime => TelegramConversationState.Idle,
            _ => TelegramConversationState.Idle
        };
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
