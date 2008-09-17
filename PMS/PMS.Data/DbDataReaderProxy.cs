using System;
using System.Collections;
using System.Data;
using System.Threading;

namespace PMS.Data
{
    public class DbDataReaderProxy : IDataReader, IEnumerable
    {
        //private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private IDataReader reader = null;
		private DbConnectionProxy conn = null;

        internal DbDataReaderProxy(IDataReader reader, DbConnectionProxy conn)
        {
			this.conn = conn;
            this.reader = reader;
        }

        /* {{{ IDataReader Members */
		public bool IsClosed {
			get { return this.reader.IsClosed; }
		}

		public int RecordsAffected {
			get { return this.reader.RecordsAffected; }
		}

		public int Depth {
			get { return this.reader.Depth; }
		}

		public DataTable GetSchemaTable()
		{
			return this.reader.GetSchemaTable();
		}

		public bool NextResult()
		{
			return this.reader.NextResult();
		}

		public bool Read()
		{
			return this.reader.Read();
		}

		public void Close()
		{
			Dispose();
		}
		/*}}}*/

		public void Dispose()
		{    
			try {
				if (this.reader != null)
					this.reader.Close();
			} finally {
				if (this.conn != null) {
					this.conn.ReleaseLock();
					this.conn = null;
				}

				this.reader = null;
			}
		}

		/* {{{ IDataRecord Members */
		public Int32 FieldCount {
			get { return this.reader.FieldCount; }
		}

		public String GetName(Int32 i)
		{
			return this.reader.GetName(i);
		}

		public Int32 GetOrdinal(String name)
		{
			return this.reader.GetOrdinal(name);
		}

		public Object this[int i] {
			get { return this.reader[i]; }
		}

		public Object this[string i] {
			get { return this.reader[i]; }
		}

		public Boolean GetBoolean(Int32 i)
		{
			return this.reader.GetBoolean(i);
		}

		public Byte GetByte(Int32 i)
		{
			return this.reader.GetByte(i);
		}

		public Int64 GetBytes(Int32 i, Int64 fOffset, Byte[] buffer, Int32 bOffset, Int32 length)
		{
			return this.reader.GetBytes(i, fOffset, buffer, bOffset, length);
		}

		public Char GetChar(Int32 i)
		{
			return this.reader.GetChar(i);
		}

		public Int64 GetChars(Int32 i, Int64 fOffset, Char[] buffer, Int32 bOffset, Int32 length)
		{
			return this.reader.GetChars(i, fOffset, buffer, bOffset, length);
		}

		public Guid GetGuid(Int32 i)
		{
			return this.reader.GetGuid(i);
		}

		public Int16 GetInt16(Int32 i)
		{
			return this.reader.GetInt16(i);
		}

		public Int32 GetInt32(Int32 i)
		{
			return this.reader.GetInt32(i);
		}
	
		public Int64 GetInt64(Int32 i)
		{
			return this.reader.GetInt64(i);
		}

		public Single GetFloat(Int32 i)
		{
			return this.reader.GetFloat(i);
		}

		public Double GetDouble(Int32 i)
		{
			return this.reader.GetDouble(i);
		}

		public String GetString(Int32 i)
		{
			return this.reader.GetString(i);
		}

		public Decimal GetDecimal(Int32 i)
		{
			return this.reader.GetDecimal(i);
		}

		public DateTime GetDateTime(Int32 i)
		{
			return this.reader.GetDateTime(i);
		}

		public IDataReader GetData(Int32 i)
		{
			return this.reader.GetData(i);
		}

		public Boolean IsDBNull(Int32 i)
		{
			return this.reader.IsDBNull(i);
		}

		public String GetDataTypeName(Int32 i)
		{
			return this.reader.GetDataTypeName(i);
		}

		public Type GetFieldType(Int32 i)
		{
			return this.reader.GetFieldType(i);
		}

		public Object GetValue(Int32 i)
		{
			return this.reader.GetValue(i);
		}

		public Int32 GetValues(Object[] values)
		{
			return this.reader.GetValues(values);
		}
		/* }}} */

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new System.Data.Common.DbEnumerator(this);
		}
	}
}
// vim:foldmethod=marker:foldlevel=0:
