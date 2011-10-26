using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace BouilholLib.Helper
{
    public class EnumList<T> : IEnumerable<T> where T : struct
    {
        public EnumList()
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("Generic parameter must be enum");
            }
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }
    }
}
