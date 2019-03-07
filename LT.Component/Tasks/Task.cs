using System;
using System.Collections.Generic;
using System.Xml;

namespace LT.Component.Tasks
{
    [Serializable]

    /// <summary>
    /// Task 通用类
    /// </summary>
    public sealed class Task
    {
        private string _name;
        private ITask _iTask;
        private Type _type;
        private XmlNode _taskXml;

        public Task(Type type, XmlNode taskXml)
        {
            _type = type;
            _taskXml = taskXml;
            _name = taskXml.Attributes["name"].Value;
        }

        public void Run()
        {
            ITask task = Instance();
            if (task != null)
            {
                try
                {
                    task.Execute(_taskXml);
                }
                catch (Exception ex)
                {
                    task.OnError(ex);
                }
            }
        }

        private ITask Instance()
        {
            if (_iTask == null)
            {
                _iTask = Activator.CreateInstance(_type) as ITask;
            }
            return _iTask;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}
