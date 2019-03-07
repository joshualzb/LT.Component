using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Linq;
using System.Web.UI;

namespace LT.Component.Web
{
    /// <summary>
    /// PageHelper 通用类
    /// </summary>
    public class PageHelper
    {
        #region 从地址栏获取Id值（整数）
        /// <summary>
        /// 从HTTP URL获取数字值
        /// </summary>
        /// <param name="Key">HTTP URL 的参数名</param>
        /// <returns></returns>
        public static int GetIdFromUrl(string Key)
        {
            int _keyid = 0;
            string keyid = HttpContext.Current.Request.QueryString[Key];
            if (keyid != null && keyid.Length > 0)
            {
                if (Utility.Strings.IsNumber(keyid))
                {
                    _keyid = int.Parse(keyid);
                }
                else
                {
                    _keyid = 0;
                }
            }
            return _keyid;
        }

        /// <summary>
        /// 获取页数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int GetIdFromUrlPage(string key)
        {
            string id = GetValueFromUrlPage(key);
            if (id.Length == 0 || !Utility.Strings.IsNumber(id))
            {
                return 0;
            }
            return int.Parse(id);
        }

        /// <summary>
        /// 获取页数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValueFromUrlPage(string key)
        {
            //ID数据源
            string id = HttpContext.Current.Request.QueryString[key];
            if (id == null || id.Length == 0)
            {
                return "";
            }
            string[] ids = id.Split(',');
            int records = ids.Length;

            //当前页码
            string p = HttpContext.Current.Request.QueryString["p"];
            int currentPage = 1;
            if (p != null)
            {
                if (Utility.Strings.IsNumber(p))
                {
                    currentPage = int.Parse(p);
                }
                if (currentPage > records)
                {
                    currentPage = records;
                }
                else if (currentPage < 1)
                {
                    currentPage = 1;
                }
            }

            //获取号
            return (ids[currentPage - 1]);
        }
        #endregion

        #region 从Form栏获取Id值（整数）
        /// <summary>
        /// 从网页提交的表单 Form 获取数字
        /// </summary>
        /// <param name="Key">Form表单的参数名</param>
        /// <returns></returns>
        public static int GetIdFromForm(string Key)
        {
            int _keyid = 0;
            string keyid = HttpContext.Current.Request.Form[Key];
            if (keyid != null && keyid.Length > 0)
            {
                if (Utility.Strings.IsNumber(keyid) == true)
                {
                    _keyid = int.Parse(keyid);
                }
                else
                {
                    _keyid = 0;
                }
            }
            return _keyid;
        }
        #endregion

        #region 从地址栏获取值（key值字符串）
        /// <summary>
        ///  从URL栏获取值(key值字符串)
        /// </summary>
        public static string GetKeyFromUrl(string key)
        {
            string keyvalue = HttpContext.Current.Request.QueryString[key];
            if (keyvalue != null && Utility.Strings.IsKeyString(keyvalue))
            {
                return keyvalue;
            }
            return "";
        }
        #endregion

        #region 从地址栏获取值（字符串）
        /// <summary>
        /// 增加
        ///     从URL栏获取值(字符串)
        ///     Fanming 2007-8-10
        /// </summary>
        public static string GetValueFromUrl(string key)
        {
            string keyvalue = HttpContext.Current.Request.QueryString[key];

            if (keyvalue != null)
            {
                //将字符串转换为合法字符串
                FormatParamStr(ref keyvalue);
                return keyvalue;
            }
            return "";
        }
        #endregion

        #region 从Form栏获取值（key值字符串）
        /// <summary>
        ///  从URL栏获取值(key值字符串)
        /// </summary>
        public static string GetKeyFromForm(string key)
        {
            string keyvalue = HttpContext.Current.Request.Form[key];
            if (keyvalue != null && Utility.Strings.IsKeyString(keyvalue))
            {
                return keyvalue;
            }
            return "";
        }
        #endregion

        #region 从Form栏获取值(字符串)
        /// <summary>
        /// 增加
        ///     从Form栏获取值(字符串)
        ///     Faming 2007-8-9
        /// </summary>
        public static string GetValueFromForm(string key)
        {
            string keyvalue = HttpContext.Current.Request.Form[key];
            if (keyvalue != null)
            {
                //将字符串转换为合法字符串
                FormatParamStr(ref keyvalue);
                return keyvalue;
            }
            return "";
        }
        #endregion

