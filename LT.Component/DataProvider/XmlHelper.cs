using System;
using System.IO;
using System.Xml;

namespace LT.Component.DataProvider
{
    /// <summary>
    /// XmlHelper 通用类
    /// </summary>
    public class XmlHelper
    {
        private XmlDocument _xmlDoc;

        public XmlHelper(string fileName)
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(appPath, fileName);

            //读取XML文档对象
            if (File.Exists(filePath))
            {
                _xmlDoc = new XmlDocument();
                _xmlDoc.Load(filePath);
            }
            else
            {
                throw new Exception("Xml file \"" + fileName + "\" does not exist.");
            }
        }

        /// <summary>
        /// 读取单节点对象
        /// </summary>
        public XmlNode SelectSingleNode(string xPath)
        {
            return _xmlDoc.SelectSingleNode(xPath);
        }

        /// <summary>
        /// 读取某节点下对象集合
        /// </summary>
        public XmlNodeList SelectNodes(string xPath)
        {
            return _xmlDoc.SelectNodes(xPath);
        }
    }
}
