using System.Dynamic;

namespace Utilities.SqlHelpers.Dynamic
{
    public class Table : DynamicObject
    {
        private readonly DynamicQuery _dynamicQuery;

        public Table(string connectionString, string table)
        {
            _dynamicQuery = new DynamicQuery(connectionString, table);
        }

        public static dynamic FindBy(dynamic param)
        {
            return null;
        }

        public static dynamic FindAllBy(dynamic param)
        {
            return null;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (string.IsNullOrEmpty(binder.Name))
            {
                result = null;
                return false;
            }

            result = _dynamicQuery.Execute(binder.Name.ToUpper(), args);
            return true;
        }
    }
}