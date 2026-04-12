using Mediator;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;

namespace NuestrasCuentitas.Bitacora.Application.Queries.Telegram;

/// <summary>
/// Returns the Telegram session for the authenticated patient (RF-TG-001..003).
/// </summary>
public readonly record struct GetTelegramSessionQuery(
    Guid PatientId,
    Guid TraceId) : IQuery<GetTelegramSessionResponse>;

public sealed record GetTelegramSessionResponse(
    bool IsLinked,
    Guid? SessionId,
    string? ChatId,
    string? LinkedAtUtc);

public sealed class GetTelegramSessionQueryHandler(
    ITelegramSessionRepository sessionRepository)
    : IQueryHandler<GetTelegramSessionQuery, GetTelegramSessionResponse>
{
    public async ValueTask<GetTelegramSessionResponse> Handle(GetTelegramSessionQuery query, CancellationToken cancellationToken)
    {
        var session = await sessionRepository.FindLinkedByPatientIdAsync(query.PatientId, cancellationToken);

        if (session == null)
        {
            return new GetTelegramSessionResponse(
                IsLinked: false,
                SessionId: null,
                ChatId: null,
                LinkedAtUtc: null);
        }

        return new GetTelegramSessionResponse(
            IsLinked: true,
            SessionId: session.TelegramSessionId,
            ChatId: session.ChatId,
            LinkedAtUtc: session.LinkedAtUtc.ToString("O"));
    }
}
