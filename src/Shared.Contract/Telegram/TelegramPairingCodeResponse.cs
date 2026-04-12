namespace Shared.Contract.Telegram;

/// <summary>
/// Response DTO for the pairing code generation endpoint (POST /api/v1/telegram/pairing).
/// </summary>
public sealed record TelegramPairingCodeResponse(
    string Code,
    int ExpiresIn,
    DateTime ExpiresAt);
