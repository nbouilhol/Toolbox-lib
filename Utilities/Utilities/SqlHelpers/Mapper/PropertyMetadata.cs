using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;

namespace Utilities.SqlHelpers.Mapper
{
    public class PropertyMetadata
    {
        private readonly Delegate _getter;
        private readonly PropertyInfo _property;
        private readonly Delegate _setter;
        private readonly Type _type;

        public PropertyMetadata(Type type, PropertyInfo property)
        {
            Contract.Requires(type != null);
            Contract.Requires(property != null);

            _type = type;
            _property = property;
            _setter = BuildSetterDelegate(type, property);
            _getter = BuildGetterDelegate(property);
            PropertyName = MapPropertyName(property);
        }

        public PropertyInfo Property
        {
            get { return _property; }
        }

        public string PropertyName { get; protected set; }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(_property != null);
            Contract.Invariant(_setter != null);
            Contract.Invariant(_getter != null);
        }

        private static string MapPropertyName(PropertyInfo property)
        {
            var attribute = (ColumnAttribute) Attribute.GetCustomAttribute(property, typeof (ColumnAttribute));

            if (attribute != null)
                return attribute.Name;
            if (property != null)
                return property.Name;
            return null;
        }

        public void SetValue(object instance, object value)
        {
            _setter.DynamicInvoke(instance, value);
        }

        public object GetValue(object instance)
        {
            return _getter.DynamicInvoke(instance);
        }

        private static Delegate BuildSetterDelegate(Type type, PropertyInfo propertyInfo)
        {
            Contract.Requires(type != null);
            Contract.Requires(propertyInfo != null);

            ParameterExpression instance = Expression.Parameter(type, "x");
            ParameterExpression argument = Expression.Parameter(typeof (object), "v");
            MethodInfo setMethod = propertyInfo.GetSetMethod(true);

            if (setMethod == null) throw new UtilitiesException("setMethod");

            MethodCallExpression setterCall = Expression.Call(instance, setMethod,
                Expression.Convert(argument, propertyInfo.PropertyType));

            return Expression.Lambda(setterCall, instance, argument).Compile();
        }

        private Delegate BuildGetterDelegate(PropertyInfo propertyInfo)
        {
            Contract.Requires(_type != null);
            Contract.Requires(propertyInfo != null);

            ParameterExpression param = Expression.Parameter(_type, "x");
            Expression expression = Expression.PropertyOrField(param, propertyInfo.Name);

            if (propertyInfo.PropertyType.IsValueType)
                expression = Expression.Convert(expression, typeof (object));

            return Expression.Lambda(expression, param).Compile();
        }
    }

    public class PropertyMetadata<T>
    {
        private readonly Func<T, object> _getter;
        private readonly PropertyInfo _property;
        private readonly Action<T, object> _setter;

        public PropertyMetadata(PropertyInfo property)
        {
            Contract.Requires(property != null);

            _property = property;
            _setter = BuildSetterDelegate(property);
            _getter = BuildGetterDelegate(property);
            PropertyName = MapPropertyName(property);
        }

        public PropertyInfo Property
        {
            get { return _property; }
        }

        public string PropertyName { get; protected set; }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(_property != null);
            Contract.Invariant(_setter != null);
            Contract.Invariant(_getter != null);
        }

        private static string MapPropertyName(PropertyInfo property)
        {
            var attribute = (ColumnAttribute) Attribute.GetCustomAttribute(property, typeof (ColumnAttribute));

            if (attribute != null)
                return attribute.Name;
            if (property != null)
                return property.Name;
            return null;
        }

        public void SetValue(T instance, object value)
        {
            _setter(instance, value);
        }

        public object GetValue(T instance)
        {
            return _getter(instance);
        }

        private static Action<T, object> BuildSetterDelegate(PropertyInfo propertyInfo)
        {
            Contract.Requires(propertyInfo != null);

            ParameterExpression instance = Expression.Parameter(typeof (T), "x");
            ParameterExpression argument = Expression.Parameter(typeof (object), "v");
            MethodInfo setMethod = propertyInfo.GetSetMethod(true);

            if (setMethod == null) throw new UtilitiesException("setMethod");

            MethodCallExpression setterCall = Expression.Call(instance, setMethod,
                Expression.Convert(argument, propertyInfo.PropertyType));

            return (Action<T, object>) Expression.Lambda(setterCall, instance, argument).Compile();
        }

        private static Func<T, object> BuildGetterDelegate(PropertyInfo propertyInfo)
        {
            Contract.Requires(propertyInfo != null);

            ParameterExpression param = Expression.Parameter(typeof (T), "x");
            Expression expression = Expression.PropertyOrField(param, propertyInfo.Name);

            if (propertyInfo.PropertyType.IsValueType)
                expression = Expression.Convert(expression, typeof (object));

            return Expression.Lambda<Func<T, object>>(expression, param).Compile();
        }
    }
}