        #region 获取在URL中，是否含有参数

        /// <summary>
        /// 获取在URL中，是否含有参数
        /// </summary>
        /// <param name="key">参数的名称</param>
        /// <returns></returns>
        public static bool Contains(string key)
        {
            return HttpContext.Current.Request.QueryString.AllKeys.Contains(key) || HttpContext.Current.Request.Form.AllKeys.Contains(key);
        }

        #endregion

        #region 地址参数格式化
        /// <summary>
        /// 将获取的地址参数格式化为合法字符串
        /// </summary>
        /// <param name="str"></param>
        private static void FormatParamStr(ref string str)
        {
            if (str == null)
            {
                str = "";
                return;
            }
            str = Regex.Replace(str, "'", "''");
            str = Regex.Replace(str, "insert[ ]+|update[ ]+|delete[ ]+|select[ ]+", "", RegexOptions.IgnoreCase);
        }

        #endregion

        #region 页面JS操作块

        /// <summary>
        /// 调用当前页面有权限调用的js方法
        /// </summary>
        /// <param name="method">方法名称</param>
        /// <param name="param">方法参数</param>
        public static void ControlCall(Page page, string method, params object[] param)
        {
            string methodName = FormatScriptName(method, param);
            HtmlOutPut(page, methodName);
        }

        /// <summary>
        /// JS提示后，返回上一步
        /// </summary>
        /// <param name="Message">弹出消息</param>
        /// <param name="scriptType">
        /// 默认:defalut
        /// 警告:warn
        /// 成功提示:success
        /// 错误警告:error
        /// 询问:question
        /// </param>
        public static void AlertOK(Page page, string alertMessage, string url = null)
        {
            FormatJsMessage(ref alertMessage);

            if (url == null)
            {
                HtmlOutPut(page, "submitCallBack('" + alertMessage + "');");
            }
            else
            {
                HtmlOutPut(page, "submitCallBack('" + alertMessage + "','" + url + "');");
            }
        }

        /// <summary>
        /// 提示消息，后跳转
        /// </summary>
        /// <param name="page"></param>
        /// <param name="alertMessage"></param>
        /// <param name="title"></param>
        /// <param name="scriptType"></param>
        /// <param name="url"></param>
        public static void ControlAlert(Page page, string alertMessage, string title, string scriptType, string url)
        {
            FormatJsMessage(ref alertMessage);
            HtmlOutPut(page, string.Format("windowAlert('{0}','{1}','{2}','{3}')", alertMessage, title, scriptType, url));
        }

        /// <summary>
        /// 格式化方法名称和参数
        /// </summary>
        /// <param name="method">方法名称</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        private static string FormatScriptName(string method, object[] param)
        {
            string methodName = method.Replace("();", "").Replace("()", "") + "(";

            if (param != null)
            {
                foreach (var item in param)
                {
                    methodName += string.Format("'{0}',", item);
                }
                methodName = methodName.TrimEnd(',');
            }
            methodName += ");";
            return methodName;
        }

        /// <summary>
        /// 系统输出JS跳转
        /// </summary>
        /// <param name="url"></param>
        public static void JavascriptGoTo(string url)
        {
            HttpContext.Current.Response.Write(@"<script type=""text/javascript"">location.href='" + url + "';</script>");
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// 向Page输出JS
        /// 并终止输出
        /// </summary>
        /// <param name="Html"></param>
        public static void HtmlOutPut(Page page, string html)
        {
            HttpContext.Current.Response.Write(@"<script type=""text/javascript"">");
            HttpContext.Current.Response.Write(html);
            HttpContext.Current.Response.Write("</script>");
            HttpContext.Current.Response.End();
        }

        public static void FormatJsMessage(ref string str)
        {
            if (str == null)
            {
                str = "";
                return;
            }
            str = Regex.Replace(str, "\n\r", "\\n\\r");
            str = Regex.Replace(str, "\r", "\\r");
            str = Regex.Replace(str, "\n", "\\n");
            str = Regex.Replace(str, "'", "\\'");
        }

        #endregion

        #region 获取网页消耗时间
        /// <summary>
        /// 获取网页消耗时间
        /// </summary>
        /// <returns></returns>
        public static int PageCostTime()
        {
            TimeSpan ts = DateTime.Now - System.Web.HttpContext.Current.Timestamp;
            return ts.Milliseconds;
        }
        #endregion
    }
}
