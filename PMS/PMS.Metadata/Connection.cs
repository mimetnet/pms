using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

using PMS.Data;

namespace PMS.Metadata
{
    [Serializable]
    [XmlRoot("connection")]
    public sealed class Connection : IXmlSerializable
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger("PMS.Metadata.Connection");

#region Public Properties
        public IProvider Provider;
        public string Value;
        public string ID;
        public bool IsDefault;
        public int Minimum = 0;
        public int Maximum = 20;
#endregion

#region Constructors
        public Connection()
        {
        }

        public Connection(XmlReader reader)
        {
            this.ReadXml(reader);
        }

        public Connection(string id, string conn, IProvider provider)
            : this(id, conn, provider, true, -1, -1)
        {
        }

        public Connection(string id, string conn, IProvider provider, bool isDefault)
            : this(id, conn, provider, isDefault, -1, -1)
        {
        }

        public Connection(string id, string conn, IProvider provider, bool isDefault, int minPool, int maxPool)
        {
            ID = id;
            Provider = provider;
            Value = conn;
            IsDefault = isDefault;

            if (0 < minPool)
                Minimum = minPool;

            if (0 < maxPool)
                Maximum = maxPool;

            if (Minimum >= Maximum)
                throw new ArgumentException("minPool isn't greater than maxPool");
        }
#endregion

        public bool IsValid {
            get {
                if (String.IsNullOrEmpty(this.Value))
                    return false;

                if (this.Provider == null)
                    return false;

                return true;
            }
        }

#region ObjectOverloads

        ///<summary>
        ///OverLoading == operator
        ///</summary> 
        public static bool operator ==(Connection obj1, Connection obj2)
        {
            if (Object.ReferenceEquals(obj1, obj2)) return true;
            if (Object.ReferenceEquals(obj1, null)) return false;
            if (Object.ReferenceEquals(obj2, null)) return false;

            if (obj1.ID != obj2.ID)
                return false;

            if (obj1.Provider != obj2.Provider)
                return false;

            if (obj1.Value != obj2.Value)
                return false;

            if (obj1.IsDefault != obj2.IsDefault)
                return false;

            if (obj1.Minimum != obj2.Minimum)
                return false;

            if (obj1.Maximum != obj2.Maximum)
                return false;

            return true;
        }

        ///<summary>
        ///OverLoading != operator
        ///</summary> 
        public static bool operator !=(Connection obj1, Connection obj2)
        {
            return !(obj1 == obj2);
        }

        ///<summary>
        ///Overriden Equals
        ///</summary> 
        public override bool Equals(object obj)
        {
            if (!(obj is Connection)) return false;

            return this == (Connection)obj;
        }

        ///<summary>
        ///ToString()
        ///</summary> 
        public override string ToString()
        {
            return String.Format("[ Connection (Id={0}) (Provider={1}) (Value={2}) (IsDefault={3}) (Pool={4}/{5} ]", ID, Provider, Value, IsDefault, Minimum, Maximum);
        }

        ///<summary>
        ///Overriden GetHashCode()
        ///</summary> 
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

#endregion

#region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            if (reader.LocalName != "connection")
                return;

            this.ID = reader.GetAttribute("id");

            try {
                this.Provider = PMS.Data.ProviderFactory.Create(reader.GetAttribute("provider"));
            } catch (ProviderNotFoundException e1) {
                log.WarnFormat("<connection id='{0}' />: {1}", this.ID, e1.Message);
            } catch (ArgumentNullException) {
                log.WarnFormat("<connection id=\"{0}\"/> has no @provider attribute", this.ID);
            } catch (Exception e2) {
                log.Debug("ReadXml: ", e2);
            }

            Int32.TryParse(reader.GetAttribute("pool-min"), out this.Minimum);
            Int32.TryParse(reader.GetAttribute("pool-max"), out this.Maximum);
            Boolean.TryParse(reader.GetAttribute("default"), out this.IsDefault);

            if (reader.IsEmptyElement == false) {
                this.Value = reader.ReadElementContentAsString();
            } else {
                reader.Read();
            }
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            if (String.IsNullOrEmpty(ID) == false)
                writer.WriteAttributeString("id", ID);

            if (Minimum > 0)
                writer.WriteAttributeString("pool-min", Minimum.ToString());

            if (Maximum > 0)
                writer.WriteAttributeString("pool-max", Maximum.ToString());

            if (IsDefault)
                writer.WriteAttributeString("default", "true");

            if (Provider != null)
                writer.WriteAttributeString("provider", Provider.Name);

            if (String.IsNullOrEmpty(Value) == false)
                writer.WriteString(Value);
        }

#endregion
    }
}
