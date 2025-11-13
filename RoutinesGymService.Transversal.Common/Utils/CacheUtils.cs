using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace RoutinesGymService.Transversal.Common.Utils
{
    public class CacheUtils
    {
        private readonly IMemoryCache _cache;
        private readonly ConcurrentDictionary<string, byte> _keys = new();

        public CacheUtils(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void Set<T>(string key, T value, TimeSpan expiration)
        {
            _cache.Set(key, value, expiration);
            _keys[key] = 0; 
        }

        public T? Get<T>(string key)
        {
            _cache.TryGetValue<T>(key, out var value);
            return value;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);  
            _keys.TryRemove(key, out _);
        }

        public IEnumerable<string> GetAllKeys() => _keys.Keys;
    }
}a