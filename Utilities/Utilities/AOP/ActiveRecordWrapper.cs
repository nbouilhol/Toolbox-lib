using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Utilities.SqlHelpers.Repository;

namespace Utilities.AOP
{
    public class ActiveRecordWrapper<T> : BaseWrapper<T> where T : class
    {
        private readonly string _connectionString;
        private readonly string _table;

        public ActiveRecordWrapper(T source, string connectionString, string table)
            : base(source)
        {
            Contract.Requires(!string.IsNullOrEmpty(table));
            Contract.Requires(!string.IsNullOrEmpty(connectionString));

            _connectionString = connectionString;
            _table = table;
        }

        public override object OnInvoke(T source, string methodName, object[] args)
        {
            if (methodName == "FindAll") return Query.FindAllDynamic(_connectionString, _table);
            if (!methodName.StartsWith("FindAllBy")) return base.OnInvoke(source, methodName, args);
            IDictionary<string, object> where = new Dictionary<string, object> { { methodName.Replace("FindAllBy", ""), args != null ? args.FirstOrDefault() : null } };
            return Query.FindAllByDynamic(_connectionString, _table, @where);
        }
    }
}