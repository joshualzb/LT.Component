using System.Web;

namespace LT.Component.Web.Controls
{
    /// <summary>
    /// TextBox
    /// </summary>
    public class TextBox : System.Web.UI.WebControls.TextBox
    {
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            if (_bindUrlKey != null)
            {
                this.Text = HttpContext.Current.Request.QueryString[_bindUrlKey];
            }
        }

        /// <summary>
        /// 获取外部传入的URL KEY
        /// </summary>
        public string BindUrlKey
        {
            set
            {
                _bindUrlKey = value;
            }
        }
        private string _bindUrlKey;
    }
}
