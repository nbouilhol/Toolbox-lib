using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BouilholLib.Helper
{
    public class ActiveRecordWrapper<T> : BaseWrapper<T> where T : class
    {
        private readonly string connectionString;
        private readonly string table;

        public ActiveRecordWrapper(T source, string connectionString, string table)
            : base(source)
        {
            this.connectionString = connectionString;
            this.table = table;
        }

        public override object OnInvoke(T source, string methodName, object[] args)
        {
            if (methodName == "FindAll")
                return MoteurCGRP.Helper.SqlHelpers.Query.FindAllDynamic(connectionString, table);
            if (methodName.StartsWith("FindAllBy"))
            {
                IDictionary<string, object> where = new Dictionary<string, object> { { methodName.Replace("FindAllBy", ""), args.FirstOrDefault() } };
                return MoteurCGRP.Helper.SqlHelpers.Query.FindAllByDynamic(connectionString, table, where);
            }
            return base.OnInvoke(source, methodName, args);
        }
    }
}
