using Mediator;
using NuestrasCuentitas.Bitacora.Application.Common;
using NuestrasCuentitas.Bitacora.Application.Contracts.Telegram;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Transactions;

namespace NuestrasCuentitas.Bitacora.Application.Commands.Telegram;

/// <summary>
/// Command to unlink a Telegram session from a user (soft delete).
/// </summary>
public sealed record UnlinkTelegramSessionCommand(Guid PatientId) : ICommand<UnlinkTelegramSessionResponse>;

/// <summary>
/// Handler for UnlinkTelegramSessionCommand.
/// </summary>
internal sealed class UnlinkTelegramSessionCommandHandler(
    ITelegramSessionRepository sessionRepository,
    IBitacoraUnitOfWork unitOfWork)
    : ICommandHandler<UnlinkTelegramSessionCommand, UnlinkTelegramSessionResponse>
{
    private readonly ITelegramSessionRepository _repository = sessionRepository;
    private readonly IBitacoraUnitOfWork _unitOfWork = unitOfWork;

    public async ValueTask<UnlinkTelegramSessionResponse> Handle(
        UnlinkTelegramSessionCommand command,
        CancellationToken cancellationToken)
    {
        var session = await _repository.FindLinkedByPatientIdAsync(command.PatientId, cancellationToken);

        if (session is null)
        {
            throw new BitacoraException("TG_SESSION_NOT_FOUND", "No linked Telegram session found for this user.", 404);
        }

        var nowUtc = DateTime.UtcNow;
        session.Unlink(nowUtc);

        await _repository.UpdateAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UnlinkTelegramSessionResponse
        {
            PatientId = session.PatientId,
            UnlinkedAtUtc = nowUtc
        };
    }
}
