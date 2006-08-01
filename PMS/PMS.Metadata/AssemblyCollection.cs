using System;
using System.Collections;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace PMS.Metadata
{
    /// <summary>
    /// AssemblyCollection Class
    /// </summary>
    [XmlRoot("assemblies")]
    public class AssemblyCollection : CollectionBase, IXmlSerializable
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger("PMS.Metadata.AssemblyCollection");

        ///<summary>
        /// Default constructor.
        ///</summary>
        public AssemblyCollection() : base()
        {
        }

        #region CollectionBase Members
        ///<summary>
        /// The zero-based index of the element to get or set.
        ///</summary>
        public Assembly this[int index] {
            get { return (Assembly)this.List[index]; }
            set { this.List[index] = value; }
        }

        ///<summary>
        /// The zero-based index of the element to get or set.
        ///</summary>
        public Assembly this[string name] {
            get { 
                foreach (Assembly f in this.List)
                    if (f.GetName().Name == name)
                        return f;

                return null;
            }
        }

        ///<summary>
        /// Gets the number of elements contained in the AssemblyCollection.
        ///</summary>
        public new int Count
        {
            get { return this.List.Count; }
        }

        ///<summary>
        /// Removes all items from the AssemblyCollection.
        ///</summary>
        public new void Clear()
        {
            this.List.Clear();
        }

        ///<summary>
        /// Adds an item to the AssemblyCollection.
        ///</summary>
        public void Add(Assembly assembly)
        {
            this.List.Add(assembly);
        }

        ///<summary>
        /// Adds an item to the AssemblyCollection.
        ///</summary>
        public void Add(Assembly[] assemblies)
        {
            for (int x = 0; x < assemblies.Length; x++)
                this.Add(assemblies[x]);
        }

        ///<summary>
        /// Removes the first occurrence of a specific object from the AssemblyCollection.
        ///</summary>
        public void Remove(Assembly value)
        {
            this.List.Remove(value);
        }

        ///<summary>
        /// Removes the Assembly item at the specified index.
        ///</summary>
        public new void RemoveAt(int index)
        {
            this.List.RemoveAt(index);
        }

        ///<summary>
        /// Inserts an item to the AssemblyCollection at the specified index.
        ///</summary>
        public void Insert(int index, Assembly value)
        {
            this.List.Insert(index, value);
        }

        ///<summary>
        /// Determines the index of a specific item in the AssemblyCollection.
        ///</summary>
        public int IndexOf(Assembly value)
        {
            return this.List.IndexOf(value);
        }

        ///<summary>
        /// Determines whether the AssemblyCollection contains a specific value.
        ///</summary>
        public bool Contains(Assembly value)
        {
            return this.List.Contains(value);
        }

        ///<summary>
        /// Copies elements of the AssemblyCollection to a System.Array, 
        /// starting at a particular System.Array index.
        ///</summary>
        public void CopyTo(Array array, int index)
        {
            this.List.CopyTo(array, index);
        } 
        #endregion

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            string sAssembly;

            while (reader.Read()) {
                reader.MoveToElement();

                if (reader.LocalName == "add") {
                    sAssembly = reader["assembly"];
                    if (sAssembly != null && sAssembly != String.Empty) {
                        if (!sAssembly.EndsWith(".dll") && 
                            !sAssembly.EndsWith(".DLL") &&
                            !IsLoaded(sAssembly)) {
                            Assembly.Load(sAssembly);
                            log.Info("Loading " + sAssembly);
                        }

                    }
                }

                if (reader.LocalName == "assemblies")
                    break;
            }
        }

        private bool IsLoaded(string sAssembly)
        {
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                if (a.GetName().Name == sAssembly)
                    return true;

            return false;
        }

        public void WriteXml(XmlWriter writer)
        {
            string ass = null;

            foreach (Assembly a in this.List) {
                ass = (a.GlobalAssemblyCache) ? a.GetName().Name : a.Location;

                writer.WriteStartElement("add");
                writer.WriteAttributeString("assembly", ass);
                writer.WriteEndElement();
            }
        }

        #endregion
    }
}
