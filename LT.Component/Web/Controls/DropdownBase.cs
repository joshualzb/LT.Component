using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;

namespace LT.Component.Web.Controls
{
    /// <summary>
    /// DropdownBase 通用类
    /// </summary>
    public class DropdownBase : System.Web.UI.WebControls.DropDownList
    {
        public bool ShowSelectTip { get; set; }

        /// <summary>
        /// 向dropdownlist 增加选择项目
        /// </summary>
        /// <param name="text"></param>
        /// <param name="value"></param>
        public void AddOption(string text, string value)
        {
            this.Items.Add(new ListItem(text, value));
        }

        /// <summary>
        /// 向dropdownlist 增加选择项目
        /// </summary>
        /// <param name="text"></param>
        /// <param name="value"></param>
        /// <param name="bgcolor">项目背景颜色</param>
        public void AddOption(string text, string value, string bgcolor)
        {
            this.Items.Add(new ListItem(text, value));
            this.Items[this.Items.Count - 1].Attributes.Add("style", "background-color:" + bgcolor);
        }

        protected override void OnPreRender(EventArgs e)
        {
            //通过数据源绑定数据
            if (_dataSource != null)
            {
                string value;
                string text;
                bool flag = false;

                if (DataTextField != null && DataValueField != null)
                {
                    flag = true;
                }

                foreach (object item in _dataSource)
                {
                    if (flag == true)
                    {
                        value = DataBinder.GetPropertyValue(item, DataValueField, null);
                        text = DataBinder.GetPropertyValue(item, DataTextField, null);
                    }
                    else
                    {
                        value = item.ToString();
                        text = value;
                    }
                    this.Items.Add(new ListItem(text, value));
                }
            }

            //从URL绑定选中项
            if (string.IsNullOrEmpty(Value))
            {
                if (_bindUrlKey != null)
                {
                    _value = HttpContext.Current.Request.QueryString[_bindUrlKey];
                }
            }

            this.SelectedValue = Value;
            base.OnPreRender(e);
        }

        /// <summary>
        /// Checkbox列表数据源
        /// </summary>
        public new IEnumerable DataSource
        {
            set { _dataSource = value; }
        }
        private IEnumerable _dataSource;

        /// <summary>
        /// 获取或设置下拉框选择的字符型 value 值
        /// </summary>
        public string Value
        {
            set
            {
                _value = value;
            }
            get
            {
                if (_value == null)
                {
                    return HttpContext.Current.Request[this.UniqueID];
                }
                else
                {
                    return _value;
                }
            }
        }
        private string _value;

        /// <summary>
        /// 获取外部传入值的URL KEY
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
