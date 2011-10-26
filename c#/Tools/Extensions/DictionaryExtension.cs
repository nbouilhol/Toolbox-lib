using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace BouilholLib.Helper
{
    public static class DictionaryExtension
    {
        public static TElement TryGetValue<TKey, TElement>(this IDictionary<TKey, TElement> dic, TKey key, TElement returnIfFailed)
        {
            Contract.Requires(dic != null);

            TElement value;
            if (!dic.TryGetValue(key, out value))
                return returnIfFailed;
            return value;
        }

        public static TElement TryGetValue<TKey, TElement>(this IDictionary<TKey, TElement> dic, TKey key)
        {
            Contract.Requires(dic != null);

            TElement value;
            if (!dic.TryGetValue(key, out value))
                return default(TElement);
            return value;
        }
    }
}
