using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Xml.Linq;
using System.Collections;

namespace BouilholLib.Helper
{
    public class DynamicXmlParser : DynamicObject
    {
        private XElement element;

        public DynamicXmlParser(string filename)
        {
            this.element = XElement.Load(filename);
        }

        private DynamicXmlParser(XElement element)
        {
            this.element = element;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            XElement setNode = element.Element(binder.Name);

            if (setNode != null)
                setNode.SetValue(value);
            else
            {
                if (value.GetType() == typeof(DynamicXmlParser))
                    element.Add(new XElement(binder.Name));
                else
                    element.Add(new XElement(binder.Name, value));
            }
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;

            if (element == null)
                return false;

            XElement child = element.Element(binder.Name);

            if (child == null)
                return false;

            result = new DynamicXmlParser(child);
            return true;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (binder.Type == typeof(XElement))
                result = element;
            else if (binder.Type == typeof(IEnumerable<dynamic>) || binder.Type == typeof(IEnumerable<object>))
                result = Cast<dynamic>();
            else if (binder.Type == typeof(object))
                result = (dynamic)element;
            else
                result = !string.IsNullOrEmpty(element.Value) ? Convert.ChangeType(element.Value, binder.Type) : GetDefault(binder.Type);

            return true;
        }

        public override string ToString()
        {
            if (element != null)
                return element.Value;
            return null;
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
            return (TResult)(!string.IsNullOrEmpty(element.Value) ? Convert.ChangeType(element.Value, typeof(TResult)) : GetDefault(typeof(TResult)));
        }

        public IEnumerable<TResult> Cast<TResult>()
        {
            return element.Elements().Select(child => new DynamicXmlParser(child).As<TResult>()).WhereNotNull();
        }

        public static explicit operator List<dynamic>(DynamicXmlParser parser)
        {
            return parser.Cast<dynamic>().ToList();
        }

        public static object GetDefault(Type type)
        {
            if (type.IsValueType)
                return Activator.CreateInstance(type);
            return null;
        }
    }
}
