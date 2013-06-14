using System;
using System.Collections.Generic;
using System.Data;

namespace Utilities.SqlHelpers
{
    internal static class SqlDbTypeEx
    {
        private static readonly Lazy<Dictionary<SqlDbType, Type>> Map = new Lazy<Dictionary<SqlDbType, Type>>(() => new Dictionary<SqlDbType, Type>
                                                                      {
                                                                            { SqlDbType.BigInt, typeof(long)},
                                                                            { SqlDbType.Binary, typeof(byte[]) },
                                                                            { SqlDbType.Bit, typeof(bool)},
                                                                            { SqlDbType.Char, typeof(string)},
                                                                            { SqlDbType.Date, typeof(DateTime)},
                                                                            { SqlDbType.DateTime, typeof(DateTime)},
                                                                            { SqlDbType.DateTime2, typeof(DateTime)},
                                                                            { SqlDbType.DateTimeOffset, typeof(DateTime)},
                                                                            { SqlDbType.Decimal, typeof(decimal)},
                                                                            { SqlDbType.Float, typeof(double)},
                                                                            { SqlDbType.Image, typeof(byte[])},
                                                                            { SqlDbType.Int, typeof(int)},
                                                                            { SqlDbType.Money, typeof(decimal) },
                                                                            {SqlDbType.NChar, typeof(string)},
                                                                            { SqlDbType.NText, typeof(string)},
                                                                            { SqlDbType.NVarChar, typeof(string)},
                                                                            { SqlDbType.Real, typeof(float)},
                                                                            { SqlDbType.SmallDateTime, typeof(DateTime)},
                                                                            { SqlDbType.SmallInt, typeof(short)},
                                                                            { SqlDbType.SmallMoney, typeof(decimal)},
                                                                            { SqlDbType.Structured, typeof(object)},
                                                                            { SqlDbType.Text, typeof(string)},
                                                                            { SqlDbType.Time, typeof(TimeSpan)},
                                                                            { SqlDbType.Timestamp, typeof(byte[])},
                                                                            { SqlDbType.TinyInt, typeof(byte)},
                                                                            { SqlDbType.Udt, typeof(object)},
                                                                            { SqlDbType.UniqueIdentifier, typeof(Guid)},
                                                                            { SqlDbType.VarBinary, typeof(byte[])},
                                                                            { SqlDbType.VarChar, typeof(string)},
                                                                            { SqlDbType.Variant, typeof(object)},
                                                                            { SqlDbType.Xml, typeof(string)}
                                                                      });

        private static readonly Lazy<Dictionary<Type, SqlDbType>> MapInverse = new Lazy<Dictionary<Type, SqlDbType>>(() => new Dictionary<Type, SqlDbType>
                                                                      {
                                                                            { typeof(long), SqlDbType.BigInt},
                                                                            { typeof(byte[]), SqlDbType.VarBinary },
                                                                            { typeof(bool), SqlDbType.Bit},
                                                                            { typeof(string), SqlDbType.NVarChar},
                                                                            { typeof(DateTime), SqlDbType.DateTime},
                                                                            { typeof(decimal), SqlDbType.Decimal},
                                                                            { typeof(double), SqlDbType.Float},
                                                                            { typeof(int), SqlDbType.Int},
                                                                            { typeof(float), SqlDbType.Real},
                                                                            { typeof(short), SqlDbType.SmallInt},
                                                                            { typeof(object), SqlDbType.Variant},
                                                                            { typeof(TimeSpan), SqlDbType.Time},
                                                                            { typeof(byte), SqlDbType.TinyInt},
                                                                            { typeof(Guid), SqlDbType.UniqueIdentifier}
                                                                      });

        public static Type ToClrType(this SqlDbType sqlDbType)
        {
            return Map.Value[sqlDbType];
        }

        public static SqlDbType ToSqlType(this Type type)
        {
            return MapInverse.Value[type];
        }
    }
}