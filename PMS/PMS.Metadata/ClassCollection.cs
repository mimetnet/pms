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
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger("PMS.Metadata.ClassCollection");

		private SortedList<Type, Class> list = new SortedList<Type, Class>(new TypeComparer());
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

				listLock.AcquireReaderLock(1000);
				this.list.TryGetValue(type, out klass);
				listLock.ReleaseReaderLock();

				return klass;
			}
			set { throw new NotSupportedException(); }
		}

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
			return null;
        }

        public void ReadXml(XmlReader reader)
        {
            Class klass = null;
            XmlSerializer xml = new XmlSerializer(typeof(Class));

			try {
				listLock.AcquireWriterLock(1000);

				while (reader.Read()) {
					reader.MoveToElement();

					if (reader.LocalName == "class") {
						try {
							if ((klass = (Class)xml.Deserialize(reader)) != null) {
								if (klass.Type != null) {
									this.list.Add(klass.Type, klass);
								} else {
									log.WarnFormat("Class.table {0}'s Type failed to load", klass.Table);
								}
							}
						} catch (Exception) {}
					} else if (reader.LocalName == "classes") {
						break;
					}
				}
			} catch (Exception e) {
				log.Error("ReadXml: ", e);
			} finally {
				listLock.ReleaseWriterLock();
			}
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
			listLock.AcquireWriterLock(1000);

			if (this.list.ContainsKey(item.Type) == false) {
				this.list.Add(item.Type, item);
			}

			listLock.ReleaseWriterLock();
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
			get
			{
				return this.list.Values[index];
			}
			set
			{
				Class tmp = this.list.Values[index];
				if (tmp != null) {
					throw new IndexOutOfRangeException("Index retrieved no Class, this can be used for updates, not insertions!");
				}

				if (tmp.Type != value.Type) {
					throw new Exception("Class.Type must equal value.Type in order to update");
				}

				this.list[value.Type] = value;
			}
		}

		#endregion

		class TypeComparer : IComparer<Type>
		{
			#region IComparer<Type> Members

			public int Compare(Type x, Type y)
			{
				if (x == null || y == null) {
					throw new NullReferenceException();
				}

				return x.ToString().CompareTo(y.ToString());
			}

			#endregion
		}
	}
}
