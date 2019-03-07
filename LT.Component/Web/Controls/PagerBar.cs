using System;
using System.Text;
using System.Web.UI;

namespace LT.Component.Web.Controls
{
    /// <summary>
    /// Description for this class
    /// </summary>
    public class CPNPagerBar : PagerBarBase
    {
        /// <summary>
        /// 绑定到的列表源ID
        /// </summary>
        private string _repeaterId;

        /// <summary>
        /// 获取或设置绑定到的列表源ID
        /// </summary>
        public string RepeaterId
        {
            get { return _repeaterId; }
            set { _repeaterId = value; }
        }

        /// <summary>
        /// 第一页
        /// </summary>
        public string FirstPage
        {
            get
            {
                return _firstPage;
            }
            set
            {
                _firstPage = value;
            }
        }
        private string _firstPage = "首页";

        /// <summary>
        /// 上一页
        /// </summary>
        public string PrePage
        {
            get
            {
                return _prePage;
            }
            set
            {
                _prePage = value;
            }
        }
        private string _prePage = "上页";

        /// <summary>
        /// 下一页
        /// </summary>
        public string NextPage
        {
            get
            {
                return _nextPage;
            }
            set
            {
                _nextPage = value;
            }
        }
        private string _nextPage = "下页";

        /// <summary>
        /// 最后页
        /// </summary>
        public string LastPage
        {
            get
            {
                return _lastPage;
            }
            set
            {
                _lastPage = value;
            }
        }
        private string _lastPage = "尾页";

        protected override void Render(HtmlTextWriter writer)
        {
            BaseDataList dataList = this.FindControl(RepeaterId) as BaseDataList;

            if (dataList == null)
            {
                throw new Exception("It can not find the repeater. Please reconfirm the RepeaterId.");
            }

            //当前页地址
            linkUrl = GetPageLinkUrl();

            //定义变量
            record = dataList.SearchQuery.Record;
            currentPage = dataList.SearchQuery.CurrentPage;
            int pageSize = dataList.SearchQuery.PageSize;

            //页码数
            int pages;
            if (record % pageSize == 0)
            {
                pages = record / pageSize;
            }
            else
            {
                pages = record / pageSize + 1;
            }

            sb = new StringBuilder();
            sb.Append("<dl id=\"pagerBar\">");

            if (ShowState)
            {
                sb.Append("<div class=\"state\">");
                sb.Append(string.Format(normalItem, "records", string.Format(RecordPage, record)));
                sb.Append("</div>");
            }

            sb.Append("<div class=\"list\">");

            //第一页 | 上一页
            if (currentPage > 1 && pages > 1)
            {
                pageUrl = string.Format(linkUrl, "1");
                sb.Append(string.Format(enableItem, "icon", pageUrl, FirstPage));

                pageUrl = string.Format(linkUrl, currentPage - 1);
                sb.Append(string.Format(enableItem, "icon", pageUrl, PrePage));
            }
            else
            {
                sb.Append(string.Format(disableItem, "icon", FirstPage));
                sb.Append(string.Format(disableItem, "icon", PrePage));
            }

            //数字页
            int s = 10;
            int p = pages / s;
            int n = (currentPage - 1) / s;

            if (pages < s)
            {
                CreatNumbericLink(1, pages, currentPage);
            }
            else if (p == n)
            {
                CreatNumbericLink(n * s + 1, pages, currentPage);
            }
            else
            {
                CreatNumbericLink(n * s + 1, (n + 1) * s, currentPage);
            }

            //下一页 | 最后页
            if (currentPage != pages && pages > 1)
            {
                pageUrl = string.Format(linkUrl, currentPage + 1);
                sb.Append(string.Format(enableItem, "icon", pageUrl, NextPage));

                pageUrl = string.Format(linkUrl, pages);
                sb.Append(string.Format(enableItem, "icon", pageUrl, LastPage));
            }
            else
            {
                sb.Append(string.Format(disableItem, "icon", NextPage));
                sb.Append(string.Format(disableItem, "icon", LastPage));
            }

            sb.Append("</div></dl>");

            writer.Write(sb);
        }

        ///// <summary>
        ///// 获取或设置绑定到的列表源ID
        ///// </summary>
        //public string RepeaterId
        //{
        //    get;
        //    set;
        //}

        //public CPNPagerBar()
        //{
        //    FirstPage = " ";
        //    PrevPage = " ";
        //    NextPage = " ";
        //    LastPage = " ";
        //    BoxPage = "<li class=\"number\">{0} / {1}页 / {2}行</li>";
        //    SplitPage = "<li class=\"{0}\"></li>";
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="writer"></param>
        //protected override void Render(HtmlTextWriter writer)
        //{
        //    BaseDataList dataList = this.FindControl(RepeaterId) as BaseDataList;
        //    if (dataList == null)
        //    {
        //        throw new Exception("It can not find the repeater. Please reconfirm the RepeaterId.");
        //    }

        //    int record = dataList.SearchQuery.Record;
        //    int currentPage = dataList.SearchQuery.CurrentPage;
        //    int pageSize = dataList.SearchQuery.PageSize;

        //    LoadPageIcon(record, currentPage, pageSize);

        //    writer.Write("<ul class=\"datapager\">");
        //    writer.Write(string.Format("<li class=\"btn first\">{0}</li>", FirstPage));
        //    writer.Write(string.Format("<li class=\"btn prev\">{0}</li>", PrevPage));
        //    writer.Write(SplitPage);
        //    writer.Write(BoxPage);
        //    writer.Write(SplitPage);
        //    writer.Write(string.Format("<li class=\"btn next\">{0}</li>", NextPage));
        //    writer.Write(string.Format("<li class=\"btn last\">{0}</li>", LastPage));
        //    writer.Write("</ul>");
        //}
    }
}