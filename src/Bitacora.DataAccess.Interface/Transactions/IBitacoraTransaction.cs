namespace NuestrasCuentitas.Bitacora.DataAccess.Interface.Transactions;

public interface IBitacoraTransaction : IAsyncDisposable
{
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
