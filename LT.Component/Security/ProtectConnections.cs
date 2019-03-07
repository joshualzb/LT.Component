using System.Configuration;
using System.Xml;

namespace LT.Component.Security
{

    /// <summary>
    /// ProtectConnections 通用类
    /// </summary>
    public class ProtectConnections : ProtectedConfigurationProvider
    {
        /// <summary>
        /// password
        /// </summary>
        private static readonly string keyCode = "fe7jfwe";

        /// <summary>
        /// ProtectedConfigurationProvider.Encrypt
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override XmlNode Encrypt(XmlNode node)
        {
            return null;
        }

        /// <summary>
        /// ProtectedConfigurationProvider.Decrypt
        /// </summary>
        /// <param name="encryptedNode"></param>
        /// <returns></returns>
        public override XmlNode Decrypt(XmlNode encryptedNode)
        {
            string decryptedData = Utility.DESCrypto.Decrypt(keyCode, encryptedNode.InnerText);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.LoadXml("<connectionStrings>" + decryptedData + "</connectionStrings>");

            return xmlDoc.DocumentElement;
        }
    }
}
