using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Xml;
using System.Xml.Linq;
using Utilities.Extensions;

namespace Utilities
{
    public static class DynamicXmlExtensions
    {
        public static dynamic Parse(string filename)
        {
            return new DynamicXmlElement(XElement.Load(filename));
        }

        public static dynamic Parse(Stream stream)
        {
            return new DynamicXmlElement(XElement.Load(stream));
        }

        public static dynamic ToDynamic(this XElement xml)
        {
            return new DynamicXmlElement(xml);
        }

        internal static dynamic IsCollection<T>(this IEnumerable<T> list)
        {
            return list.Skip(1).Any();
        }

        internal static bool TryXmlConvert(string value, Type returnType, out object result)
        {
            result = null;

            if (returnType == typeof(string))
            {
                result = value;
                return true;
            }
            if (returnType.IsEnum && Enum.IsDefined(returnType, value))
            {
                result = Enum.Parse(returnType, value);
                return true;
            }
            if (returnType.IsEnum)
            {
                result = Enum.ToObject(returnType, GetXmlConverter()[Enum.GetUnderlyingType(returnType)].Invoke(value));
                return true;
            }

            Func<string, object> converter;
            if (GetXmlConverter().TryGetValue(returnType, out converter))
            {
                result = converter(value);
                return true;
            }

            return false;
        }

        private static IDictionary<Type, Func<string, object>> GetXmlConverter()
        {
            return new Dictionary<Type, Func<string, object>>
			{
				{ typeof(bool), s => !string.IsNullOrEmpty(s) ? XmlConvert.ToBoolean(s) : default(bool) },
				{ typeof(byte), s => !string.IsNullOrEmpty(s) ? XmlConvert.ToByte(s) : default(byte) },
				{ typeof(char), s => !string.IsNullOrEmpty(s) ? XmlConvert.ToChar(s) : default(char) },
				{ typeof(DateTime), s => !string.IsNullOrEmpty(s) ? XmlConvert.ToDateTime(s, XmlDateTimeSerializationMode.RoundtripKind) : default(DateTime) },
				{ typeof(DateTimeOffset), s => !string.IsNullOrEmpty(s) ? XmlConvert.ToDateTimeOffset(s) : default(DateTimeOffset) },
				{ typeof(decimal), s => !string.IsNullOrEmpty(s) ? XmlConvert.ToDecimal(s.Replace(",", ".")) : default(decimal) },
				{ typeof(double), s => !string.IsNullOrEmpty(s) ? XmlConvert.ToDouble(s.Replace(",", ".")) : default(double) },
				{ typeof(Guid), s => !string.IsNullOrEmpty(s) ? XmlConvert.ToGuid(s) : default(Guid) },
				{ typeof(short), s => !string.IsNullOrEmpty(s) ? XmlConvert.ToInt16(s) : default(short) },
				{ typeof(int), s => !string.IsNullOrEmpty(s) ? XmlConvert.ToInt32(s) : default(int) },
				{ typeof(long), s => !string.IsNullOrEmpty(s) ? XmlConvert.ToInt64(s) : default(long) },
				{ typeof(sbyte), s => !string.IsNullOrEmpty(s) ? XmlConvert.ToSByte(s) : default(sbyte) },
				{ typeof(float), s => !string.IsNullOrEmpty(s) ? XmlConvert.ToSingle(s.Replace(",", ".")) : default(float) },
				{ typeof(TimeSpan), s => !string.IsNullOrEmpty(s) ? XmlConvert.ToTimeSpan(s) : default(TimeSpan) },
				{ typeof(ushort), s => !string.IsNullOrEmpty(s) ? XmlConvert.ToUInt16(s) : default(ushort) },
				{ typeof(uint), s => !string.IsNullOrEmpty(s) ? XmlConvert.ToUInt32(s) : default(uint) },
				{ typeof(ulong), s => !string.IsNullOrEmpty(s) ? XmlConvert.ToUInt64(s) : default(ulong) },
			};
        }
    }

