using System;
using System.Collections.Generic;
using System.Xml;
using System.Threading;

namespace LT.Component.Tasks
{
    /// <summary>
    /// TaskThread 通用类
    /// </summary>
    public sealed class TaskThread : IDisposable
    {
        private Dictionary<string, Task> _tasks;
        private Timer _timer;
        private int _seconds;
        private bool disposed = false;

        public TaskThread(XmlNode threadXml)
        {
            _tasks = new Dictionary<string, Task>();

            _seconds = int.Parse(threadXml.Attributes["seconds"].Value);
            if (_seconds <= 0)
            {
                _seconds = 600;
            }
        }

        public void AddTask(Task task)
        {
            if (!_tasks.ContainsKey(task.Name))
            {
                _tasks.Add(task.Name, task);
            }
        }

        /// <summary>
        /// Creates the timer and sets the callback if it is enabled
        /// </summary>
        public void InitializeTimer()
        {
            if (_timer == null)
            {
                _timer = new Timer(new TimerCallback(timer_Callback), null, Interval, Interval);
            }
        }

        private void timer_Callback(object state)
        {
            _timer.Change(-1, -1);

            foreach (Task task in _tasks.Values)
            {
                task.Run();
            }

            _timer.Change(Interval, Interval);
        }

        private int Interval
        {
            get
            {
                return 1000 * _seconds;
            }
        }

        #region IDisposable 成员

        public void Dispose()
        {
            if (!disposed)
            {
                if (_timer != null)
                {
                    lock (this)
                    {
                        _timer.Dispose();
                        _timer = null;
                    }
                }
                disposed = true;
            }
        }

        #endregion
    }
}
