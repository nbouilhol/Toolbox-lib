using System.Dynamic;

namespace Utilities.SqlHelpers.Dynamic
{
    public class Database : DynamicObject
    {
        private readonly string _connectionString;

        public Database(string connectionString)
        {
            _connectionString = connectionString;
        }

        public static dynamic OpenConnection(string connectionString)
        {
            return new Database(connectionString);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = new Table(_connectionString, binder.Name);
            return true;
        }
    }
}