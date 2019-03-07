using System;
using System.Collections.Generic;
using System.Xml;

namespace LT.Component.Tasks
{
    /// <summary>
    /// TaskManager 通用类
    /// </summary>
    public sealed class TaskManager
    {
        private List<TaskThread> _taskThreads;
        private static readonly TaskManager _taskManager = null;

        static TaskManager()
        {
            _taskManager = new TaskManager();
        }

        public static TaskManager Instance()
        {
            return _taskManager;
        }

        private TaskManager()
        {
            _taskThreads = new List<TaskThread>();

            XmlNode tasks = null;//Config.TasksConfig.Get();
            foreach (XmlNode threadXml in tasks.ChildNodes)
            {
                TaskThread taskThread = new TaskThread(threadXml);
                _taskThreads.Add(taskThread);

                foreach (XmlNode taskXml in threadXml.ChildNodes)
                {
                    if (taskXml.Attributes["enable"] != null && taskXml.Attributes["enable"].Value == "true")
                    {
                        Type type = Type.GetType(taskXml.Attributes["type"].Value);
                        if (type != null)
                        {
                            Task task = new Task(type, taskXml);
                            taskThread.AddTask(task);
                        }
                    }
                }
            }
        }

        public void Start()
        {
            foreach (TaskThread taskThread in _taskThreads)
            {
                taskThread.InitializeTimer();
            }
        }

        public void Stop()
        {
            foreach (TaskThread taskThread in _taskThreads)
            {
                taskThread.Dispose();
            }
            _taskThreads.Clear();
        }
    }
}
