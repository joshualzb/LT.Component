using System;
using System.Collections.Generic;
using System.Data;

namespace LT.Component.Web
{
    /// <summary>
    /// TOrderAct 通用类
    /// </summary>
    public class TOrderAct
    {
        /// <summary>
        /// 向上
        /// </summary>
        public static string upText = "∧";

        /// <summary>
        /// 向下
        /// </summary>
        public static string downText = "∨";

        /// <summary>
        /// 禁用Link
        /// </summary>
        public static string disableAct = "<font color=\"#666666\">{0}</font>";

        /// <summary>
        /// 可用Link
        /// </summary>
        public static string enableAct = "<a href=\"#\" onclick=\"order(this.parent, '{0}', '{1}')\" style=\"text-decoration:none;\">{2}</a>";

        /// <summary>
        /// 向对象的OrderAct字段加入排序按钮
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="key">主键字字段名称</param>
        public static void Set<T>(List<T> list, string key) where T : class
        {
            var len = list.Count;
            if (len > 0)
            {
                Type type = typeof(T);

                if (len == 1)
                {
                    type.GetProperty("OrderAct").SetValue(list[0], string.Format("{0} &nbsp; {1}", string.Format(disableAct, upText), string.Format(disableAct, downText)), null);
                }
                else
                {
                    //第一个
                    T t0 = list[0];
                    T t1 = list[1];
                    T t2 = null;

                    type.GetProperty("OrderAct").SetValue(t0, string.Format("{0} &nbsp; {1}", string.Format(disableAct, upText), string.Format(enableAct, getModelValue(type, t0, key), getModelValue(type, t1, key), downText)), null);

                    //中间的
                    for (int i = 1; i < (len - 1); i++)
                    {
                        t0 = list[i - 1];
                        t1 = list[i];
                        t2 = list[i + 1];
                        type.GetProperty("OrderAct").SetValue(t1, string.Format("{0} &nbsp; {1}", string.Format(enableAct, getModelValue(type, t1, key), getModelValue(type, t0, key), upText), string.Format(enableAct, getModelValue(type, t1, key), getModelValue(type, t2, key), downText)), null);
                    }

                    //最后一个
                    var last = len - 1;
                    t0 = list[last - 1];
                    t1 = list[last];
                    type.GetProperty("OrderAct").SetValue(t1, string.Format("{0} &nbsp; {1}", string.Format(enableAct, getModelValue(type, t1, key), getModelValue(type, t0, key), upText), string.Format(disableAct, downText)), null);
                }
            }
        }

        /// <summary>
        /// 获取对象中的值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="model"></param>
        /// <param name="name">字段名称</param>
        /// <returns></returns>
        static object getModelValue(Type type, object model, string name)
        {
            return type.GetProperty(name).GetValue(model, null);
        }

        /// <summary>
        /// 调整排序
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key">主键字字段名称</param>
        public static void Set(DataTable data, string key)
        {
            if (data.Columns.IndexOf("OrderAct") == -1)
            {
                data.Columns.Add("OrderAct", typeof(string));
            }

            var idx = data.Columns.Count - 1;
            var drs = data.Rows;
            var len = drs.Count;

            if (len > 0)
            {
                if (len == 1)
                {
                    drs[0][idx] = string.Format("{0} | {1}", string.Format(disableAct, upText), string.Format(disableAct, downText));
                }
                else
                {
                    //第一个
                    drs[0][idx] = string.Format("{0} | {1}", string.Format(disableAct, upText), string.Format(enableAct, drs[0][key], drs[1][key], downText));

                    //中间的
                    for (int i = 1; i < (len - 1); i++)
                    {
                        drs[i][idx] = string.Format("{0} | {1}", string.Format(enableAct, drs[i][key], drs[i - 1][key], upText), string.Format(enableAct, drs[i][key], drs[i + 1][key], downText));
                    }

                    //最后一个
                    var last = len - 1;
                    drs[last][idx] = string.Format("{0} | {1}", string.Format(enableAct, drs[last][key], drs[last - 1][key], upText), string.Format(disableAct, downText));
                }
            }
        }

        /// <summary>
        /// 调整排序
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startId"></param>
        /// <param name="key">主键字字段名称</param>
        public static void Set(DataTable data, object startId, string key)
        {
            if (data.Columns.IndexOf("OrderAct") == -1)
            {
                data.Columns.Add("OrderAct", typeof(string));
            }

            var idx = data.Columns.Count - 1;
            var drs = data.Select("ParentId='" + startId + "'");
            var len = drs.Length;

            if (len > 0)
            {
                if (len == 1)
                {
                    drs[0][idx] = string.Format("{0} | {1}", string.Format(disableAct, upText), string.Format(disableAct, downText));
                }
                else
                {
                    //第一个
                    drs[0][idx] = string.Format("{0} | {1}", string.Format(disableAct, upText), string.Format(enableAct, drs[0][key], drs[1][key], downText));

                    //中间的
                    for (int i = 1; i < (len - 1); i++)
                    {
                        drs[i][idx] = string.Format("{0} | {1}", string.Format(enableAct, drs[i][key], drs[i - 1][key], upText), string.Format(enableAct, drs[i][key], drs[i + 1][key], downText));
                    }

                    //最后一个
                    var last = len - 1;
                    drs[last][idx] = string.Format("{0} | {1}", string.Format(enableAct, drs[last][key], drs[last - 1][key], upText), string.Format(disableAct, downText));
                }

                foreach (DataRow item in drs)
                {
                    Set(data, item[key], key);
                }
            }
        }
    }
}
