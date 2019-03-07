using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;

namespace LT.Component.Web
{
    /// <summary>
    /// TTreeDropdown 通用类
    /// </summary>
    public class TTreeDropdown
    {
        private int _depth;
        private string _keyId = "SortId";
        private string _keyName = "SortName";

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">原型对象</typeparam>
        /// <param name="dict">数据源</param>
        /// <param name="startId"></param>
        /// <param name="removeId">要去除的主键ID</param>
        /// <param name="dropdown">下拉对象</param>
        public void DataBind<T>(Dictionary<int, T> dict, int startId, string removeId, Controls.DropdownBase dropdown) where T : class
        {
            //判断是否含有数据
            if (dict == null || dict.Count == 0)
            {
                return;
            }

            DataRow[] drs;

            //获取数据表
            DataTable table = Generic.TTable.Get<T>(dict, _keyId);

            //处理删除项
            if (removeId != null)
            {
                drs = table.Select(_keyId + "='" + removeId +"'");
                if (drs.Length > 0)
                {
                    table.Rows.Remove(drs[0]);
                }
            }

            //输出载体
            List<TTreeDropdownModel> dropdowns = new List<TTreeDropdownModel>();

            //递归处理
            drs = table.Select("ParentId='" + startId + "'");
            GetTree(dropdowns, table, drs);

            //绑定数据
            int i = dropdown.Items.Count;
            foreach (TTreeDropdownModel model in dropdowns)
            {
                dropdown.Items.Add(new ListItem(model.text, model.value));
                if (model.value == dropdown.Value.ToString())
                {
                    dropdown.SelectedIndex = i;
                }
                i++;
            }
        }

        private void GetTree(List<TTreeDropdownModel> dropdowns, DataTable table, DataRow[] drs)
        {
            _depth++;
            DataRow dr = null;
            string treePath = null;
            int drsCount = drs.Length;

            for (int i = 0; i < drsCount; i++)
            {
                dr = drs[i];
                treePath = null;

                for (int k = 1; k < _depth; k++)
                {
                    treePath += "┆";
                }
                if((drsCount - 1) == i)
                {
                    treePath += "└ ";
                }
                else
                {
                    treePath += "├ "; 
                }

                dropdowns.Add(new TTreeDropdownModel(Convert.ToString(dr[0]), treePath + dr[_keyName]));

                DataRow[] drs1 = table.Select("ParentId=" + dr[0]);
                if (drs1.Length > 0)
                {
                    GetTree(dropdowns, table, drs1);
                }
            }

            _depth--;
        }

        /// <summary>
        /// 设置主键对应的ID字段
        /// </summary>
        public string KeyId
        {
            set { _keyId = value; }
        }

        /// <summary>
        /// 设置主键对应的名字字段
        /// </summary>
        public string KeyName
        {
            set { _keyName = value; }
        }
    }

    public class TTreeDropdownModel
    {
        private string _value;
        private string _text;

        public TTreeDropdownModel(string value, string text)
        {
            _value = value;
            _text = text;
        }

        public string value
        {
            get { return _value; }
        }

        public string text
        {
            get { return _text; }
        }
    }
}
