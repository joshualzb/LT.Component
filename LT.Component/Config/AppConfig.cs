using System.Configuration;
using System.Text.RegularExpressions;
using System.Xml;

namespace LT.Component.Config
{
    /// <summary>
    /// AppConfig 通用类
    /// </summary>
    public static class AppConfig
    {
        /// <summary>
        /// 获取配置文件的AppSettings的节点值，返回string型
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetString(string key)
        {
            string val = ConfigurationManager.AppSettings[key];

            if (val == null)
            {
                return "";
            }
            return val;
        }

        /// <summary>
        /// 获取配置文件的AppSettings的节点值，返回INT型
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int GetInt(string key)
        {
            string val = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(val) || !Regex.IsMatch(val, @"^\d+$"))
            {
                return 0;
            }
            return int.Parse(val);
        }

        /// <summary>
        /// 获取配置文件的Section配置XML内容
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public static XmlNode GetSection(string sectionName)
        {
            return ConfigurationManager.GetSection(sectionName) as XmlNode;
        }

        /// <summary>
        /// 获取配置文件件中的数据库连接符
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConnection(string key)
        {
            return ConfigurationManager.ConnectionStrings[key].ConnectionString;
        }
    }
}
