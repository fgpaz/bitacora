namespace NuestrasCuentitas.Bitacora.DataAccess.Interface.Transactions;

public interface IBitacoraUnitOfWork
{
    Task<IBitacoraTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
