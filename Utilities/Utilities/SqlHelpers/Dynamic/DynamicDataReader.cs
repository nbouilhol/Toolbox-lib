using System;
using System.Data;
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.Reflection;

namespace Utilities.SqlHelpers
{
    public class DynamicDataReader : DynamicObject
    {
        private IDataReader dataReader;

        protected BindingFlags BindingFlags
        {
            get { return BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic; }
        }

        public DynamicDataReader(IDataReader dataReader)
        {
            Contract.Requires(dataReader != null);

            this.dataReader = dataReader;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(dataReader != null);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (binder == null) { result = null; return false; }
            result = null;

            if (binder.Name == "IsClosed")
                result = dataReader.IsClosed;
            else if (binder.Name == "RecordsAffected")
                result = dataReader.RecordsAffected;
            else
            {
                try
                {
                    result = dataReader[binder.Name];
                    if (result == DBNull.Value)
                        result = null;
                }
                catch
                {
                    result = null;
                    return false;
                }
            }

            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (binder.Name == "Read")
                result = dataReader.Read();
            if (binder.Name == "Dispose")
            {
                dataReader.Dispose();
                result = null;
            }
            else if (binder.Name == "Close")
            {
                dataReader.Close();
                result = null;
            }
            else
                result = GetMethodAndInvoke(binder.Name, args);
            return true;
        }

        private object GetMethodAndInvoke(string methodName, object[] args)
        {
            Contract.Requires(methodName != null);

            MethodInfo methodeInfo = typeof(IDataReader).BaseType != null ? typeof(IDataReader).BaseType.GetMethod(methodName, BindingFlags) : null;
            return methodeInfo != null ? methodeInfo.Invoke(dataReader, args) : null;
        }
    }
}