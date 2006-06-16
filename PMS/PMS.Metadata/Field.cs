using System;
using System.Collections;
using System.Xml.Serialization;

namespace PMS.Metadata
{  
    public class Field
    {
        [XmlAttributeAttribute("name")] 
        public string Name;

        [XmlAttributeAttribute("column")] 
        public string Column;

        [XmlAttributeAttribute("db_type")] 
        public string DbType;

        [XmlAttributeAttribute("primarykey")] 
        public bool PrimaryKey = false;
        
        [XmlAttributeAttribute("ignore_default")]
        public bool IgnoreDefault = false;

        public Field()
        {
        }

        public Field(string name, string column, string dbType)
        {
            Name = name;
            Column = column;
            DbType = dbType;
        }
        
        public Field(string name, string column, string dbType, 
                     bool ignoreDefault, bool iskey)
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
