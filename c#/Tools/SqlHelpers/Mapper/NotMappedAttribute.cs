using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BouilholLib.Helper
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NotMappedAttribute : Attribute
    {
    }
}
