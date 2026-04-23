using NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Persistence;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.Domain.Entities;

namespace NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Repositories;

public sealed class AnalyticsEventRepository(AppDbContext dbContext) : IAnalyticsEventRepository
{
    public async ValueTask AddAsync(AnalyticsEvent analyticsEvent, CancellationToken cancellationToken = default)
    {
        await dbContext.AnalyticsEvents.AddAsync(analyticsEvent, cancellationToken);
    }
}
