using System;
using System.Xml;

namespace LT.Component.Tasks
{
    /// <summary>
    /// ITask 通用类
    /// </summary>
    public interface ITask
    {
        void Execute(XmlNode node);

        void OnError(Exception ex);
    }
}
