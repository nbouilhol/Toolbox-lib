using System;
using System.Collections.Generic;

namespace Utilities
{
    public class SwitchCase<TKey> : Dictionary<TKey, Action>
    {
        private Action defaultAction;

        public SwitchCase() { }

        public SwitchCase(Action defaultAction)
        {
            this.defaultAction = defaultAction;
        }

        public void Eval(TKey key)
        {
            if (this.ContainsKey(key))
            {
                Action action = this[key];
                if (action != null) action();
            }
            else
                this.defaultAction();
        }
    }
}
