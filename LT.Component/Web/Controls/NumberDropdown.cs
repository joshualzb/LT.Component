using System;
using System.Web.UI.WebControls;

namespace LT.Component.Web.Controls
{
    /// <summary>
    /// NumberDropdown 通用类
    /// </summary>
    public class NumberDropdown : DropdownBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!string.IsNullOrEmpty(_defaultText))
            { 
                this.Items.Add(new ListItem(_defaultText, _defaultValue));
            }

            if (_len > 0)
            {
                for (int i = _min; i <= _max; i += _step)
                {
                    this.Items.Add(i.ToString().PadLeft(_len, '0'));
                }
            }
            else
            {
                for (int i = _min; i <= _max; i += _step)
                {
                    this.Items.Add(i.ToString());
                }
            }
        }

        /// <summary>
        /// 设置步长，默认为1
        /// </summary>
        public int Step
        {
            set
            {
                _step = value;
            }
        }
        private int _step = 1;

        /// <summary>
        /// 设置字符长度，默认为0，表示自动
        /// </summary>
        public int Len
        {
            set
            {
                _len = value;
            }
        }
        private int _len = 0;

        /// <summary>
        /// 设置最大值,默认为10
        /// </summary>
        public int Max
        {
            set
            {
                _max = value;
            }
        }
        private int _max = 10;


        /// <summary>
        /// 设置最小值，默认为0
        /// </summary>
        public int Min
        {
            set
            {
                _min = value;
            }
        }
        private int _min = 0;


        /// <summary>
        /// 设置第一个下拉框文本
        /// </summary>
        public string DefaultText
        {
            set
            {
                _defaultText = value;
            }
        }
        private string _defaultText;

        /// <summary>
        /// 设置第一个下拉框值
        /// </summary>
        public string DefaultValue
        {
            set
            {
                _defaultValue = value;
            }
        }
        private string _defaultValue;
    }
}
