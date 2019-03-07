using System;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LT.Component.Web.Controls
{
    /// <summary>
    /// BooleanState 通用类
    /// </summary>
    public class BooleanState : Literal
    {
        /// <summary>
        /// 真假值字段名
        /// </summary>
        private string _column = "IsActivity";

        /// <summary>
        /// 输出值方式
        /// </summary>
        private Enums.BooleanStates _state = Enums.BooleanStates.True;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            object data = null;
            Control parent = this.Parent;
            if (parent is IDataItemContainer)
            {
                RepeaterItem ri = (RepeaterItem)parent;
                data = ri.DataItem;

                PropertyInfo pi = data.GetType().GetProperty(Column);
                if (pi != null)
                {
                    bool value = Convert.ToBoolean(pi.GetValue(data, null));
                    switch (State)
                    {
                        case Enums.BooleanStates.True:
                            if (value == true)
                            {
                                this.Text = "√";
                            }
                            break;
                        case Enums.BooleanStates.False:
                            if (value == false)
                            {
                                this.Text = "×";
                            }
                            break;
                        case Enums.BooleanStates.TrueFlase:
                            if (value == true)
                            {
                                this.Text = "<font color=\"blue\">√</font>";
                            }
                            else
                            {
                                this.Text = "<font color=\"red\">×</font>";
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 获取或设置真假值字段名
        /// </summary>
        public string Column
        {
            set { _column = value; }
            get { return _column; }
        }

        /// <summary>
        /// 获取或设置输出值方式
        /// </summary>
        public Enums.BooleanStates State
        {
            set { _state = value; }
            get { return _state; }
        }
    }
}
