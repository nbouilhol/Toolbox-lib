using System;
using System.Data;
using System.Dynamic;

namespace Utilities.SqlHelpers
{
    public class DynamicDataRow : DynamicObject
    {
        private DataRow dataRow;

        public DynamicDataRow(DataRow dataRow)
        {
            this.dataRow = dataRow;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;

            try
            {
                result = dataRow[binder.Name];

                if (result == DBNull.Value)
                    result = null;

                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            try
            {
                if (value == null)
                    value = DBNull.Value;

                dataRow[binder.Name] = value;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}