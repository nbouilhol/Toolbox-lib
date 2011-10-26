namespace BouilholLib.Helper
{
    #region

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    #endregion

    public static class ObjectExtension
    {
        /// <summary>
        /// F# - |>
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="arg1"></param>
        /// <param name="func"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        public static TResult Forward<TArg1, TArg2, TResult>(this TArg1 arg1, Func<TArg1, TArg2, TResult> func, TArg2 arg2)
        {
            return func(arg1, arg2);
        }

        /// <summary>
        /// F# - |>
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <param name="arg1"></param>
        /// <param name="action"></param>
        /// <param name="arg2"></param>
        public static void Forward<TArg1, TArg2>(this TArg1 arg1, Action<TArg1, TArg2> action, TArg2 arg2)
        {
            action(arg1, arg2);
        }

        /// <summary>
        /// F# - <|
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="arg2"></param>
        /// <param name="func"></param>
        /// <param name="arg1"></param>
        /// <returns></returns>
        public static TResult Rev<TArg1, TArg2, TResult>(this TArg2 arg2, Func<TArg1, TArg2, TResult> func, TArg1 arg1)
        {
            return func(arg1, arg2);
        }

        /// <summary>
        /// F# - <|
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <param name="arg2"></param>
        /// <param name="action"></param>
        /// <param name="arg1"></param>
        public static void Rev<TArg1, TArg2>(this TArg2 arg2, Action<TArg1, TArg2> action, TArg1 arg1)
        {
            action(arg1, arg2);
        }

        /// <summary>
        /// Clone properties from an original object to a destination object.
        /// </summary>
        /// <param name="origin">
        /// The origin.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <typeparam name="T1">
        /// </typeparam>
        /// <typeparam name="T2">
        /// </typeparam>
        public static void CloneProperties<T1, T2>(this T1 origin, T2 destination)
            where T2 : class
            where T1 : class
        {
            // Instantiate if necessary
            Guard.AgainstNull(destination, "destination");

            // Loop through each property in the destination
            foreach (var destinationProperty in destination.GetType().GetProperties()) // find and set val if we can find a matching property name and matching type in the origin with the origin's value
                if (origin != null && destinationProperty.CanWrite)
                {
                    PropertyInfo destinationProperty1 = destinationProperty;
                    origin.GetType().GetProperties().Where(
                        x => x.CanRead && (x.Name == destinationProperty1.Name && x.PropertyType == destinationProperty1.PropertyType))
                        .ToList().ForEach(x => destinationProperty1.SetValue(destination, x.GetValue(origin, null), null));
                }
        }

        /// <summary>
        /// Gets the readable (non indexed) properties names and values.
        /// The keys holds the names of the properties.
        /// The values are the values of the properties
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The get properties dictionary.
        /// </returns>
        public static IDictionary GetPropertiesDictionary(this object obj)
        {
            object propertyValue;
            IDictionary ht = new Hashtable();

            foreach (var property in
                obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public
                                            | BindingFlags.NonPublic))
            {
                if (!property.CanRead || property.GetIndexParameters().Length != 0)
                    continue;
                if (property.PropertyType.IsSimple())
                {
                    propertyValue = property.GetValue(obj, null);
                    ht[property.Name] = propertyValue == null
                                            ? null
                                            : propertyValue.ToString();
                }
                else
                    ht[property.Name] = property.GetValue(obj, null);
            }

            return ht;
        }

        public static object GetPropertyValue(this object obj, string property)
        {
            return TypeDescriptor.GetProperties(obj)[property].GetValue(obj);
        }

        public static IDictionary<string, object> ToDictionary(this object obj)
        {
            IDictionary<string, object> result = new Dictionary<string, object>();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(obj);
            foreach (PropertyDescriptor property in properties)
            {
                result.Add(property.Name, property.GetValue(obj));
            }

            return result;
        }

        /// <summary>
        /// Return true or false
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static bool WhenNotNull<T>(this T value, Action func) where T : class
        {
            if (value != null)
            {
                func();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Return true or false
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static bool WhenNotNull<T>(this T? value, Action func) where T : struct
        {
            if (value != null)
            {
                func();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Return true or false
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static bool WhenNotNull<T>(this T value, Action<T> func) where T : class
        {
            if (value != null)
            {
                func(value);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Return TV or null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <param name="value"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static TV WhenNotNull<T, TV>(this T value, Func<TV> func)
            where T : class
            where TV : class
        {
            return value != null ? func() : null;
        }

        /// <summary>
        /// Return TV or null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <param name="value"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static TV WhenNotNull<T, TV>(this T? value, Func<TV> func)
            where T : struct
            where TV : class
        {
            return value != null ? func() : null;
        }

        /// <summary>
        /// Return TV or default 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <param name="value"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static TV WhenNotNullS<T, TV>(this T? value, Func<TV> func)
            where T : struct
            where TV : struct
        {
            return value != null ? func() : default(TV);
        }
		
		public static string GetMethodName<T, TProperty>(this T owner, Expression<Func<T, TProperty>> selector) where T : class
        {
            return ((MethodCallExpression)(selector.Body is UnaryExpression ? ((UnaryExpression)selector.Body).Operand : selector.Body)).Method.Name;
        }

        public static string GetMethodName<T>(this T owner, Expression<Action<T>> selector) where T : class
        {
            return ((MethodCallExpression)(selector.Body is UnaryExpression ? ((UnaryExpression)selector.Body).Operand : selector.Body)).Method.Name;
        }

        public static string GetPropertyName<T, TProperty>(this T owner, Expression<Func<T, TProperty>> property) where T : class
        {
            return ((MemberExpression)(property.Body is UnaryExpression ? ((UnaryExpression)property.Body).Operand : property.Body)).Member.Name;
        }
    }
}