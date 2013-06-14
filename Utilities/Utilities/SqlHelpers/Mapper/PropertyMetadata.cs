using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;

namespace Utilities.SqlHelpers.Mapper
{
    public class PropertyMetadata
    {
        private readonly Type type;
        private readonly Delegate setter;
        private readonly PropertyInfo property;
        private readonly Delegate getter;

        public PropertyMetadata(Type type, PropertyInfo property)
        {
            Contract.Requires(type != null);
            Contract.Requires(property != null);

            this.type = type;
            this.property = property;
            this.setter = BuildSetterDelegate(type, property);
            this.getter = BuildGetterDelegate(property);
            PropertyName = MapPropertyName(property);
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(property != null);
            Contract.Invariant(setter != null);
            Contract.Invariant(getter != null);
        }

        private static string MapPropertyName(PropertyInfo property)
        {
            var attribute = (ColumnAttribute)Attribute.GetCustomAttribute(property, typeof(ColumnAttribute));

            if (attribute != null)
                return attribute.Name;
            if (property != null)
                return property.Name;
            return null;
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

        private static Delegate BuildSetterDelegate(Type type, PropertyInfo propertyInfo)
        {
            Contract.Requires(type != null);
            Contract.Requires(propertyInfo != null);

            ParameterExpression instance = Expression.Parameter(type, "x");
            ParameterExpression argument = Expression.Parameter(typeof(object), "v");
            MethodInfo setMethod = propertyInfo.GetSetMethod(true);

            if (setMethod == null) throw new UtilitiesException("setMethod");

            MethodCallExpression setterCall = Expression.Call(instance, setMethod, Expression.Convert(argument, propertyInfo.PropertyType));

            return Expression.Lambda(setterCall, instance, argument).Compile();
        }

        private Delegate BuildGetterDelegate(PropertyInfo propertyInfo)
        {
            Contract.Requires(type != null);
            Contract.Requires(propertyInfo != null);

            ParameterExpression param = Expression.Parameter(type, "x");
            Expression expression = Expression.PropertyOrField(param, propertyInfo.Name);

            if (propertyInfo.PropertyType.IsValueType)
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
            Contract.Requires(property != null);

            this.property = property;
            this.setter = BuildSetterDelegate(property);
            this.getter = BuildGetterDelegate(property);
            PropertyName = MapPropertyName(property);
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(property != null);
            Contract.Invariant(setter != null);
            Contract.Invariant(getter != null);
        }

        private static string MapPropertyName(PropertyInfo property)
        {
            var attribute = (ColumnAttribute)Attribute.GetCustomAttribute(property, typeof(ColumnAttribute));

            if (attribute != null)
                return attribute.Name;
            if (property != null)
                return property.Name;
            return null;
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

        private static Action<T, object> BuildSetterDelegate(PropertyInfo propertyInfo)
        {
            Contract.Requires(propertyInfo != null);

            ParameterExpression instance = Expression.Parameter(typeof(T), "x");
            ParameterExpression argument = Expression.Parameter(typeof(object), "v");
            MethodInfo setMethod = propertyInfo.GetSetMethod(true);

            if (setMethod == null) throw new UtilitiesException("setMethod");

            MethodCallExpression setterCall = Expression.Call(instance, setMethod, Expression.Convert(argument, propertyInfo.PropertyType));

            return (Action<T, object>)Expression.Lambda(setterCall, instance, argument).Compile();
        }

        private static Func<T, object> BuildGetterDelegate(PropertyInfo propertyInfo)
        {
            Contract.Requires(propertyInfo != null);

            ParameterExpression param = Expression.Parameter(typeof(T), "x");
            Expression expression = Expression.PropertyOrField(param, propertyInfo.Name);

            if (propertyInfo.PropertyType.IsValueType)
                expression = Expression.Convert(expression, typeof(object));

            return Expression.Lambda<Func<T, object>>(expression, param).Compile();
        }
    }
}