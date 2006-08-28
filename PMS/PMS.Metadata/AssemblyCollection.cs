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
        public string this[int index]
        {
            get { return (string)this.List[index]; }
            set { this.List[index] = value; }
        }

        ///<summary>
        /// The zero-based index of the element to get or set.
        ///</summary>
        public string this[string assemblyName] 
        {
            get { 
                foreach (string f in this.List)
                    if (f == assemblyName)
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
        public void Add(string assemblyName)
        {
            this.List.Add(assemblyName);
        }

        ///<summary>
        /// Adds an item to the AssemblyCollection.
        ///</summary>
        public void Add(string[] assemblyNames)
        {
            for (int x = 0; x < assemblyNames.Length; x++)
                this.Add(assemblyNames[x]);
        }

        ///<summary>
        /// Removes the first occurrence of a specific object from the AssemblyCollection.
        ///</summary>
        public void Remove(string assemblyName)
        {
            this.List.Remove(assemblyName);
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
        public void Insert(int index, string assemblyName)
        {
            this.List.Insert(index, assemblyName);
        }

        ///<summary>
        /// Determines the index of a specific item in the AssemblyCollection.
        ///</summary>
        public int IndexOf(string assemblyName)
        {
            return this.List.IndexOf(assemblyName);
        }

        ///<summary>
        /// Determines whether the AssemblyCollection contains a specific value.
        ///</summary>
        public bool Contains(string assemblyName)
        {
            return this.List.Contains(assemblyName);
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
			return null;
        }

        public void ReadXml(XmlReader reader)
        {
            Assembly assembly;
            string sAssembly;

            while (reader.Read()) {
                reader.MoveToElement();

                if (reader.LocalName == "add") {
                    sAssembly = reader["assembly"];
                    if (sAssembly != null && sAssembly != String.Empty) {
                        if (!IsLoaded(sAssembly)) {
                            try {
								assembly = Assembly.LoadWithPartialName(sAssembly);
								log.Debug("Assembly.Load: " + assembly.FullName);
                            } catch (Exception e) {
                                log.Error("Assembly.Load: " + e.Message);
                            }
                        } else {
							log.Debug("Already.Loaded: " + sAssembly);
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
            foreach (string ass in this.List) {
                writer.WriteStartElement("add");
                writer.WriteAttributeString("assembly", ass);
                writer.WriteEndElement();
            }
        }

        #endregion
    }
}
