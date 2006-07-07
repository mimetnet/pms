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

        public override string ToString()
        {
            return "\nField:\n  Name: " + Name +
                "\n  Column: " + Column +
                "\n  Type: " + DbType +
                "\n  Primary: " + PrimaryKey +
                "\n  IgnoreDefault: " + IgnoreDefault;
        }
    }
}
