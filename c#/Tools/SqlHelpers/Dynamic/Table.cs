using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;

namespace BouilholLib.Helper
{
    public class Table : DynamicObject
    {
        private readonly string connectionString;
        private readonly string table;

        public Table(string connectionString, string table)
        {
            this.connectionString = connectionString;
            this.table = table;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = null;
            if (binder.Name.StartsWith(Method.FindAllBy.ToString()))
                result = Query.FindAllByDynamic(connectionString, table, BuildParam(Method.FindAllBy.ToString(), binder.Name, args));
            else if (binder.Name.StartsWith(Method.FindBy.ToString()))
                result = Query.FindOneByDynamic(connectionString, table, BuildParam(Method.FindBy.ToString(), binder.Name, args));
            else if (binder.Name.StartsWith(Method.FindAllIn.ToString()))
                result = Query.FindOneByDynamic(connectionString, table, BuildParam(Method.FindAllIn.ToString(), binder.Name, args));
            else if (binder.Name == Method.FindAll.ToString() || binder.Name == Method.All.ToString())
                result = Query.FindAllDynamic(connectionString, table);
            else
                return false;
            return true;
        }

        private IDictionary<string, object> BuildParam(string method, string binderName, object[] args)
        {
            IList<string> keys = binderName.Replace(method, "").Split(' ').Where(param => string.Compare(param, "AND", true) != 0).ToList();
            return args.Zip(keys, (value, key) => new KeyValuePair<string, object>(key, value)).ToDictionary(x => x.Key, x => x.Value);
        }

        private enum Method
        {
            All,
            FindAll,
            FindAllBy,
            FindBy,
            FindAllIn
        }
    }
}
