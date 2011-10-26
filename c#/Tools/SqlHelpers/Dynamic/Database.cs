using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;

namespace BouilholLib.Helper
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
            result = new Table(connectionString, binder.Name);
            return true;
        }
    }
}
