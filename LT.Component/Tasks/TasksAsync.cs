using System.Collections.Generic;
using System.Threading;

namespace LT.Component.Tasks
{
    public sealed class TasksAsync
    {
        /// <summary>
        /// 回调代理主题
        /// </summary>
        /// <param name="paramters"></param>
        public delegate void Target(object[] paramters);

        /// <summary>
        /// 主体集合
        /// </summary>
        public Dictionary<int,  Target> m_targets = new Dictionary<int, Target>();
        public Dictionary<int, object[]> m_parms = new Dictionary<int, object[]>();

        /// <summary>
        /// 自动更新线程Timer
        /// </summary>
        public Timer m_timer = null;

        /// <summary>
        /// 单件实体
        /// </summary>
        private static TasksAsync m_instance;

         /// <summary>
        /// 初始化
        /// </summary>
        private TasksAsync()
        {
            //实例化并启动Timer
            m_timer = new Timer(TimerTickCallback, null, 1000, 1000);
        }

        /// <summary>
        /// Timer回调
        /// </summary>
        /// <param name="state"></param>
        void TimerTickCallback(object state)
        {
            Target target = null;
            object[] parm = null;
            foreach (var key in m_parms.Keys)
            {
                target = m_targets[key];
                parm = m_parms[key];
                target.BeginInvoke(parm, null, null);
            }
        }

        /// <summary>
        /// 获取单件实体
        /// </summary>
        public static TasksAsync Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new TasksAsync();
                }
                return m_instance;
            }
        }

        /// <summary>
        /// 添加回调代理
        /// </summary>
        /// <param name="target">外部代理方法</param>
        /// <param name="parms">调用参数</param>
        public void AddTarget(Target target, object[] parms)
        {
            int idx = m_parms.Count + 1;
            m_parms.Add(idx, parms);
            m_targets.Add(idx, target);
        }

    }
}
