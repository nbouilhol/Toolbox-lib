using System.Dynamic;

namespace Utilities.SqlHelpers
{
    public class Database : DynamicObject
    {
        private readonly string connectionString;

        public Database(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public static dynamic OpenConnection(string connectionString)
        {
            return new Database(connectionString);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (binder == null) { result = null; return false; }
            result = new Table(connectionString, binder.Name);
            return true;
        }
    }
}