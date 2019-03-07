using System;
using System.Collections.Generic;

namespace LT.Component.Generic
{
    /// <summary>
    /// TBase 通用类
    /// </summary>
    public class TBase
    {
        private string _keyName = "SortId";

        /// <summary>
        /// 类集的主键名称，默认为SortId
        /// </summary>
        public string KeyName
        {
            set { _keyName = value; }
            get { return _keyName; }
        }
    }
}
