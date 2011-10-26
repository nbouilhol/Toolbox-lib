using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;

namespace BouilholLib.Helper
{
    public class MemoryCacheManager<TKey, TResult>
        where TKey : class
        where TResult : class
    {
        private ObjectCache cache;
        private readonly int minutesBeforeDelete;

        public MemoryCacheManager(int minutes)
        {
            this.minutesBeforeDelete = minutes;
            cache = MemoryCache.Default;
        }

        public bool Contains(TKey param)
        {
            return cache.Contains(param.GetHashCode().ToString());
        }

        public TResult Get(TKey param)
        {
            return cache.Get(param.GetHashCode().ToString()) as TResult;
        }

        public void Add(TKey param, TResult result)
        {
            if (minutesBeforeDelete == default(int))
                return;

            cache.Set(param.GetHashCode().ToString(), result, DateTimeOffset.Now.AddMinutes(minutesBeforeDelete));
        }

        public void Clear(TKey param)
        {
            cache.Remove(param.GetHashCode().ToString());
        }

        public IEnumerable<string> GetAll()
        {
            return cache.Select(keyValuePair => keyValuePair.Key).ToList();
        }
    }
}
