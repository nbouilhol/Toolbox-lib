using System;
using System.Collections.Concurrent;
using Utilities.Extensions;

namespace Utilities
{
    public class SwitchCase<TKey> : ConcurrentDictionary<TKey, Action>
    {
        private Action defaultAction;

        public SwitchCase()
        {
        }

        public SwitchCase(Action defaultAction)
        {
            this.defaultAction = defaultAction;
        }

        public void Eval(TKey key)
        {
            Action action = this.TryGetValue(key, defaultAction);
            if (action != null) action();
        }
    }
}