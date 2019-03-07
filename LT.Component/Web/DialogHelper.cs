using System.Web.UI;

namespace LT.Component.Web
{
    /// <summary>
    /// Dialog提示类，处理动态弹出对话框等
    /// 前端请用Js来验证处理弹出，
    /// 此类为后端处理弹出。
    /// </summary>
    public static class DialogHelper
    {
        #region 弹出对话框，跳出Iframe
        /// <summary>
        /// 弹出对话框，跳出Iframe
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="msg">内容</param>
        /// <param name="icon">提示类型</param>
        public static void ShowMessageThroughIframe(this System.Web.UI.Page page, string title, string msg, MessageIcon icon)
        {
            ScriptManager.RegisterClientScriptBlock(page, page.GetType(), "", "MyAlertThroughIframe('" + title + "','" + msg.ToString() + "','" + icon.ToString() + "');", true);
        }
        #endregion

        #region 弹出对话框
        /// <summary>
        /// 弹出对话框
        /// </summary>
        /// <param name="msg">内容</param>
        public static void ShowMessage(this System.Web.UI.Page page, string msg)
        {
            ScriptManager.RegisterClientScriptBlock(page, page.GetType(), "", "MyAlert('','" + msg.ToString() + "','" + MessageIcon.Info.ToString() + "');", true);
        }

        /// <summary>
        /// 弹出对话框
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="msg">内容</param>
        public static void ShowMessage(this System.Web.UI.Page page, string title, string msg)
        {
            ScriptManager.RegisterClientScriptBlock(page, page.GetType(), "", "MyAlert('" + title + "','" + msg.ToString() + "','" + MessageIcon.Info.ToString() + "');", true);
        }

        /// <summary>
        /// 弹出对话框
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="msg">内容</param>
        /// <param name="icon">提示类型</param>
        public static void ShowMessage(this System.Web.UI.Page page, string title, string msg, MessageIcon icon)
        {
            ScriptManager.RegisterClientScriptBlock(page, page.GetType(), "", "MyAlert('" + title + "','" + msg.ToString() + "','" + icon.ToString() + "');", true);
        }
        #endregion
    }

    /// <summary>
    /// 提示类型枚举
    /// </summary>
    public enum MessageIcon
    {
        Info,
        Succeed,
        Warning,
        Error
    }
}
