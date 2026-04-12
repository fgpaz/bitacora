namespace Shared.Cache;

public interface ICacheService
{
    bool TryGetValue<TValue>(string key, out TValue? value);
    void Set<TValue>(string key, TValue value, TimeSpan ttl);
}
