using System;
using System.Collections;
using System.Xml.Serialization;

namespace PMS.Metadata
{  
    public class Field
    {
        public string Name;
        public string Column;
        public string DbType;
        public bool PrimaryKey = false;
        public bool IgnoreDefault = false;

        public Field()
        {
        }

        public Field(string name, string column, string dbType)
            : this(name, column, dbType, false, false)
        {
        }

        public Field(string name, string column, string dbType, bool ignoreDefault)
            : this(name, column, dbType, ignoreDefault, false)
        {
        }
        
        public Field(string name, string column, string dbType, bool ignoreDefault, bool iskey)
        {
            Name = name;
            Column = column;
            DbType = dbType;
            PrimaryKey = iskey;
            IgnoreDefault = ignoreDefault;
        }

        #region ObjectOverloads

        ///<summary>
        ///OverLoading == operator
        ///</summary> 
        public static bool operator ==(Field obj1, Field obj2)
        {
            if (Object.ReferenceEquals(obj1, obj2)) return true;
            if (Object.ReferenceEquals(obj1, null)) return false;
            if (Object.ReferenceEquals(obj2, null)) return false;

            if (obj1.Name != obj2.Name) return false;
            if (obj1.Column != obj2.Column) return false;
            if (obj1.DbType != obj2.DbType) return false;
            if (obj1.PrimaryKey != obj2.PrimaryKey) return false;
            if (obj1.IgnoreDefault != obj2.IgnoreDefault) return false;

            return true;
        }

        ///<summary>
        ///OverLoading != operator
        ///</summary> 
        public static bool operator !=(Field obj1, Field obj2)
        {
            return !(obj1 == obj2);
        }

        ///<summary>
        ///Overriden Equals
        ///</summary> 
        public override bool Equals(object obj)
        {
            if (!(obj is Field)) return false;

            return this == (Field)obj;
        }

        ///<summary>
        ///ToString()
        ///</summary> 
        public override string ToString()
        {
            return String.Format("[Field (Name={0}) (Column={1}) (DbType={2}) (PrimaryKey={3}) (IgnoreDefault={4}) (Type={5})]", Name, Column, DbType, PrimaryKey, IgnoreDefault);
        }

        ///<summary>
        ///Overriden GetHashCode()
        ///</summary> 
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        #endregion
    }
}
