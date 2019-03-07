using System;
using LT.Component.DataProvider;

namespace LT.Component.Tasks
{

    /// <summary>
    /// WriteToText 通用类
    /// </summary>
    public class WriteToText : ITask
    {
        public void Execute(System.Xml.XmlNode node)
        {
            //string sqlText = "INSERT platform_log (TypeId,MemberName,Source,Content,AddTime) VALUES (0,'deiva','url','" + TaskManager.startUpPath + "|" + TaskManager.webHostUrl + "','" + DateTime.Now + "')";
            //MySqlConnector.NonQuery(sqlText, System.Data.CommandType.Text, null);
        }

        public void OnError(Exception ex)
        {
            //string sqlText = "INSERT platform_log (TypeId,MemberName,Source,Content,AddTime) VALUES (1,'deiva','url','" + ex.ToString() + "','"+ DateTime.Now +"')";
            //MySqlConnector.NonQuery(sqlText, System.Data.CommandType.Text, null);
        }
    }
}
