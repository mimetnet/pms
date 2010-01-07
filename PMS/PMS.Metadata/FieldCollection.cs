using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace PMS.Metadata
{
    [Serializable]
    [XmlRoot("fields")]
    public class FieldCollection : ICollection<Field>, IList<Field>, IEnumerable<Field>, IXmlSerializable
    {
        private SortedList<String, Field> list = new SortedList<String, Field>(StringComparer.Ordinal);

        public FieldCollection()
        {
        }

        public Field this[string name] {
            get {
                if (!String.IsNullOrEmpty(name)) {
                    IList<Field> values = this.list.Values;

                    foreach (Field f in values)
                        if (0 == StringComparer.InvariantCulture.Compare(f.Name, name))
                            return f;
                    foreach (Field f in values)
                        if (0 == StringComparer.InvariantCulture.Compare(f.Column, name))
                            return f;
                }

                return null;
            }
            set { this.list[value.Name] = value; }
        }

#region ICollection<Field> Members
        public void Add(Field item)
        {
            this[item.Name] = item;
        }

        public void Clear()
        {
            this.list.Clear();
        }

        public bool Contains(Field item)
        {
            return this.list.ContainsKey(item.Name);
        }

        public void CopyTo(Field[] array, int arrayIndex)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int Count {
            get { return this.list.Count; }
        }

        public bool IsReadOnly {
            get { return false; }
        }

        public bool Remove(Field item)
        {
            return this.list.Remove(item.Name);
        }
#endregion

#region IList<Field> Members
        public int IndexOf(Field item)
        {
            return this.list.IndexOfKey(item.Name);
        }

        public void Insert(int index, Field item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            this.list.RemoveAt(index);
        }

        public Field this[int i] {
            get { return this.list.Values[i]; }
            set { this.list.Values[i] = value; }
        }
#endregion

#region IEnumerable<Field> Members
        public IEnumerator<Field> GetEnumerator()
        {
            foreach (KeyValuePair<String, Field> kv in this.list) {
                yield return kv.Value;
            }
        }
#endregion

#region IEnumerable Members
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (KeyValuePair<String, Field> kv in this.list) {
                yield return kv.Value;
            }
        }
#endregion

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader.LocalName != "fields")
                throw new InvalidOperationException("ReadXml expected <fields/>, but found <" + reader.LocalName + "/> " + reader.LocalName);

            if (reader.IsEmptyElement)
                return;

            //Console.WriteLine("Fields.Enter: {0} {1}", reader.LocalName, reader.NodeType);

            XmlSerializer xml = new XmlSerializer(typeof(Field));
            if (reader.ReadToDescendant("field")) {
                do {
                    //Console.WriteLine("Fields.Middle(1): {0} {1}", reader.LocalName, reader.NodeType);
                    this.Add((Field) xml.Deserialize(reader));
                    //Console.WriteLine("Fields.Middle(2): {0} {1}\n", reader.LocalName, reader.NodeType);
                } while (reader.ReadToNextSibling("field"));
            }

            //Console.WriteLine("Fields.Exit: {0} {1}\n", reader.LocalName, reader.NodeType);
        }

        public void WriteXml(XmlWriter writer)
        {
            XmlSerializer xml = new XmlSerializer(typeof(Field));

            foreach (Field f in this)
                xml.Serialize(writer, f);
        }
    }
}
