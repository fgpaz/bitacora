using Microsoft.Extensions.Caching.Memory;
namespace Shared.Cache;

public sealed class InMemoryCacheService(IMemoryCache cache) : ICacheService
{
    public bool TryGetValue<TValue>(string key, out TValue? value)
    {
        if (cache.TryGetValue(key, out var raw) && raw is TValue typed)
        {
            value = typed;
            return true;
        }

        value = default;
        return false;
    }

    public void Set<TValue>(string key, TValue value, TimeSpan ttl)
    {
        cache.Set(key, value, ttl);
    }
}
