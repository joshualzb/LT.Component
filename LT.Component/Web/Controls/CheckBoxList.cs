using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LT.Component.Web.Controls
{
    /// <summary>
    /// CheckBoxList 通用类
    /// </summary>
    public class CheckBoxList : System.Web.UI.WebControls.PlaceHolder
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

                sb.Append(" <div style=\"");
                sb.Append(_style);
                sb.Append("\"><input type=\"checkbox\" id=\"");
                sb.Append(id);
                sb.Append("_");
                sb.Append(i);
                sb.Append("\" name=\"");
                sb.Append(id);
                sb.Append("\" value=\"");
                sb.Append(value);
                sb.Append("\"");

                //选中的所选列
                if (_checkedAll == true || (_value != null && _value.Contains(value)))
                {
                    sb.Append(" checked=\"checked\"");
                }

                sb.Append("><label for=\"");
                sb.Append(id);
                sb.Append("_");
                sb.Append(i);
                sb.Append("\">");
                sb.Append(text);
                sb.Append("</label></div>");
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
        /// 设置Style
        /// </summary>
        public string Style
        {
            set { _style = value; }
        }
        private string _style;

        /// <summary>
        /// Checkbox列表数据源
        /// </summary>
        public IEnumerable DataSource
        {
            set { _dataSource = value; }
        }
        private IEnumerable _dataSource;

        /// <summary>
        /// 设置初始化值是否全选
        /// </summary>
        public bool CheckedAll
        {
            set
            {
                _checkedAll = value;
            }
        }
        private bool _checkedAll = false;

        /// <summary>
        /// 获取和设置选择的列值（逗号分隔）
        /// </summary>
        public string Selected
        {
            get
            {
                return Web.PageHelper.GetValueFromForm(this.UniqueID);
            }
            set
            {
                List<string> list = new List<string>();
                if (!string.IsNullOrEmpty(value))
                {
                    string[] values = value.Split(',');
                    foreach (string val in values)
                    {
                        list.Add(val);
                    }
                }

                _value = list;
            }
        }

        /// <summary>
        /// 获取或设置选择的列
        /// </summary>
        public List<string> Value
        {
            get
            {
                string value = Web.PageHelper.GetValueFromForm(this.UniqueID);

                //未选任何一项，返回null值
                if (value == null || value == "")
                {
                    return null;
                }

                string[] values = value.Split(',');

                List<string> list = new List<string>();
                foreach (string val in values)
                {
                    list.Add(val);
                }
                return list;
            }
            set
            {
                _value = value;
            }
        }
        private List<string> _value;
    }
}
