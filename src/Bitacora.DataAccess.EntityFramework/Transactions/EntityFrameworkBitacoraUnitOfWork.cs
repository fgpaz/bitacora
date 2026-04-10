using NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Persistence;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Transactions;

namespace NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Transactions;

public sealed class EntityFrameworkBitacoraUnitOfWork(AppDbContext dbContext) : IBitacoraUnitOfWork
{
    public async Task<IBitacoraTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        return new EntityFrameworkBitacoraTransaction(transaction);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
