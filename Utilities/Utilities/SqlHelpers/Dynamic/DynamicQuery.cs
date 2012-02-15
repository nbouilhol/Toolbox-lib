using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Utilities.Extensions;

namespace Utilities.SqlHelpers
{
    public class DynamicQuery
    {
        private const char separator = '|';
        private string connectionString;
        private string table;

        public DynamicQuery(string connectionString, string table)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(!string.IsNullOrEmpty(table));

            this.connectionString = connectionString;
            this.table = table;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(!string.IsNullOrEmpty(connectionString));
            Contract.Invariant(!string.IsNullOrEmpty(table));
        }

        public bool IsQuery(string name)
        {
            return Enum.GetNames(typeof(Method)).Any(method => name.StartsWith(method));
        }

        public object Execute(string name, object[] args)
        {
            Contract.Requires(!string.IsNullOrEmpty(name));

            if (name.StartsWith(Method.FINDALLBY.ToString()))
                return FindAllBy(name, args);
            else if (name.StartsWith(Method.FINDBY.ToString()))
                return FindBy(name, args);
            else if (name == Method.FINDALL.ToString() || name == Method.ALL.ToString())
                return FindAll();
            else
                return false;
        }

        private ICollection<dynamic> FindAll()
        {
            return Query.FindAllDynamic(connectionString, table);
        }

        private dynamic FindBy(string name, object[] args)
        {
            return Query.FindOneByDynamic(connectionString, table, BuildWhere(Method.FINDBY.ToString(), name, args), args != null ? ExtractParams(args) : null);
        }

        private ICollection<dynamic> FindAllBy(string name, object[] args)
        {
            return Query.FindAllByDynamic(connectionString, table, BuildWhere(Method.FINDALLBY.ToString(), name, args), args != null ? ExtractParams(args) : null);
        }

        private static string BuildWhere(string method, string name, IEnumerable<object> args)
        {
            Contract.Requires(name != null);

            IEnumerable<string> propertiesOperators = ExtractProperties(method, name);
            IDictionary<string, string> propertiesRequests = args != null ? ExtractPropertiesWithRequests(method, name, args) : null;

            return string.Join(" ", propertiesOperators.Select(property => propertiesRequests.ContainsKey(property) ? propertiesRequests.TryGetValue(property) : property));
        }

        private static IDictionary<string, string> ExtractPropertiesWithRequests(string method, string name, IEnumerable<object> args)
        {
            Contract.Requires(!string.IsNullOrEmpty(method));
            Contract.Requires(!string.IsNullOrEmpty(name));
            Contract.Requires(args != null);

            return name.Replace(method, "")
                .Split(new string[] { Operator.AND.ToString(), Operator.OR.ToString() }, StringSplitOptions.RemoveEmptyEntries)
                .Zip(args, (property, arg) => new { property = property, arg = arg })
                .Select((x, index) => new { property = x.property, arg = x.arg, index = index })
                .ToDictionary(x => x.property, x => ConvertParamToRequest(x.index, x.property, x.arg));
        }

        private static string ConvertParamToRequest(int index, string property, object arg)
        {
            if (arg != null && arg is IEnumerable && IsArray(arg))
            {
                IEnumerable<object> arrayArg = (arg as IEnumerable).Cast<object>();
                if (arrayArg == null || arrayArg.Count() == 0) return null;
                return string.Format("{0} IN ({1})", property, string.Join(",", arrayArg.Select(a => FormatForIn(a))));
            }
            if (arg != null && IsBetween(arg))
                return string.Format("{0} BETWEEN @{1} AND @{2}", property, index, index + 1);
            return string.Format("{0}=@{1}", property, index);
        }

        private static string FormatForIn(object arg)
        {
            Contract.Requires(arg != null);

            return arg.GetType().Name == "String" ? string.Format("'{0}'", arg) : arg.ToString();
        }

        private static IDictionary<string, object> ExtractPropertiesWithParams(string method, string name, IEnumerable<object> args)
        {
            Contract.Requires(!string.IsNullOrEmpty(name));
            Contract.Requires(!string.IsNullOrEmpty(method));

            return name.Replace(method, "")
                .Split(new string[] { Operator.AND.ToString(), Operator.OR.ToString() }, StringSplitOptions.RemoveEmptyEntries)
                .Zip(args, (property, arg) => new { property = property, arg = arg })
                .ToDictionary(x => x.property, x => x.arg);
        }

        private static IEnumerable<string> ExtractProperties(string method, string name)
        {
            Contract.Requires(!string.IsNullOrEmpty(name));
            Contract.Requires(!string.IsNullOrEmpty(method));

            return name.Replace(method, "")
                .Replace(Operator.AND.ToString(), separator + Operator.AND.ToString() + separator)
                .Replace(Operator.OR.ToString(), separator + Operator.OR.ToString() + separator)
                .Split(separator);
        }

        private static object[] ExtractParams(IEnumerable<object> args)
        {
            Contract.Requires(args != null);

            return args.Where(arg => !IsArray(arg)).SelectMany(arg =>
            {
                return IsBetween(arg)
                    ? new object[] { ((dynamic)arg).Item1, ((dynamic)arg).Item2 }
                    : arg.AsEnumerable();
            }).ToArray();
        }

        private static bool IsBetween(object arg)
        {
            Contract.Requires(arg != null);

            return arg.GetType().Name == "Tuple`2";
        }

        private static bool IsArray(object arg)
        {
            Contract.Requires(arg != null);

            return arg.GetType().IsArray;
        }

        private enum Method
        {
            ALL,
            FINDALL,
            FINDALLBY,
            FINDBY
        }

        private enum Operator
        {
            AND,
            OR
        }
    }

    public static class DynamicQueryExtension
    {
        public static dynamic to<TParam>(this TParam start, TParam end)
        {
            return Tuple.Create<TParam, TParam>(start, end);
        }
    }
}
