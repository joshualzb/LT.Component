using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace LT.Component.Web
{

    /// <summary>
    /// TSingleDropdown 通用类
    /// </summary>
    public class TSingleDropdown
    {
        public void DataBind<T>(List<T> list, string removeId, Controls.DropdownBase dropdown) where T : class
        {
            Type type;
            string text;
            string value;
            string selected = dropdown.Value;

            int i = dropdown.Items.Count;
            foreach (T t in list)
            {
                type = t.GetType();
                value = type.GetProperty(_keyId).GetValue(t, null).ToString();
                if (removeId == value)
                {
                    continue;
                }

                text = Utility.Strings.HtmlDecode(type.GetProperty(_keyName).GetValue(t, null).ToString());

                dropdown.Items.Add(new ListItem(text, value));
                if (value == selected)
                {
                    dropdown.SelectedIndex = i;
                }
                i++;
            }
        }

        /// <summary>
        /// 设置主键对应的ID字段
        /// </summary>
        public string KeyId
        {
            set { _keyId = value; }
        }
        private string _keyId = "SortId";

        /// <summary>
        /// 设置主键对应的名字字段
        /// </summary>
        public string KeyName
        {
            set { _keyName = value; }
        }
        private string _keyName = "SortName";
    }
}
