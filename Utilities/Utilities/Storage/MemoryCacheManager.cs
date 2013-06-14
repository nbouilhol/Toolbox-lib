using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Runtime.Caching;

namespace Utilities.Storage
{
    public class MemoryCacheManager<TKey, TResult>
        where TKey : class
        where TResult : class
    {
        private readonly ObjectCache _cache;
        private readonly int _minutesBeforeDelete;

        public MemoryCacheManager(int minutes)
        {
            _minutesBeforeDelete = minutes;
            _cache = MemoryCache.Default;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(_cache != null);
        }

        public bool Contains(TKey param)
        {
            return _cache.Contains(param.GetHashCode().ToString(CultureInfo.InvariantCulture));
        }

        public TResult Get(TKey param)
        {
            return _cache.Get(param.GetHashCode().ToString(CultureInfo.InvariantCulture)) as TResult;
        }

        public void Add(TKey param, TResult result)
        {
            if (_minutesBeforeDelete == default(int))
                return;

            _cache.Set(param.GetHashCode().ToString(CultureInfo.InvariantCulture), result, DateTimeOffset.Now.AddMinutes(_minutesBeforeDelete));
        }

        public void Clear(TKey param)
        {
            _cache.Remove(param.GetHashCode().ToString(CultureInfo.InvariantCulture));
        }

        public IEnumerable<string> GetAll()
        {
            return _cache.Select(keyValuePair => keyValuePair.Key).ToList();
        }
    }
}