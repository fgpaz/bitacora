namespace Shared.Contract.Telegram;

/// <summary>
/// Response DTO for the Telegram session query endpoint.
/// </summary>
public sealed record TelegramSessionResponse(
    bool IsLinked,
    Guid? SessionId,
    string? ChatId,
    string? LinkedAtUtc);
