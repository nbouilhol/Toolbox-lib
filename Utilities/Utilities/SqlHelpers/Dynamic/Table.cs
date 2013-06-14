using System.Dynamic;

namespace Utilities.SqlHelpers
{
    public class Table : DynamicObject
    {
        private readonly string connectionString;
        private readonly string table;
        private readonly DynamicQuery dynamicQuery;

        public Table(string connectionString, string table)
        {
            this.connectionString = connectionString;
            this.table = table;
            this.dynamicQuery = new DynamicQuery(connectionString, table);
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
            if (binder == null || string.IsNullOrEmpty(binder.Name))
            {
                result = null;
                return false;
            }

            result = this.dynamicQuery.Execute(binder.Name.ToUpper(), args);
            return true;
        }
    }
}