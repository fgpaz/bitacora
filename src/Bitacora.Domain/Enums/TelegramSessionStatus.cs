namespace NuestrasCuentitas.Bitacora.Domain.Enums;

/// <summary>
/// Status of a Telegram session linking a patient to a Telegram chat.
/// </summary>
public enum TelegramSessionStatus
{
    /// <summary>The chat is linked to a patient and can receive reminders.</summary>
    Linked = 1,

    /// <summary>The chat was unlinked and no longer receives messages.</summary>
    Unlinked = 2
}
