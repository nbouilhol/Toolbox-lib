using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace BouilholLib.Helper
{
    public class PropertyMetadata
    {
        private readonly Type type;
        private readonly Delegate setter;
        private readonly PropertyInfo property;
        private readonly Delegate getter;

        public PropertyMetadata(Type type, PropertyInfo property)
        {
            this.type = type;
            this.property = property;
            this.setter = BuildSetterDelegate(type, property);
            this.getter = BuildGetterDelegate(property);
            PropertyName = MapPropertyName(property);
        }

        static string MapPropertyName(PropertyInfo property)
        {
            var attribute = (ColumnAttribute)Attribute.GetCustomAttribute(property, typeof(ColumnAttribute));

            if (attribute == null)
                return property.Name;

            return attribute.Name;
        }

        public PropertyInfo Property
        {
            get { return property; }
        }

        public string PropertyName
        {
            get;
            protected set;
        }

        public void SetValue(object instance, object value)
        {
            setter.DynamicInvoke(instance, value);
        }

        public object GetValue(object instance)
        {
            return getter.DynamicInvoke(instance);
        }

        private static Delegate BuildSetterDelegate(Type type, PropertyInfo prop)
        {
            var instance = Expression.Parameter(type, "x");
            var argument = Expression.Parameter(typeof(object), "v");

            var setterCall = Expression.Call(instance, prop.GetSetMethod(true), Expression.Convert(argument, prop.PropertyType));

            return Expression.Lambda(setterCall, instance, argument).Compile();
        }

        private Delegate BuildGetterDelegate(PropertyInfo prop)
        {
            var param = Expression.Parameter(type, "x");
            Expression expression = Expression.PropertyOrField(param, prop.Name);

            if (prop.PropertyType.IsValueType)
                expression = Expression.Convert(expression, typeof(object));

            return Expression.Lambda(expression, param).Compile();
        }
    }

    public class PropertyMetadata<T>
    {
        private readonly Action<T, object> setter;
        private readonly PropertyInfo property;
        private readonly Func<T, object> getter;

        public PropertyMetadata(PropertyInfo property)
        {
            this.property = property;
            this.setter = BuildSetterDelegate(property);
            this.getter = BuildGetterDelegate(property);
            PropertyName = MapPropertyName(property);
        }

        static string MapPropertyName(PropertyInfo property)
        {
            var attribute = (ColumnAttribute)Attribute.GetCustomAttribute(property, typeof(ColumnAttribute));

            if (attribute == null)
                return property.Name;

            return attribute.Name;
        }

        public PropertyInfo Property
        {
            get { return property; }
        }

        public string PropertyName
        {
            get;
            protected set;
        }

        public void SetValue(T instance, object value)
        {
            setter(instance, value);
        }

        public object GetValue(T instance)
        {
            return getter(instance);
        }

        private static Action<T, object> BuildSetterDelegate(PropertyInfo prop)
        {
            var instance = Expression.Parameter(typeof(T), "x");
            var argument = Expression.Parameter(typeof(object), "v");

            var setterCall = Expression.Call(instance, prop.GetSetMethod(true), Expression.Convert(argument, prop.PropertyType));

            return (Action<T, object>)Expression.Lambda(setterCall, instance, argument).Compile();
        }

        private Func<T, object> BuildGetterDelegate(PropertyInfo prop)
        {
            var param = Expression.Parameter(typeof(T), "x");
            Expression expression = Expression.PropertyOrField(param, prop.Name);

            if (prop.PropertyType.IsValueType)
                expression = Expression.Convert(expression, typeof(object));

            return Expression.Lambda<Func<T, object>>(expression, param).Compile();
        }
    }
}
