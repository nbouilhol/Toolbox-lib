using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;

namespace Utilities.SqlHelpers.Mapper
{
    public class ObjectDataReader<TData> : IDataReader
    {
        private readonly PropertyAccessor<TData> _propertyAccessor;
        private readonly IEnumerator<TData> _dataEnumerator;

        public ObjectDataReader(IEnumerable<TData> data)
        {
            Contract.Requires(data != null);

            _propertyAccessor = PropertyAccessor<TData>.Create();
            _dataEnumerator = data.GetEnumerator();
        }

        #region IDataReader Members

        public void Close()
        {
            Dispose();
        }

        public int Depth
        {
            get { return 1; }
        }

        public DataTable GetSchemaTable()
        {
            return null;
        }

        public bool IsClosed
        {
            get { return _dataEnumerator == null; }
        }

        public bool NextResult()
        {
            return false;
        }

        public bool Read()
        {
            if (_dataEnumerator == null) throw new ObjectDisposedException("ObjectDataReader");

            return _dataEnumerator.MoveNext();
        }

        public int RecordsAffected
        {
            get { return -1; }
        }

        #endregion IDataReader Members

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing) if (_dataEnumerator != null) _dataEnumerator.Dispose();
        }

        #endregion IDisposable Members

        #region IDataRecord Members

        public int FieldCount
        {
            get { return _propertyAccessor.Accessors.Length; }
        }

        public bool GetBoolean(int i)
        {
            throw new NotImplementedException();
        }

        public byte GetByte(int i)
        {
            throw new NotImplementedException();
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            throw new NotImplementedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            throw new NotImplementedException();
        }

        public decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        public double GetDouble(int i)
        {
            throw new NotImplementedException();
        }

        public Type GetFieldType(int i)
        {
            throw new NotImplementedException();
        }

        public float GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(int i)
        {
            throw new NotImplementedException();
        }

        public int GetInt32(int i)
        {
            throw new NotImplementedException();
        }

        public long GetInt64(int i)
        {
            throw new NotImplementedException();
        }

        public string GetName(int i)
        {
            throw new NotImplementedException();
        }

        public int GetOrdinal(string name)
        {
            int ordinal;

            if (!_propertyAccessor.OrdinalLookup.TryGetValue(name, out ordinal)) throw new InvalidOperationException("Unknown parameter name " + name);

            return ordinal;
        }

        public string GetString(int i)
        {
            throw new NotImplementedException();
        }

        public object GetValue(int i)
        {
            if (_dataEnumerator == null) throw new ObjectDisposedException("ObjectDataReader");
            return i < _propertyAccessor.Accessors.Length ? _propertyAccessor.Accessors[i](_dataEnumerator.Current) : null;
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public bool IsDBNull(int i)
        {
            throw new NotImplementedException();
        }

        public object this[string name]
        {
            get { throw new NotImplementedException(); }
        }

        public object this[int i]
        {
            get { throw new NotImplementedException(); }
        }

        #endregion IDataRecord Members
    }
}