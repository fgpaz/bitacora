using Mediator;
using Microsoft.Extensions.Logging;
using NuestrasCuentitas.Bitacora.Application.Common;
using NuestrasCuentitas.Bitacora.Application.Interfaces;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Transactions;
using NuestrasCuentitas.Bitacora.Domain.Entities;

namespace NuestrasCuentitas.Bitacora.Application.Commands.Auth;

public readonly record struct BootstrapPatientCommand(
    string SupabaseUserId,
    string Email,
    string? InviteToken,
    Guid TraceId) : ICommand<BootstrapPatientResponse>;

public sealed record BootstrapPatientResponse(
    Guid UserId,
    string Status,
    bool NeedsConsent,
    bool ResumePendingInvite);

public sealed class BootstrapPatientCommandHandler(
    IUserRepository userRepository,
    IPendingInviteRepository pendingInviteRepository,
    IBitacoraUnitOfWork unitOfWork,
    IEncryptionService encryptionService,
    ILogger<BootstrapPatientCommandHandler> logger)
    : ICommandHandler<BootstrapPatientCommand, BootstrapPatientResponse>
{
    public async ValueTask<BootstrapPatientResponse> Handle(
        BootstrapPatientCommand command,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.SupabaseUserId) || string.IsNullOrWhiteSpace(command.Email))
        {
            throw new BitacoraException("ONB_001_JWT_INVALID", "El token no contiene los claims requeridos.", 401);
        }

        var normalizedEmail = command.Email.Trim().ToLowerInvariant();
        var current = await userRepository.GetBySupabaseUserIdAsync(command.SupabaseUserId, cancellationToken);

        if (current is null)
        {
            try
            {
                var encryptedEmail = encryptionService.EncryptString(normalizedEmail);
                var emailHash = encryptionService.ComputeSha256(normalizedEmail);
                current = User.CreatePatient(
                    command.SupabaseUserId,
                    encryptedEmail,
                    emailHash,
                    encryptionService.GetActiveKeyVersion(),
                    DateTime.UtcNow);

                await userRepository.AddAsync(current, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex) when (ex is not BitacoraException)
            {
                logger.LogError(ex, "Bootstrap failed while encrypting patient email.");
                throw new BitacoraException("ONB_001_ENCRYPT_FAILED", "No pudimos preparar el alta segura del usuario.", 500);
            }
        }

        var resumePendingInvite = false;
        if (!string.IsNullOrWhiteSpace(command.InviteToken))
        {
            var pendingInvite = await pendingInviteRepository.FindResumableByTokenAndEmailHashAsync(
                command.InviteToken,
                encryptionService.ComputeSha256(normalizedEmail),
                DateTime.UtcNow,
                cancellationToken);

            resumePendingInvite = pendingInvite is not null;
        }

        return new BootstrapPatientResponse(
            current.UserId,
            current.Status.ToString().ToLowerInvariant(),
            current.Status is not Domain.Enums.UserStatus.Active and not Domain.Enums.UserStatus.ConsentGranted,
            resumePendingInvite);
    }
}
