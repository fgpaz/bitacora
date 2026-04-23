using NuestrasCuentitas.Bitacora.Domain.Entities;

namespace NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;

public interface IAnalyticsEventRepository
{
    ValueTask AddAsync(AnalyticsEvent analyticsEvent, CancellationToken cancellationToken = default);
}
