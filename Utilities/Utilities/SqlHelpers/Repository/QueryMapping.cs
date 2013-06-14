using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Utilities.SqlHelpers
{
    public static class QueryMapping
    {
        public static IEnumerable<IEnumerable<T>> MapForFlatten<T>(SqlDataReader reader,
            Func<SqlDataReader, ICollection<T>> mapper)
        {
            while (reader.HasRows)
            {
                while (reader.Read())
                {
                    ICollection<T> result = mapper(reader);
                    if (result != null)
                        yield return result;
                }
                reader.NextResult();
            }
        }

        public static IEnumerable<T> Map<T>(SqlDataReader reader, Func<SqlDataReader, T> mapper)
        {
            while (reader.HasRows)
            {
                while (reader.Read())
                {
                    T result = mapper(reader);
                    if (result != null)
                        yield return result;
                }
                reader.NextResult();
            }
        }
    }
}