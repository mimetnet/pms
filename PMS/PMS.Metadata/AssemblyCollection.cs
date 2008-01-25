using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace PMS.Metadata
{
    /// <summary>
    /// AssemblyCollection Class
    /// </summary>
    [XmlRoot("assemblies")]
    public class AssemblyCollection : List<String>, IXmlSerializable
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger("PMS.Metadata.AssemblyCollection");

        ///<summary>
        /// Default constructor.
        ///</summary>
        public AssemblyCollection() : base()
        {
        }

        ///<summary>
        /// The zero-based index of the element to get or set.
        ///</summary>
        public string this[string assemblyName] 
        {
            get { 
                foreach (string f in this)
                    if (f == assemblyName)
                        return f;

                return null;
            }
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
			return null;
        }

        public void ReadXml(XmlReader reader)
        {
            string sAssembly;

            while (reader.Read()) {
                reader.MoveToElement();

				if (reader.LocalName == "add") {

					if (!reader.IsEmptyElement) {
						sAssembly = reader.ReadElementContentAsString();
						if (!String.IsNullOrEmpty(sAssembly)) {
							try {
								if (File.Exists(sAssembly)) {
									Assembly.LoadFile(sAssembly);
								} else {
									Assembly.Load(sAssembly);
								}
							} catch (Exception e) {
								log.Error("Load: " + e.Message + Environment.NewLine);
							}
						}
					} else {
						reader.Read();
					}
				}

				if (reader.LocalName == "assemblies" && reader.NodeType == XmlNodeType.EndElement)
                    break;
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (string ass in this) {
                writer.WriteElementString("add", ass);
            }
        }
    }
}
