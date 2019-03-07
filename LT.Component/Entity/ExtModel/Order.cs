using System;

namespace LT.Component.Entity.ExtModel
{
    /// <summary>
    /// Order 通用类
    /// </summary>
    public class Order
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        public int ListId
        {
            set { _listId = value; }
            get { return _listId; }
        }
        private int _listId;

        /// <summary>
        /// 关键字词，如外部提供获取值的URL参数名
        /// </summary>
        public string Key
        {
            set { _key = value; }
            get { return _key; }
        }
        private string _key;

        /// <summary>
        /// Case类型
        /// </summary>
        public string Case
        {
            set { _case = value; }
            get { return _case; }
        }
        private string _case;

        /// <summary>
        /// 单SQL语名，不带关系符
        /// </summary>
        public string SQL
        {
            set { _sql = value; }
            get { return _sql; }
        }
        private string _sql;

        /// <summary>
        /// 复制内容
        /// </summary>
        /// <returns></returns>
        public Order Clone()
        {
            Order model = new Order();
            model.ListId = _listId;
            model.Key = _key;
            model.Case = _case;
            model.SQL = _sql;

            return model;
        }
    }
}
