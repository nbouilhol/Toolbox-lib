using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Utilities
{
    public class EnumList<T> : IEnumerable<T> where T : struct
    {
        public EnumList()
        {
            Contract.Requires(typeof (T).IsEnum);

            if (!typeof (T).IsEnum)
            {
                throw new ArgumentException("Generic parameter must be enum");
            }
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            return Enum.GetValues(typeof (T)).Cast<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>) this).GetEnumerator();
        }
    }
}