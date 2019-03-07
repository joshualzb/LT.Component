using System.Collections.Generic;

namespace LT.Component.Entity
{
    /// <summary>
    /// ExtXmlHolder 通用类
    /// </summary>
    public class ExtXml
    {
        /// <summary>
        /// Query容器
        /// </summary>
        public List<ExtModel.Query> Querys
        {
            set { _querys = value; }
            get { return _querys; }
        }
        private List<ExtModel.Query> _querys;

        /// <summary>
        /// Order容器
        /// </summary>
        public List<ExtModel.Order> Orders
        {
            set { _orders = value; }
            get { return _orders; }
        }
        private List<ExtModel.Order> _orders;

        /// <summary>
        /// Pager容器
        /// </summary>
        public List<ExtModel.Pager> Pagers
        {
            set { _pagers = value; }
            get { return _pagers; }
        }
        private List<ExtModel.Pager> _pagers;
    }
}
