using NuestrasCuentitas.Bitacora.Domain.Entities;

namespace NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;

public interface IBindingCodeRepository
{
    Task<BindingCode?> FindByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<BindingCode?> FindActiveByProfessionalIdAsync(Guid professionalId, CancellationToken cancellationToken = default);
    Task AddAsync(BindingCode bindingCode, CancellationToken cancellationToken = default);
    Task UpdateAsync(BindingCode bindingCode, CancellationToken cancellationToken = default);
}
