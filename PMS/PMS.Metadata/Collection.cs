using System.Collections;
using System.Xml.Serialization;

namespace PMS.Metadata
{  
    public class Collection
    {
        private string _name;
        private string _class_ref;
        private string _orderby;
        private string _sort;
        private string _field_ref;
        
        [XmlAttributeAttribute()]
        public string name {
            get { return _name; }
            set { _name = value;  }
        }
        
        [XmlAttributeAttribute()]
        public string class_ref {
            get { return _class_ref; }
            set { _class_ref = value; }
        }
        
        [XmlAttributeAttribute()]
        public string orderby {
            get { return _orderby; }
            set { _orderby = value; }
        }

        [XmlAttributeAttribute()]
        public string sort {
            get { return _sort; }
            set { _sort = value; }
        }
        
        [XmlElementAttribute("foreignkey")]
        public string field_ref {
            get { return _field_ref; }
            set { _field_ref = value; }
        }
    }
}
