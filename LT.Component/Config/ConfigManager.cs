using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using LT.Component.DataProvider;

namespace LT.Component.Config
{
    /// <summary>
    /// ConfigManager 通用类
    /// </summary>
    public class ConfigManager<T> where T : class
    {
        private static object loadConfig_lock = new object();

        /// <summary>
        /// 定时检查文件是否有更新
        /// </summary>
        private Timer _timer;

        /// <summary>
        /// 配置文本最后获取时间
        /// </summary>
        private DateTime _lastDateTime;

        /// <summary>
        /// 配置的内容对象
        /// </summary>
        private IConfigInfo _configInfo;

        /// <summary>
        /// 完整文件路径
        /// </summary>
        private string _fullfile;

        /// <summary>
        /// 储存属性值的字典
        /// </summary>
        private Dictionary<string, object> _values = new Dictionary<string, object>();

        public ConfigManager(string configName, string filename)
        {
            //获取配置文件路径
            string configPath = "";
            if (configName != null && configName.Length > 0)
            {
                configPath = AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings[configName];
            }
            else
            {
                configPath = AppDomain.CurrentDomain.BaseDirectory;
            }

            //判断配置文件是否存在
            _fullfile = Path.Combine(configPath, filename);
            if (!File.Exists(_fullfile))
            {
                throw new FileNotFoundException(string.Format("{0} is missing!", filename), _fullfile);
            }

            //初始化加载配置数据
            LoadData(_fullfile, false);

            //初始化一个定时检查Timer
            _timer = new Timer(TimerClickCallback, null, Timeout.Infinite, Timeout.Infinite);
            _timer.Change(5000, 5000);
        }

        /// <summary>
        /// 定时器回调
        /// </summary>
        /// <param name="state"></param>
        void TimerClickCallback(object state)
        {
            //暂停Timer
            _timer.Change(Timeout.Infinite, Timeout.Infinite);

            //判断是否时间有差别
            if (File.GetLastWriteTime(_fullfile) != _lastDateTime)
            {
                LoadData(_fullfile, true);
            }

            //重起Timer
            _timer.Change(5000, 5000);
        }

        /// <summary>
        /// 加载配置数据
        /// </summary>
        /// <param name="configfile"></param>
        /// <param name="forceToLoad">是否强制重新读取</param>
        public virtual void LoadData(string configfile, bool forceToLoad)
        {
            if (_configInfo == null || forceToLoad == true)
            {
                _configInfo = Load(typeof(T), configfile);

                //清除属性值字典
                _values.Clear();

                //获取文件最后写入时间
                _lastDateTime = File.GetLastWriteTime(configfile);
            }
        }

        /// <summary>
        /// 从XML中加载配置对象内容
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fullfile">完整路径的XML配置文件</param>
        /// <returns></returns>
        IConfigInfo Load(Type type, string fullfile)
        {
            lock (loadConfig_lock)
            {
                return (IConfigInfo)SerializationHelper.Load(type, fullfile);
            }
        }

        /// <summary>
        /// 保存配置对象内容到XML
        /// </summary>
        /// <param name="configinfo"></param>
        /// <returns></returns>
        public bool Save(IConfigInfo configinfo)
        {
            return SerializationHelper.Save(configinfo, _fullfile);
        }

        /// <summary>
        /// 获取指定属性名的值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetProperty(string name)
        {
            if (!_values.ContainsKey(name))
            {
                _values.Add(name, typeof(T).GetProperty(name).GetValue(_configInfo, null));
            }
            return _values[name];
        }

        /// <summary>
        /// 获取配置的内容对象
        /// </summary>
        public IConfigInfo ConfigInfo
        {
            get
            {
                return _configInfo;
            }
        }
    }
}
