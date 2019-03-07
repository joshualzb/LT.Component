using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace LT.Component.Web.Url
{
    /// <summary>
    /// Rules 通用类
    /// </summary>
    public class Rules
    {
        private static Dictionary<string, List<RuleModel>> _rules;
        private static string [] _paths;

        public static List<RuleModel> Get(string path)
        {
            Dictionary<string, List<RuleModel>> rules = GetRules( );
            if ( rules.ContainsKey( path ) )
            {
                return rules [path];
            }

            return null;
        }

        public static string [] GetPaths()
        {
            if ( _paths == null )
            {
                Dictionary<string, List<RuleModel>> rules = GetRules( );
                Dictionary<string, List<RuleModel>>.KeyCollection keys = rules.Keys;

                _paths = new string [keys.Count];
                keys.CopyTo( _paths, 0 );
            }

            return _paths;
        }

        private static Dictionary<string, List<RuleModel>> GetRules()
        {
            if ( _rules == null )
            {
                XmlNode node = ConfigurationManager.GetSection("UrlConfig") as XmlNode;
                if ( node == null )
                {
                    return null;
                }

                //初始化容器
                _rules = new Dictionary<string, List<RuleModel>>( );

                //获取所有Pages节点
                XmlNodeList pages = node.SelectNodes( "/UrlConfig/Rules/pages" );

                int i = 0;
                int j = 0;
                int count = pages.Count;

                for ( i = 0;i < count;i++ )
                {
                    //获取所有Url节点
                    XmlNodeList urls = pages [i].ChildNodes;
                    List<RuleModel> list = new List<RuleModel>( );

                    for ( j = 0;j < urls.Count;j++ )
                    {
                        list.Add( new RuleModel( urls [j].Attributes ["from"].Value, urls [j].Attributes ["to"].Value, urls [j].Attributes ["op"].Value) );
                    }

                    string key = pages [i].Attributes ["path"].Value;
                    if ( !_rules.ContainsKey( key ) )
                    {
                        _rules.Add( key, list );
                    }
                }
            }

            //返回
            return _rules;
        }
    }
}
