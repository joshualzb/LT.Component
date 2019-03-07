using System;

namespace LT.Component.Entity.ExtModel
{
    /// <summary>
    /// Query 通用类
    /// </summary>
    public class Query
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
        /// 父ID
        /// </summary>
        public int ParentId
        {
            set { _parentId = value; }
            get { return _parentId; }
        }
        private int _parentId;

        /// <summary>
        /// 含有子个数
        /// </summary>
        public int ChildNodes
        {
            set { _childNodes = value; }
            get { return _childNodes; }
        }
        private int _childNodes;

        /// <summary>
        /// SQL语句关系符
        /// </summary>
        public string OP
        {
            set { _op = value; }
            get { return _op; }
        }
        private string _op;

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
        /// 不接受值
        /// </summary>
        public string Exc
        {
            set { _exc = value; }
            get { return _exc; }
        }
        private string _exc;

        /// <summary>
        /// 数据类型
        /// string：字符串（默认）
        /// number：数字，含有正负和小数点
        /// </summary>
        public string Fmt
        {
            set { _fmt = value; }
            get { return _fmt; }
        }
        private string _fmt = "string";

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
        public Query Clone()
        {
            Query model = new Query();
            model.ListId = _listId;
            model.ParentId = _parentId;
            model.ChildNodes = _childNodes;
            model.OP = _op;
            model.Key = _key;
            model.Exc = _exc;
            model.Fmt = _fmt;
            model.SQL = _sql;

            return model;
        }
    }
}
