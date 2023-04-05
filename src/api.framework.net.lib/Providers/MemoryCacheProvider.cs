using api.framework.net.Lib.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace api.framework.net.Lib.Providers
{
    public class MemoryCacheProvider : ICacheProvider
    {
        ObjectCache cache = MemoryCache.Default;
        

        public void Add<T>(string key, T value, int expMinutes) where T : class
        {
            cache.Add(key, value, new CacheItemPolicy { AbsoluteExpiration = DateTime.UtcNow.AddMinutes(expMinutes) });
        }

        

        public T Get<T>(string key, int cacheExp, Func<string, T> getItemDelegate, string par) where T : class
        {
            T data = cache.Get(key) as T;
            if (data != null)
            {
                return data;
            }
            data = getItemDelegate(par);
            Add<T>(key, data, cacheExp);
            return data;
        }
    }
}
