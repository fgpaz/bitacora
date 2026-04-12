using Mediator;
using Microsoft.Extensions.Logging;
using NuestrasCuentitas.Bitacora.Application.Common;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Transactions;
using NuestrasCuentitas.Bitacora.Domain.Entities;
using NuestrasCuentitas.Bitacora.Domain.Enums;

namespace NuestrasCuentitas.Bitacora.Application.Commands.Vinculos;

/// <summary>
/// Professional sends a PendingInvite to a patient by email hash.
/// Does NOT create a CareLink — the patient self-binds via binding code after registering.
/// Invariant: PendingInvite never grants clinical access.
/// </summary>
public readonly record struct CreatePendingInviteCommand(
    Guid ProfessionalId,
    string InviteEmailHash,
    Guid ActorId,
    Guid TraceId) : ICommand<CreatePendingInviteResponse>;

public sealed record CreatePendingInviteResponse(
    Guid PendingInviteId,
    string InviteToken,
    DateTime ExpiresAtUtc);

public sealed class CreatePendingInviteCommandHandler(
    IPendingInviteRepository pendingInviteRepository,
    IBitacoraUnitOfWork unitOfWork,
    ILogger<CreatePendingInviteCommandHandler> logger)
    : ICommandHandler<CreatePendingInviteCommand, CreatePendingInviteResponse>
{
    private const int DefaultTtlDays = 7;

    public async ValueTask<CreatePendingInviteResponse> Handle(
        CreatePendingInviteCommand command,
        CancellationToken cancellationToken)
    {
        if (command.ProfessionalId == Guid.Empty)
        {
            throw new BitacoraException("PROFESSIONAL_ID_REQUIRED",
                "El identificador del profesional es obligatorio.", 400);
        }

        if (string.IsNullOrWhiteSpace(command.InviteEmailHash))
        {
            throw new BitacoraException("INVITE_EMAIL_HASH_REQUIRED",
                "El hash de email del invitado es obligatorio.", 400);
        }

        if (command.InviteEmailHash.Length != 64) // SHA256 hex = 64 chars
        {
            throw new BitacoraException("INVALID_EMAIL_HASH_FORMAT",
                "El hash de email debe ser un SHA256 valido (64 caracteres hex).", 400);
        }

        var nowUtc = DateTime.UtcNow;
        var inviteToken = GenerateSecureToken();
        var expiresAt = nowUtc.AddDays(DefaultTtlDays);

        var pendingInvite = PendingInvite.Create(
            command.ProfessionalId,
            command.InviteEmailHash,
            inviteToken,
            expiresAt,
            nowUtc);

        await pendingInviteRepository.AddAsync(pendingInvite, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "PendingInvite {PendingInviteId} created by professional {ProfessionalId}, expires at {ExpiresAt}",
            pendingInvite.PendingInviteId, command.ProfessionalId, expiresAt);

        return new CreatePendingInviteResponse(
            pendingInvite.PendingInviteId,
            inviteToken,
            expiresAt);
    }

    private static string GenerateSecureToken()
    {
        var bytes = new byte[32];
        Random.Shared.NextBytes(bytes);
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}