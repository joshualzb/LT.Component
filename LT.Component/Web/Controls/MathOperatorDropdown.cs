using System;

namespace LT.Component.Web.Controls
{
    /// <summary>
    /// 数学操作符下拉框
    /// </summary>
    public class MathOperatorDropdown : DropdownBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.AddOption("[>]", ">");
            this.AddOption("[<]", "<");
            this.AddOption("[=]", "=");
            this.AddOption("[>=]", ">=");
            this.AddOption("[<=]", "<=");
            this.AddOption("[!=]", "!=");
        }
    }
}