    public class DynamicXmlElement : DynamicObject, IEnumerable<DynamicObject>
    {
        private readonly XElement element;

        public DynamicXmlElement(XElement element)
        {
            this.element = element;
        }

        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
        {
            if (binder == null) { result = null; return false; }
            if (arg == null) { result = null; return false; }

            if (binder.Operation == ExpressionType.GreaterThan)
            {
                Type type = arg is Type ? (Type)arg : arg.GetType();

                if (binder.ReturnType == typeof(XElement))
                {
                    result = element;
                    return true;
                }
                else if (type == typeof(IEnumerable<dynamic>) || type == typeof(IEnumerable<object>))
                {
                    result = Cast<dynamic>();
                    return true;
                }
                else if (type == typeof(object))
                {
                    result = (dynamic)element;
                    return true;
                }
                else if (DynamicXmlExtensions.TryXmlConvert(element.Value, binder.ReturnType, out result))
                    return true;
                else
                    result = !string.IsNullOrEmpty(element.Value) ? Convert.ChangeType(element.Value, type) : GetDefault(type);

                return true;
            }
            return base.TryBinaryOperation(binder, arg, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (binder == null) return false;

            XElement setNode = element.Element(binder.Name);

            if (setNode != null)
                setNode.SetValue(value);
            else
            {
                if (value != null && value.GetType() == typeof(DynamicXmlElement))
                    element.Add(new XElement(binder.Name));
                else
                    element.Add(new XElement(binder.Name, value));
            }
            return true;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            XName name = GetNameIndex(indexes);
            XAttribute existingAttr = element.Attribute(name);

            if (existingAttr != null)
            {
                result = new DynamicXmlAttribute(existingAttr);
                return true;
            }

            IEnumerable<XElement> matches = element.Elements(name);

            if (matches.IsCollection())
                result = new DynamicXmlElements(matches);
            else
                result = matches.Select(x => new DynamicXmlElement(x)).FirstOrDefault();

            return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            XName name = GetNameIndex(indexes);

            XAttribute existingAttr = element.Attribute(name);
            if (existingAttr != null)
            {
                if (value is XAttribute)
                    existingAttr.SetValue(((XAttribute)value).Value);
                else
                    existingAttr.SetValue(value as string);
                return true;
            }

            XElement existingEl = element.Element(name);
            if (existingEl != null)
                existingEl.SetValue(value);
            else
            {
                if (value is XAttribute)
                    element.Add((XAttribute)value);
                else
                    element.Add(new XAttribute(name, value));
            }

            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (binder == null) { result = null; return false; }

            IEnumerable<XElement> matches = element.Elements().Where(x => x.Name.LocalName.Equals(binder.Name));
            if (matches.IsCollection())
                result = new DynamicXmlElements(matches);
            else
                result = matches.Select(el => new DynamicXmlElement(el)).FirstOrDefault();

            return true;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (binder == null) { result = null; return false; }

            if (binder.ReturnType == typeof(XElement))
            {
                result = element;
                return true;
            }
            else if (binder.Type == typeof(IEnumerable<dynamic>) || binder.Type == typeof(IEnumerable<object>))
            {
                result = Cast<dynamic>();
                return true;
            }
            else if (binder.Type == typeof(object))
            {
                result = (dynamic)element;
                return true;
            }
            else if (DynamicXmlExtensions.TryXmlConvert(element.Value, binder.ReturnType, out result))
                return true;
            else
                result = !string.IsNullOrEmpty(element.Value) ? Convert.ChangeType(element.Value, binder.Type) : GetDefault(binder.Type);

            return base.TryConvert(binder, out result);
        }

        public override string ToString()
        {
            if (element != null)
                return element.Value;
            return string.Empty;
        }

        public string this[string attr]
        {
            get
            {
                if (element == null)
                    return string.Empty;
                return element.Attribute(attr).Value;
            }
        }

        public TResult As<TResult>()
        {
            if (typeof(TResult) == typeof(object))
                return (TResult)((dynamic)this);

            object result;
            if (DynamicXmlExtensions.TryXmlConvert(element.Value, typeof(TResult), out result))
                return (TResult)result;
            else
                return (TResult)(!string.IsNullOrEmpty(element.Value) ? Convert.ChangeType(element.Value, typeof(TResult)) : GetDefault(typeof(TResult)));
        }

        public IEnumerable<TResult> Cast<TResult>()
        {
            return element.Elements().Select(child => new DynamicXmlElement(child).As<TResult>()).WhereNotNull().ToList();
        }

        public static explicit operator List<dynamic>(DynamicXmlElement parser)
        {
            Contract.Requires(parser != null);

            return parser.Cast<dynamic>().ToList();
        }

        public static object GetDefault(Type type)
        {
            Contract.Requires(type != null);

            if (type.IsValueType)
                return CreateNewExpression(type).DynamicInvoke();
            return null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return element.Elements().Select(el => new DynamicXmlElement(el)).GetEnumerator();
        }

        IEnumerator<DynamicObject> IEnumerable<DynamicObject>.GetEnumerator()
        {
            return element.Elements().Select(el => new DynamicXmlElement(el)).GetEnumerator();
        }

        private static Delegate CreateNewExpression(Type type)
        {
            Contract.Requires(type != null);

            return Expression.Lambda(Expression.New(type)).Compile();
        }

        private XName GetNameIndex(object[] indexes)
        {
            if (indexes.Length != 1) throw new NotSupportedException("Attributes can only be accessed using a single index of type string or XName");

            object first = indexes.First();
            var result = default(XName);
            if (first is string)
                result = XName.Get((string)first);
            else if (first is XName)
                result = (XName)first;
            else
                throw new NotSupportedException("Attribute index can only be a simple attribute name, an expanded XML name string, or an XName");

            return result;
        }
    }

