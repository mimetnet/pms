using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Xml.Serialization;
using System.Threading;

namespace PMS.Metadata
{
    /// <summary>
    /// ClassCollection Class
    /// </summary>
    [XmlRoot("classes")]
    public class ClassCollection : ICollection<Class>, IList<Class>, IEnumerable<Class>, IXmlSerializable
    {
        //private static readonly log4net.ILog log =
        //    log4net.LogManager.GetLogger("PMS.Metadata.ClassCollection");

        private SortedList<Type, Class> list = new SortedList<Type, Class>(new PMS.Util.TypeComparer());
        private static ReaderWriterLock listLock = new ReaderWriterLock();

        ///<summary>
        /// Default constructor.
        ///</summary>
        public ClassCollection()
        {
        }

        public Class this[Type type]
        {
            get {
                Class klass = null;

                try {
                    listLock.AcquireReaderLock(2000);
                    this.list.TryGetValue(type, out klass);
                } finally {
                    listLock.ReleaseReaderLock();
                }

                return klass;
            }
            set {
                if (null == value)
                    throw new ArgumentNullException("value must be a Class instance");

                if (value.Type != type)
                    throw new ArgumentException(String.Format("type({0}) != value.Type({1})", type, value.Type));

                try {
                    listLock.AcquireWriterLock(2000);
                    this.list[type] = value;
                } finally {
                    listLock.ReleaseWriterLock();
                }
            }
        }

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
			return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader.LocalName != "classes")
                throw new InvalidOperationException("ReadXml expected <classes/>, but found <" + reader.LocalName + "/> " + reader.LocalName);

            if (reader.IsEmptyElement)
                return;

            //Console.WriteLine("Classes.Enter: {0} {1}", reader.LocalName, reader.NodeType);

            try {
                listLock.AcquireWriterLock(2000);

                XmlSerializer xml = new XmlSerializer(typeof(Class));
                if (reader.ReadToDescendant("class")) {
                    do {
                        //Console.WriteLine("Classes.Middle(1): {0} {1}", reader.LocalName, reader.NodeType);
                        Class k = xml.Deserialize(reader) as Class;
                        if (null != k && null != k.Type)
                            this.Add(k);
                        //Console.WriteLine("Classes.Middle(2): {0} {1}\n", reader.LocalName, reader.NodeType);
#if NET_2_0
                    } while (reader.ReadToNextSibling("class"));
#else
                    } while (reader.NodeType == XmlNodeType.Element && reader.LocalName == "class");
#endif
                }
            } finally {
                listLock.ReleaseWriterLock();
            }

            //Console.WriteLine("Classes.Exit: {0} {1}\n", reader.LocalName, reader.NodeType);
        }

        public void WriteXml(XmlWriter writer)
        {
            XmlSerializer xml = new XmlSerializer(typeof(Class));

            foreach (KeyValuePair<Type, Class> kv in this.list)
                xml.Serialize(writer, kv.Value);
        }

        #endregion

		#region ICollection<Class> Members

		public void Add(Class item)
		{
			this[item.Type] = item;
		}

		public void Clear()
		{
			this.list.Clear();
		}

		public bool Contains(Class item)
		{
			return this.list.ContainsKey(item.Type);
		}

		public void CopyTo(Class[] array, int arrayIndex)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public int Count
		{
			get { return this.list.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(Class item)
		{
			return this.list.Remove(item.Type);
		}

		#endregion

		#region IEnumerable<Class> Members

		public IEnumerator<Class> GetEnumerator()
		{
			foreach (KeyValuePair<Type, Class> kv in this.list) {
				yield return kv.Value;
			}
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			foreach (KeyValuePair<Type, Class> kv in this.list) {
				yield return kv.Value;
			}
		}

		#endregion

		#region IList<Class> Members

		public int IndexOf(Class item)
		{
			return this.list.IndexOfKey(item.Type);
		}

		public void Insert(int index, Class item)
		{
			throw new NotSupportedException();
		}

		public void RemoveAt(int index)
		{
			this.list.RemoveAt(index);
		}

		public Class this[int index]
		{
			get {
				Class klass = null;

				listLock.AcquireReaderLock(2000);
				klass = this.list.Values[index];
				listLock.ReleaseReaderLock();

				return klass;
			}
			set {
				listLock.AcquireWriterLock(2000);

				Class tmp = this.list.Values[index];

				if (tmp != null) {
					listLock.ReleaseWriterLock();
					throw new IndexOutOfRangeException("Index retrieved no Class, this can be used for updates, not insertions!");
				}

				if (tmp.Type != value.Type) {
					listLock.ReleaseWriterLock();
					throw new Exception("Class.Type must equal value.Type in order to update");
				}

				this.list[value.Type] = value;

				listLock.ReleaseWriterLock();
			}
		}

		#endregion
	}
}
