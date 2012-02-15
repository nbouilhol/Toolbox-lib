using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Utilities.AOP
{
    public abstract class BaseWrapper<T> : DynamicObject where T : class
    {
        private readonly T source;
        private readonly Type type;

        protected BaseWrapper(T source)
        {
            this.source = source;
            this.type = typeof(T);
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.type != null);
        }

        public T RealObject { get { return source; } }
        protected Type TargetType { get { return type; } }
        protected T Instance { get { return source; } }
        protected BindingFlags BindingFlags
        {
            get { return BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic; }
        }

        #region DynamicObject

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return type.GetMembers().Select(member => member.Name).ToList();
        }

        public override DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return base.GetMetaObject(parameter);
        }

        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
        {
            //throw new NotImplementedException("TryBinaryOperation");
            return base.TryBinaryOperation(binder, arg, out result);
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (binder == null) { result = null; return false; }
            result = binder.Type != null ? Convert.ChangeType(Instance, binder.Type) : Instance;
            return true;
        }

        public override bool TryCreateInstance(CreateInstanceBinder binder, object[] args, out object result)
        {
            //throw new NotImplementedException("TryCreateInstance");
            return base.TryCreateInstance(binder, args, out result);
        }

        public override bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes)
        {
            //throw new NotImplementedException("TryDeleteIndex");
            return base.TryDeleteIndex(binder, indexes);
        }

        public override bool TryDeleteMember(DeleteMemberBinder binder)
        {
            //throw new NotImplementedException("TryDeleteMember");
            return base.TryDeleteMember(binder);
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            //throw new NotImplementedException("TryGetIndex");
            return base.TryGetIndex(binder, indexes, out result);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (binder == null) { result = null; return false; }
            MemberInfo info = type.GetMember(binder.Name, BindingFlags).FirstOrDefault();
            if (info == null || !(info is FieldInfo || info is PropertyInfo)) { result = null; return false; }
            result = info.Get(source);
            return true;
        }

        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            return base.TryInvoke(binder, args, out result);
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            //throw new NotImplementedException("TrySetIndex");
            return base.TrySetIndex(binder, indexes, value);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (binder == null || string.IsNullOrEmpty(binder.Name)) { return false; }

            PropertyInfo property = type.GetProperty(binder.Name, BindingFlags);
            if (property != null && property.CanWrite)
            {
                property.Set(source, value);
                return true;
            }
            FieldInfo field = type.GetField(binder.Name, BindingFlags);
            if (field != null)
            {
                field.Set(source, value);
                return true;
            }
            return false;
        }

        public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result)
        {
            //throw new NotImplementedException("TryUnaryOperation");
            return base.TryUnaryOperation(binder, out result);
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (binder == null || binder.Name == null) { result = null; return false; }
            OnEntry(source, binder.Name, args);

            try
            {
                result = OnInvoke(source, binder.Name, args);
                OnSuccess(source, binder.Name, args, result);
            }
            catch (Exception ex)
            {
                result = null;
                OnException(source, binder.Name, args, ex);
            }

            OnExit(source, binder.Name, args, result);

            return true;
        }

        public override string ToString()
        {
            return RealObject.ToString();
        }

        #endregion

        #region AOP

        public virtual object OnInvoke(T source, string methodName, object[] args)
        {
            Contract.Requires(methodName != null);

            try
            {
                return GetMethodAndInvoke(type, source, methodName, args);
            }
            catch (MissingMethodException)
            {
                if (type.BaseType != null)
                    return GetMethodAndInvoke(type.BaseType, source, methodName, args);
                throw;
            }
        }

        public virtual void OnEntry(T source, string methodName, object[] args) { }
        public virtual void OnExit(T source, string methodName, object[] args, object result) { }
        public virtual void OnException(T source, string methodName, object[] args, Exception ex) { }
        public virtual void OnSuccess(T source, string methodName, object[] args, object result) { }

        #endregion

        private MethodInfo GetMethodInfo(Type type, string methodName)
        {
            Contract.Requires(methodName != null);
            Contract.Requires(type != null);

            return type.GetMethod(methodName, BindingFlags);
        }

        private object Invoke(MethodInfo methodeInfo, T source, object[] args)
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

            if (memberInfo is PropertyInfo)
                return ((PropertyInfo)memberInfo).GetValue(source, null);
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
