using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Utilities.SqlHelpers
{
    public class BulkCopy<T> where T : class
    {
        private readonly int batchSize;
        private readonly string connectionString;
        private readonly string destinationTableName;

        public BulkCopy(string connectionString, string destinationTableName, int batchSize = 0)
        {
            this.connectionString = connectionString;
            this.destinationTableName = destinationTableName;
            this.batchSize = batchSize;
        }

        public virtual void WriteToServer(IList<T> items)
        {
            WriteToServer(items, SqlBulkCopyOptions.Default);
        }

        public virtual void WriteToServer(IList<T> items, SqlBulkCopyOptions options)
        {
            WriteToServer(items.ToDataTable(PropertiesDescriptor<T>.Create()), options);
        }

        public virtual void WriteToServer(IList<T> items, SqlBulkCopyOptions options,
            IEnumerable<SqlBulkCopyColumnMapping> columnMappings)
        {
            WriteToServer(items.ToDataTable(PropertiesDescriptor<T>.Create()), options, columnMappings);
        }

        public void WriteToServer(DataTable dataTable)
        {
            WriteToServer(dataTable, SqlBulkCopyOptions.Default);
        }

        public void WriteToServer(DataTable dataTable, SqlBulkCopyOptions options)
        {
            IEnumerable<SqlBulkCopyColumnMapping> columnMappings =
                dataTable.Columns.Cast<DataColumn>()
                    .Select(column => new SqlBulkCopyColumnMapping(column.ColumnName, column.ColumnName));
            WriteToServer(dataTable, options, columnMappings);
        }

        public void WriteToServer(DataTable dataTable, SqlBulkCopyOptions options,
            IEnumerable<SqlBulkCopyColumnMapping> columnMappings)
        {
            if (dataTable == null || dataTable.Columns.Count == 0) return;
            if (string.IsNullOrWhiteSpace(destinationTableName))
                throw new ArgumentException("destinationTableName cannot be null or empty");

            using (var sqlConnexion = new SqlConnection(connectionString))
            using (var bulkCopy = new SqlBulkCopy(connectionString, options))
            {
                bulkCopy.DestinationTableName = destinationTableName;
                if (batchSize != 0) bulkCopy.BatchSize = batchSize;
                if (sqlConnexion.State == ConnectionState.Closed) sqlConnexion.Open();
                foreach (SqlBulkCopyColumnMapping mapping in columnMappings) bulkCopy.ColumnMappings.Add(mapping);
                bulkCopy.WriteToServer(dataTable);
            }
        }
    }
}