    public class DynamicXmlAttribute : DynamicObject
    {
        private readonly XAttribute attribute;

        public DynamicXmlAttribute(XAttribute attribute)
        {
            this.attribute = attribute;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (binder.ReturnType == typeof(XAttribute))
            {
                result = attribute;
                return true;
            }
            else if (DynamicXmlExtensions.TryXmlConvert(attribute.Value, binder.ReturnType, out result))
                return true;

            return base.TryConvert(binder, out result);
        }

        public override string ToString()
        {
            return attribute.Value;
        }
    }

    public class DynamicXmlElements : DynamicObject, IEnumerable<XElement>, IEnumerable<DynamicObject>
    {
        private List<XElement> elements;

        public DynamicXmlElements(IEnumerable<XElement> elements)
        {
            this.elements = elements.ToList();
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (binder.ReturnType.IsAssignableFrom(typeof(IEnumerable))
                || binder.ReturnType.IsAssignableFrom(typeof(IEnumerable<object>))
                || binder.ReturnType.IsAssignableFrom(typeof(IEnumerable<DynamicObject>)))
            {
                result = elements.Select(el => new DynamicXmlElement(el)).ToList();
                return true;
            }
            else if (binder.ReturnType == typeof(object[]))
            {
                result = elements.Select(el => (object)new DynamicXmlElement(el)).ToArray();
                return true;
            }

            return base.TryConvert(binder, out result);
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            var first = indexes.First();
            if (indexes.Length == 1 && first is int)
            {
                result = new DynamicXmlElement(elements[(int)first]);
                return true;
            }

            return base.TryGetIndex(binder, indexes, out result);
        }

        public IEnumerator<XElement> GetEnumerator()
        {
            return elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<DynamicObject>)this).GetEnumerator();
        }

        IEnumerator<DynamicObject> IEnumerable<DynamicObject>.GetEnumerator()
        {
            return elements.Select(el => new DynamicXmlElement(el)).GetEnumerator();
        }
    }
}