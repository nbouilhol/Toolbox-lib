using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Utilities.Extensions;
using Utilities.SqlHelpers.Repository;

namespace Utilities.SqlHelpers.Dynamic
{
    public class DynamicQuery
    {
        private const char Separator = '|';
        private readonly string _connectionString;
        private readonly string _table;

        public DynamicQuery(string connectionString, string table)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(!string.IsNullOrEmpty(table));

            _connectionString = connectionString;
            _table = table;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(!string.IsNullOrEmpty(_connectionString));
            Contract.Invariant(!string.IsNullOrEmpty(_table));
        }

        public bool IsQuery(string name)
        {
            return Enum.GetNames(typeof (Method)).Any(name.StartsWith);
        }

        public object Execute(string name, object[] args)
        {
            Contract.Requires(!string.IsNullOrEmpty(name));

            if (name.StartsWith(Method.Findallby.ToString()))
                return FindAllBy(name, args);
            if (name.StartsWith(Method.Findby.ToString()))
                return FindBy(name, args);
            if (name == Method.Findall.ToString() || name == Method.All.ToString())
                return FindAll();
            return false;
        }

        private ICollection<dynamic> FindAll()
        {
            return Query.FindAllDynamic(_connectionString, _table);
        }

        private dynamic FindBy(string name, object[] args)
        {
            return Query.FindOneByDynamic(_connectionString, _table, BuildWhere(Method.Findby.ToString(), name, args),
                args != null ? ExtractParams(args) : null);
        }

        private ICollection<dynamic> FindAllBy(string name, object[] args)
        {
            return Query.FindAllByDynamic(_connectionString, _table, BuildWhere(Method.Findallby.ToString(), name, args),
                args != null ? ExtractParams(args) : null);
        }

        private static string BuildWhere(string method, string name, IEnumerable<object> args)
        {
            Contract.Requires(name != null);

            IEnumerable<string> propertiesOperators = ExtractProperties(method, name);
            IDictionary<string, string> propertiesRequests = args != null
                ? ExtractPropertiesWithRequests(method, name, args)
                : null;

            return string.Join(" ",
                propertiesOperators.Select(
                    property =>
                        propertiesRequests != null && propertiesRequests.ContainsKey(property)
                            ? propertiesRequests.TryGetValue(property)
                            : property));
        }

        private static IDictionary<string, string> ExtractPropertiesWithRequests(string method, string name,
            IEnumerable<object> args)
        {
            Contract.Requires(!string.IsNullOrEmpty(method));
            Contract.Requires(!string.IsNullOrEmpty(name));
            Contract.Requires(args != null);

            return name.Replace(method, "")
                .Split(new[] {Operator.And.ToString(), Operator.Or.ToString()}, StringSplitOptions.RemoveEmptyEntries)
                .Zip(args, (property, arg) => new {property, arg})
                .Select((x, index) => new {x.property, x.arg, index})
                .ToDictionary(x => x.property, x => ConvertParamToRequest(x.index, x.property, x.arg));
        }

        private static string ConvertParamToRequest(int index, string property, object arg)
        {
            if (arg is IEnumerable && IsArray(arg))
            {
                IEnumerable<object> arrayArg = (arg as IEnumerable).Cast<object>();
                IEnumerable<object> enumerable = arrayArg as IList<object> ?? arrayArg.ToList();
                if (enumerable.Any()) return null;
                return string.Format("{0} IN ({1})", property, string.Join(",", enumerable.Select(FormatForIn)));
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

/*
        private static IDictionary<string, object> ExtractPropertiesWithParams(string method, string name, IEnumerable<object> args)
        {
            Contract.Requires(!string.IsNullOrEmpty(name));
            Contract.Requires(!string.IsNullOrEmpty(method));

            return name.Replace(method, "")
                .Split(new[] { Operator.And.ToString(), Operator.Or.ToString() }, StringSplitOptions.RemoveEmptyEntries)
                .Zip(args, (property, arg) => new {property, arg })
                .ToDictionary(x => x.property, x => x.arg);
        }
*/

        private static IEnumerable<string> ExtractProperties(string method, string name)
        {
            Contract.Requires(!string.IsNullOrEmpty(name));
            Contract.Requires(!string.IsNullOrEmpty(method));

            return name.Replace(method, "")
                .Replace(Operator.And.ToString(), Separator + Operator.And.ToString() + Separator)
                .Replace(Operator.Or.ToString(), Separator + Operator.Or.ToString() + Separator)
                .Split(Separator);
        }

        private static object[] ExtractParams(IEnumerable<object> args)
        {
            Contract.Requires(args != null);

            return args.Where(arg => !IsArray(arg)).SelectMany(arg => IsBetween(arg)
                ? new object[] {((dynamic) arg).Item1, ((dynamic) arg).Item2}
                : arg.AsEnumerable()).ToArray();
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
            All,
            Findall,
            Findallby,
            Findby
        }

        private enum Operator
        {
            And,
            Or
        }
    }

    public static class DynamicQueryExtension
    {
        public static dynamic To<TParam>(this TParam start, TParam end)
        {
            return Tuple.Create(start, end);
        }
    }
}