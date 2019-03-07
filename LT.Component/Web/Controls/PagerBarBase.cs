using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

namespace LT.Component.Web.Controls
{
    /// <summary>
    /// PagerBarBase 通用类
    /// </summary>
    public class PagerBarBase : Control
    {
        protected static readonly string normalItem = "<div class=\"{0}\">{1}</div>";
        protected static readonly string enableItem = "<div class=\"{0}\"><a href=\"{1}\">{2}</a></div>";
        protected static readonly string disableItem = "<div class=\"{0}\" disabled=\"disabled\">{1}</div>";
        protected static readonly string selectedItem = "<div class=\"{0}\"><font color=\"red\"><b>{1}</b></font></div>";

        protected static string pageUrl;
        protected static string linkUrl;
        protected static string numberPage;

        protected static int record;
        protected static int currentPage;

        protected static StringBuilder sb;

        /// <summary>
        /// 获取或设置翻页链接地址
        /// </summary>
        public string LinkUrl
        {
            get
            {
                return _linkUrl;
            }
            set
            {
                _linkUrl = value;
            }
        }
        private string _linkUrl = "";

        /// <summary>
        /// 获取或设置是否显示页码状态 (默认值true)
        /// </summary>
        public bool ShowState
        {
            get
            {
                return _showState;
            }
            set
            {
                _showState = value;
            }
        }
        private bool _showState = true;

        /// <summary>
        /// 数字页
        /// </summary>
        public string NumberPage
        {
            get
            {
                return _numberPage;
            }
            set
            {
                _numberPage = value;
            }
        }
        private string _numberPage = "[{0}]";

        /// <summary>
        /// 总记录数
        /// </summary>
        public string RecordPage
        {
            get
            {
                return _recordPage;
            }
            set
            {
                _recordPage = value;
            }
        }
        private string _recordPage = "共: {0} 条";

        protected string GetPageLinkUrl()
        {
            string url = (LinkUrl.Length == 0) ? HttpContext.Current.Request.RawUrl : linkUrl;
            Regex r = new Regex("(\\?|&)p=([^&]*)", RegexOptions.IgnoreCase);
            Match m = r.Match(url);
            if (m.Success)
            {
                return r.Replace(url, "$1p={0}");
            }
            else
            {
                if (url.IndexOf("?") == -1)
                {
                    return url + "?p={0}";
                }
                else
                {
                    return url + "&p={0}";
                }
            }
        }

        protected void CreatNumbericLink(int startPage, int endPage, int currentPage)
        {
            for (int i = startPage; i <= endPage; i++)
            {
                numberPage = string.Format(NumberPage, i);
                if (i == currentPage)
                {
                    sb.Append(string.Format(selectedItem, "number", numberPage));
                }
                else
                {
                    pageUrl = string.Format(linkUrl, i);
                    sb.Append(string.Format(enableItem, "number", pageUrl, numberPage));
                }
            }
        }

        //protected static readonly string enableItem = "<a class=\"{0}\" href=\"{1}\">{2}</a>";
        //protected static readonly string disableItem = "<a class=\"{0} disabled\">{1}</a>";
        //protected static readonly string selectedItem = "<a class=\"{0} curpage\">{1}</a>";

        ///// <summary>
        ///// 第一页
        ///// </summary>
        //public string FirstPage
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// 上一页
        ///// </summary>
        //public string PrevPage
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// 下一页
        ///// </summary>
        //public string NextPage
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// 最后页
        ///// </summary>
        //public string LastPage
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// 数字页
        ///// 格式：[{0}]
        ///// </summary>
        //public string NumberPage
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// 输入框页
        ///// 格式：{0} / {1}
        ///// <input type="text" value="1" onkeydown="if(event.keyCode=13){}" />
        ///// </summary>
        //public string BoxPage
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// 输入框页
        ///// 格式：<li class="{0}"></li>
        ///// </summary>
        //public string SplitPage
        //{
        //    get;
        //    set;
        //}

        //protected string GetPageLinkUrl()
        //{
        //    string url = HttpContext.Current.Request.RawUrl;
        //    Regex r = new Regex("(\\?|&)p=([^&]*)", RegexOptions.IgnoreCase);
        //    Match m = r.Match(url);
        //    if (m.Success)
        //    {
        //        return r.Replace(url, "$1p={0}");
        //    }
        //    else
        //    {
        //        if (url.IndexOf("?") == -1)
        //        {
        //            return url + "?p={0}";
        //        }
        //        else
        //        {
        //            return url + "&p={0}";
        //        }
        //    }
        //}

