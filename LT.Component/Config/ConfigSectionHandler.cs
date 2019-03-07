using System.Configuration;
using System.Xml;

namespace LT.Component.Config
{
    /// <summary>
    /// ConfigSectionHandler 通用类
    /// </summary>
    public class ConfigSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            return section;
        }
    }
}
