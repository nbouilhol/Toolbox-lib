using System;
using System.Diagnostics.Contracts;

namespace Utilities.Extensions
{
    public static class DynamicExtension
    {
        public static bool IsSimpleType(this Type type)
        {
            Contract.Requires(type != null);

            return type.IsPrimitive;
        }

        public static bool IsValueType(this Type type)
        {
            Contract.Requires(type != null);

            return type.IsValueType;
        }

        //public static XElement ToXml(this object input)
        //{
        //    return input.ToXml(null);
        //}

        //public static XElement ToXml(this object input, string element)
        //{
        //    if (input == null)
        //        return null;

        //    if (String.IsNullOrEmpty(element))
        //        element = "object";

        //    XElement xElement = new XElement(XmlConvert.EncodeName(element));

        //    xElement.Add(input.GetType().GetProperties()
        //        .Select(property => new
        //        {
        //            Name = XmlConvert.EncodeName(property.Name),
        //            Val = property.PropertyType.IsArray ? "array" : property.GetValue(input, null),
        //            Property = property
        //        })
        //        .Select(elem => new
        //        {
        //            Name = elem.Name,
        //            Val = elem.Val,
        //            Value = elem.Property.PropertyType.IsArray ? GetArrayElement(elem.Property, (Array)elem.Property.GetValue(input, null)) : (elem.Property.PropertyType.IsSimpleType() ? new XElement(elem.Name, elem.Val) : elem.Val.ToXml(elem.Name))
        //        }).WhereNotNull().ToList());

        //    return xElement;
        //}

        //private static XElement GetArrayElement(PropertyInfo info, Array input)
        //{
        //    XElement rootElement = new XElement(XmlConvert.EncodeName(info.Name));
        //    int arrayCount = input.GetLength(0);

        //    for (int i = 0; i < arrayCount; i++)
        //    {
        //        object val = input.GetValue(i);
        //        XElement childElement = val.GetType().IsSimpleType() ? new XElement(rootElement.Name + "Child", val) : val.ToXml();

        //        rootElement.Add(childElement);
        //    }

        //    return rootElement;
        //}
    }
}