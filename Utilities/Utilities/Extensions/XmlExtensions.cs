using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Utilities.Extensions
{
    public static class XmlExtensions
    {
        public static Nullable<int> ToInt(this XAttribute source)
        {
            if (source == null || source.Value == null) return null;
            return source.Value.ToInt();
        }

        public static Nullable<bool> ToBool(this XAttribute source)
        {
            if (source == null || source.Value == null) return null;
            return source.Value.ToBool();
        }

        public static Nullable<DateTime> ToDateTime(this XAttribute source)
        {
            if (source == null || source.Value == null) return null;
            DateTime result;
            if (DateTime.TryParse(source.Value, out result)) return result;
            return null;
        }

        public static Nullable<TEnum> ToEnum<TEnum>(this XAttribute source)
            where TEnum : struct
        {
            if (source == null || source.Value == null) return null;
            TEnum result;
            if (Enum.TryParse<TEnum>(source.Value, out result)) return result;
            return null;
        }

        public static string ValueToString(this XAttribute source)
        {
            return source != null ? source.Value : null;
        }

        public static IList<T> ExtractElements<T>(this XElement source, string name, Func<XElement, T> extract)
        {
            if (source == null) return new List<T>();
            XElement parent = source.Element(name);
            return ExtractElements(parent, extract);
        }

        public static IList<T> ExtractFlatElements<T>(this XElement source, string name, Func<XElement, IEnumerable<T>> extract)
        {
            if (source == null) return new List<T>();
            XElement parent = source.Element(name);
            return ExtractFlatElements(parent, extract);
        }

        public static IList<T> ExtractElements<T>(this XElement parent, Func<XElement, T> extract)
        {
            if (parent == null) return new List<T>();
            IEnumerable<XElement> childrens = parent.Elements() ?? Enumerable.Empty<XElement>();
            return childrens.Where(elem => elem != null).Select(elem => extract(elem)).ToList();
        }

        public static IList<T> ExtractFlatElements<T>(this XElement parent, Func<XElement, IEnumerable<T>> extract)
        {
            if (parent == null) return new List<T>();
            IEnumerable<XElement> childrens = parent.Elements() ?? Enumerable.Empty<XElement>();
            return childrens.Where(elem => elem != null).SelectMany(elem => extract(elem)).ToList();
        }
    }
}