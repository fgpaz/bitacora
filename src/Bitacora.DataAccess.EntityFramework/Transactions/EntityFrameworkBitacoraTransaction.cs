using Microsoft.EntityFrameworkCore.Storage;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Transactions;

namespace NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Transactions;

public sealed class EntityFrameworkBitacoraTransaction(IDbContextTransaction innerTransaction) : IBitacoraTransaction
{
    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        return innerTransaction.CommitAsync(cancellationToken);
    }

    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        return innerTransaction.RollbackAsync(cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        return innerTransaction.DisposeAsync();
    }
}
