using System.Collections;
using System.Web;
using System.Web.UI;

namespace LT.Component.Web.Controls
{
    /// <summary>
    /// SearchQuery 通用类
    /// </summary>
    public class SearchQuery : Control
    {

        /// <summary>
        /// 设置从页面获取的参数
        /// </summary>
        /// <param name="key"></param>
        public void AddRequest(string key)
        {
            string val = HttpContext.Current.Request.Params[key];
            if (val != null && val.Length > 0)
            {
                val = Utility.Strings.SqlEncode(val);
                this.Condition.Add(key, val);
            }
        }

        /// <summary>
        /// 保存条件到容器中
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public void AddCondition(string key, string val)
        {
            if (!string.IsNullOrEmpty(val))
            {
                Condition.Add(key, val);
            }
        }

        public string RequestString
        {
            set
            {
                if (value != null)
                {
                    string[] keys = value.Split(',');
                    foreach (string key in keys)
                    {
                        AddRequest(key);
                    }
                }
            }
        }

        /// <summary>
        /// 获取或设置页记录数大小(每页显示的记录数)，默认为20
        /// </summary>
        public int PageSize
        {
            set
            {
                _pageSize = value;
            }
            get
            {
                return _pageSize;
            }
        }
        private int _pageSize = 20;

        /// <summary>
        /// 获取或设置显示列表的字段集，使用逗号分事，如 SortId,SortName
        /// </summary>
        public string Columns
        {
            set
            {
                _columns = value;
            }
            get
            {
                return _columns;
            }
        }
        private string _columns = "";

        /// <summary>
        /// 设置或获取记录集大小
        /// </summary>
        public int Record
        {
            set
            {
                _record = value;
            }
            get
            {
                return _record;
            }
        }
        private int _record;

        /// <summary>
        /// 获取和设置当前页码 (默认传入参数为p)
        /// </summary>
        public int CurrentPage
        {
            get
            {
                if (_currentPage > 0)
                {
                    return _currentPage;
                }

                string pstr = HttpContext.Current.Request.QueryString["p"];

                if (string.IsNullOrEmpty(pstr))
                {
                    return 1;
                }

                int pid = int.Parse(pstr);
                return pid < 1 ? 1 : pid;
            }
            set
            {
                _currentPage = value;
            }
        }
        private int _currentPage;

        /// <summary>
        /// 设置或获取排序方式 (eg. InfoId ASC, OrderId DESC)
        /// </summary>
        public string OrderBy
        {
            set
            {
                _orderBy = value;
            }
            get
            {
                return _orderBy;
            }
        }
        private string _orderBy;

        /// <summary>
        /// 获取其它条件的装载器
        /// </summary>
        public Hashtable Condition
        {
            get
            {
                if (_condition != null) return _condition;
                _condition = new Hashtable();
                return _condition;
            }
        }
        private Hashtable _condition;
    }
}
