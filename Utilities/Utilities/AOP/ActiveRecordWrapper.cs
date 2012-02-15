using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Utilities.AOP
{
    public class ActiveRecordWrapper<T> : BaseWrapper<T> where T : class
    {
        private readonly string connectionString;
        private readonly string table;

        public ActiveRecordWrapper(T source, string connectionString, string table)
            : base(source)
        {
            Contract.Requires(!string.IsNullOrEmpty(table));
            Contract.Requires(!string.IsNullOrEmpty(connectionString));

            this.connectionString = connectionString;
            this.table = table;
        }

        public override object OnInvoke(T source, string methodName, object[] args)
        {
            if (methodName == "FindAll")
                return Utilities.SqlHelpers.Query.FindAllDynamic(connectionString, table);
            if (methodName.StartsWith("FindAllBy"))
            {
                IDictionary<string, object> where = new Dictionary<string, object> { { methodName.Replace("FindAllBy", ""), args != null ? args.FirstOrDefault() : null } };
                return Utilities.SqlHelpers.Query.FindAllByDynamic(connectionString, table, where);
            }
            return base.OnInvoke(source, methodName, args);
        }
    }
}