        //private string CreatNumbericLink(int startPage, int endPage, int currentPage, string linkUrl)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    string numberPage = null;
        //    string pageUrl = null;

        //    for (int i = startPage; i <= endPage; i++)
        //    {
        //        numberPage = string.Format(NumberPage, i);
        //        if (i == currentPage)
        //        {
        //            sb.Append(string.Format(selectedItem, "number", numberPage));
        //        }
        //        else
        //        {
        //            pageUrl = string.Format(linkUrl, i);
        //            sb.Append(string.Format(enableItem, "number", pageUrl, numberPage));
        //        }
        //    }
        //    return sb.ToString();
        //}

        //protected void LoadPageIcon(int record, int currentPage, int pageSize)
        //{
        //    //当前页地址
        //    string linkUrl = GetPageLinkUrl();
        //    string pageUrl = null;

        //    //页码数
        //    int pages = 0;
        //    if (record % pageSize == 0)
        //    {
        //        pages = record / pageSize;
        //    }
        //    else
        //    {
        //        pages = record / pageSize + 1;
        //    }
        //    if (currentPage > pages)
        //    {
        //        currentPage = pages;
        //    }
        //    if (currentPage <= 0)
        //    {
        //        currentPage = 1;
        //    }

        //    //第一页 | 上一页
        //    if (currentPage > 1 && pages > 1)
        //    {
        //        if (FirstPage != null)
        //        {
        //            pageUrl = string.Format(linkUrl, "1");
        //            FirstPage = string.Format(enableItem, "icon", pageUrl, FirstPage);
        //        }
        //        if (PrevPage != null)
        //        {
        //            pageUrl = string.Format(linkUrl, currentPage - 1);
        //            PrevPage = string.Format(enableItem, "icon", pageUrl, PrevPage);
        //        }
        //    }
        //    else
        //    {
        //        if (FirstPage != null)
        //        {
        //            FirstPage = string.Format(disableItem, "icon", FirstPage);
        //        }
        //        if (PrevPage != null)
        //        {
        //            PrevPage = string.Format(disableItem, "icon", PrevPage);
        //        }
        //    }

        //    //数字页
        //    if (NumberPage != null)
        //    {
        //        int s = 10;
        //        int p = pages / s;
        //        int n = (currentPage - 1) / s;

        //        if (pages < s)
        //        {
        //            NumberPage = CreatNumbericLink(1, pages, currentPage, linkUrl);
        //        }
        //        else if (p == n)
        //        {
        //            NumberPage = CreatNumbericLink(n * s + 1, pages, currentPage, linkUrl);
        //        }
        //        else
        //        {
        //            NumberPage = CreatNumbericLink(n * s + 1, (n + 1) * s, currentPage, linkUrl);
        //        }
        //    }

        //    //下一页 | 最后页
        //    if (currentPage != pages && pages > 1)
        //    {
        //        if (NextPage != null)
        //        {
        //            pageUrl = string.Format(linkUrl, currentPage + 1);
        //            NextPage = string.Format(enableItem, "icon", pageUrl, NextPage);
        //        }
        //        if (LastPage != null)
        //        {
        //            pageUrl = string.Format(linkUrl, pages);
        //            LastPage = string.Format(enableItem, "icon", pageUrl, LastPage);
        //        }
        //    }
        //    else
        //    {
        //        if (NextPage != null)
        //        {
        //            NextPage = string.Format(disableItem, "icon", NextPage);
        //        }
        //        if (LastPage != null)
        //        {
        //            LastPage = string.Format(disableItem, "icon", LastPage);
        //        }
        //    }

        //    //输入框页
        //    if (BoxPage != null)
        //    {
        //        string curinput = "<input type=\"text\" value=\""+ currentPage +"\" onkeydown=\"if(event.keyCode==13){window.location.href=('" + linkUrl + "'.replace('{0}', this.value));return false;}\" />";
        //        BoxPage = string.Format(BoxPage, curinput, pages, record);
        //    }

        //    //分离页
        //    if (SplitPage != null)
        //    {
        //        SplitPage = string.Format(SplitPage, "split");
        //    }
        //}
    }
}
