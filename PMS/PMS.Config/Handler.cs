using System;
using System.Configuration;
using System.Xml;

namespace PMS.Config
{
    public sealed class Handler : IConfigurationSectionHandler
    {
        public Handler()
        {
        }

        public object Create(object parent, object configContext, XmlNode node)
        {
            return node;
        }
    }
}