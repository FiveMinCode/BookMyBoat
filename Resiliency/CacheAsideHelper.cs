using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Resiliency
{
    // This class uses MemoryCache to store and retrieve cached data.
    public class CacheAsideHelper
    {
        private readonly ObjectCache _cache = MemoryCache.Default;

        public T Get<T>(string key)
        {
            return (T)_cache[key];
        }

        public void Set<T>(string key, T value, TimeSpan expiration)
        {
            var policy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.Add(expiration) };
            _cache.Set(key, value, policy);
        }
    }


    // Example usage:
    public class SomeService
    {
        private readonly HttpClient _client;
        private readonly CacheAsideHelper _cacheHelper;

        public SomeService()
        {
            _client = new HttpClient();
            _cacheHelper = new CacheAsideHelper();
        }

        public async Task<string> GetDataAsync(string url)
        {
            string cacheKey = $"Cache_{url}";
            TimeSpan cacheExpiration = TimeSpan.FromMinutes(5);

            // Try to get data from the cache
            string cachedData = _cacheHelper.Get<string>(cacheKey);
            if (cachedData != null)
            {
                return cachedData;
            }

            // If data is not in the cache, retrieve it from the HTTP API
            HttpResponseMessage response = await _client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                _cacheHelper.Set(cacheKey, data, cacheExpiration);
                return data;
            }
            else
            {
                throw new Exception($"HTTP request failed with status code: {response.StatusCode}");
            }
        }
    }
}
