using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LT.Component.Web
{
    /// <summary>
    /// TRepeater 通用类
    /// </summary>
    public class TRepeater
    {
        /// <summary>
        /// 获取Repeater父容器的行数据对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c">当前对象，使用this传入</param>
        /// <returns></returns>
        public static T GetDataItem<T>(Control c) where T : class
        {
            object data = null;
            Control parent = c.Parent;
            if (parent is IDataItemContainer)
            {
                RepeaterItem ri = (RepeaterItem)parent;
                data = ri.DataItem;
            }

            if (data == null)
            {
                throw new Exception("RepeaterItem is null, please put this control at repeater's itemplate inside.");
            }

            //转换为目标对象
            if (data is T)
            {
                return (T)data;
            }
            else
            {
                throw new Exception("The DataItem value does not match T mode.");
            }
        }

        /// <summary>
        /// 获取Repeater父容器的某个属性值
        /// </summary>
        /// <param name="c">当前对象，使用this传入</param>
        /// <param name="column">需要获取的字段值</param>
        /// <returns></returns>
        public static object GetDataValue(Control c, string column)
        {
            object data = null;
            Control parent = c.Parent;
            if (parent is IDataItemContainer)
            {
                RepeaterItem ri = (RepeaterItem)parent;
                data = ri.DataItem;
            }

            //通过反射获取值并返回值
            return data.GetType().GetProperty(column).GetValue(data, null);
        }
    }
}
