using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Utilities.AOP
{
    public abstract class BaseWrapper<T> : DynamicObject where T : class
    {
        private readonly T _source;
        private readonly Type _type;

        protected BaseWrapper(T source)
        {
            _source = source;
            _type = typeof(T);
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(_type != null);
        }

        public T RealObject { get { return _source; } }

        protected Type TargetType { get { return _type; } }

        protected T Instance { get { return _source; } }

        protected BindingFlags BindingFlags
        {
            get { return BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic; }
        }

        #region DynamicObject

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _type.GetMembers().Select(member => member.Name).ToList();
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (binder == null) { result = null; return false; }
            result = binder.Type != null ? Convert.ChangeType(Instance, binder.Type) : Instance;
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            MemberInfo info = _type.GetMember(binder.Name, BindingFlags).FirstOrDefault();
            if (info == null || !(info is FieldInfo || info is PropertyInfo)) { result = null; return false; }
            result = info.Get(_source);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (string.IsNullOrEmpty(binder.Name)) { return false; }

            PropertyInfo property = _type.GetProperty(binder.Name, BindingFlags);
            if (property != null && property.CanWrite)
            {
                property.Set(_source, value);
                return true;
            }
            FieldInfo field = _type.GetField(binder.Name, BindingFlags);
            if (field == null) return false;
            field.Set(_source, value);
            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (binder.Name == null) { result = null; return false; }
            OnEntry(_source, binder.Name, args);

            try
            {
                result = OnInvoke(_source, binder.Name, args);
                OnSuccess(_source, binder.Name, args, result);
            }
            catch (Exception ex)
            {
                result = null;
                OnException(_source, binder.Name, args, ex);
            }

            OnExit(_source, binder.Name, args, result);

            return true;
        }

        public override string ToString()
        {
            return RealObject.ToString();
        }

        #endregion DynamicObject

        #region AOP

        public virtual object OnInvoke(T source, string methodName, object[] args)
        {
            Contract.Requires(methodName != null);

            try
            {
                return GetMethodAndInvoke(_type, source, methodName, args);
            }
            catch (MissingMethodException)
            {
                if (_type.BaseType != null)
                    return GetMethodAndInvoke(_type.BaseType, source, methodName, args);
                throw;
            }
        }

        public virtual void OnEntry(T source, string methodName, object[] args)
        {
        }

        public virtual void OnExit(T source, string methodName, object[] args, object result)
        {
        }

        public virtual void OnException(T source, string methodName, object[] args, Exception ex)
        {
        }

        public virtual void OnSuccess(T source, string methodName, object[] args, object result)
        {
        }

        #endregion AOP

        private MethodInfo GetMethodInfo(Type type, string methodName)
        {
            Contract.Requires(methodName != null);
            Contract.Requires(type != null);

            return type.GetMethod(methodName, BindingFlags);
        }

        private static object Invoke(MethodInfo methodeInfo, T source, object[] args)
        {
            return methodeInfo != null ? methodeInfo.Invoke(source, args) : null;
        }

        private object GetMethodAndInvoke(Type type, T source, string methodName, object[] args)
        {
            Contract.Requires(methodName != null);
            Contract.Requires(type != null);

            MethodInfo methodeInfo = GetMethodInfo(type, methodName);
            return methodeInfo != null ? Invoke(methodeInfo, source, args) : null;
        }
    }

    public static class MemberExtensions
    {
        public static object Get<T>(this MemberInfo memberInfo, T source)
        {
            Contract.Requires(memberInfo != null);

            PropertyInfo info = memberInfo as PropertyInfo;
            if (info != null) return info.GetValue(source, null);
            return ((FieldInfo)memberInfo).GetValue(source);
        }

        public static object Set<T>(this PropertyInfo propertyInfo, T source, object value)
        {
            Contract.Requires(propertyInfo != null);

            propertyInfo.SetValue(source, value, null);
            return value;
        }

        public static object Set<T>(this FieldInfo fieldInfo, T source, object value)
        {
            Contract.Requires(fieldInfo != null);

            fieldInfo.SetValue(source, value);
            return value;
        }
    }
}