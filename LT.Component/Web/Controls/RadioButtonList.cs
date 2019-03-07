using System.Collections;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LT.Component.Web.Controls
{
    /// <summary>
    /// RadioButtonList 通用类
    /// </summary>
    public class RadioButtonList : System.Web.UI.WebControls.PlaceHolder
    {
        protected override void Render(HtmlTextWriter writer)
        {
            StringBuilder sb = new StringBuilder();

            int i = 0;
            bool flag = false;
            if (_dataTextField != null && _dataValueField != null)
            {
                flag = true;
            }

            string id = this.UniqueID;
            string value;
            string text;

            foreach (object item in _dataSource)
            {
                i++;

                if (flag == true)
                {
                    value = DataBinder.GetPropertyValue(item, _dataValueField, null);
                    text = DataBinder.GetPropertyValue(item, _dataTextField, null);
                }
                else
                {
                    value = item.ToString();
                    text = value;
                }
                switch (RepeatDirection)
                {
                    case RepeatDirection.Horizontal:
                        sb.Append(" <span>");
                        break;
                    case RepeatDirection.Vertical:
                        sb.Append(" <div>");
                        break;
                    default:
                        break;
                }
                sb.Append("<input type=\"radio\" id=\"");
                sb.Append(id);
                sb.Append("_");
                sb.Append(i);
                sb.Append("\" name=\"");
                sb.Append(id);
                sb.Append("\" value=\"");
                sb.Append(value);
                sb.Append("\"");

                //选中的所选列
                if (_value == value)
                {
                    sb.Append(" checked=\"checked\"");
                }
                if (this.Disabled)
                {

                    sb.Append(" disabled=\"disabled\"");
                }

                sb.Append("><label for=\"");
                sb.Append(id);
                sb.Append("_");
                sb.Append(i);
                sb.Append("\">");
                sb.Append(text);
                sb.Append("</label>");
                switch (RepeatDirection)
                {
                    case RepeatDirection.Horizontal:
                        sb.Append(" </span>");
                        break;
                    case RepeatDirection.Vertical:
                        sb.Append(" </div>");
                        break;
                    default:
                        break;
                }
            }

            //输出
            Literal literal = new Literal();
            literal.Text = sb.ToString();
            this.Controls.Add(literal);

            base.Render(writer);
        }

        /// <summary>
        /// DataTextField
        /// </summary>
        public string DataTextField
        {
            set { _dataTextField = value; }
        }
        private string _dataTextField;

        /// <summary>
        /// DataValueField
        /// </summary>
        public string DataValueField
        {
            set { _dataValueField = value; }
        }
        private string _dataValueField;

        /// <summary>
        /// Checkbox列表数据源
        /// </summary>
        public IEnumerable DataSource
        {
            set { _dataSource = value; }
        }
        private IEnumerable _dataSource;

        /// <summary>
        /// 获取或设置选择的值
        /// </summary>
        public string Value
        {
            get
            {
                return HttpContext.Current.Request.Form[this.UniqueID];
            }
            set
            {
                _value = value;
            }
        }
        private string _value;

        public RepeatDirection RepeatDirection
        {
            get;
            set;
        }

        public bool Disabled { get; set; }
    }
}
