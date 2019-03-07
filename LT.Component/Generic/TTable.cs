/*******************************************************************************************
* 00: ---------------------------------------------------------------
* 01: LT.Component.Generic.TTable
* 02: TTable 通用类
* 03: Devia
* 04: 2010-08-26 10 03 58
* 05: 20100826-10
* 06: 
*******************************************************************************************/

using System;
using System.Data;
using System.Collections.Generic;
using System.Reflection;

namespace LT.Component.Generic
{

    /// <summary>
    /// TTable 通用类
    /// </summary>
    public class TTable
    {
        private static Dictionary<int, DataTable> _tables = new Dictionary<int, DataTable>();

        /// <summary>
        /// 根据DICT字典模型转换为DataTable对象
        /// 默认第一个字段必须为主键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static DataTable Get<T>(Dictionary<int, T> dict, string keyId) where T : class
        {
            //从缓存中获取
            int hash = dict.GetHashCode();
            if (_tables.ContainsKey(hash))
            {
                return _tables[hash];
            }

            //获取第一条记录
            T first = null;
            foreach (T t in dict.Values)
            {
                first = t;
                break;
            }
            PropertyInfo[] pis = first.GetType().GetProperties();

            //声时结构
            string columnName;
            DataTable table = new DataTable();
            List<string> columns = new List<string>();

            //添加第一行主键
            foreach (PropertyInfo pi in pis)
            {
                columnName = pi.Name;
                if (columnName == keyId)
                { 
                    columns.Add(columnName);
                    table.Columns.Add(columnName, pi.PropertyType);
                    break;
                }
            }

            //判断是否加了主键
            if (columns.Count == 0)
            {
                throw new Exception("There is no identity column.");
            }

            //添加其它行
            foreach (PropertyInfo pi in pis)
            {
                columnName = pi.Name;
                if (columnName == keyId)
                {
                    continue;
                }
                columns.Add(columnName);
                table.Columns.Add(columnName, pi.PropertyType);
            }

            //添加数据
            int i = 0;
            int j = columns.Count;
            Type type;
            DataRow dr;

            foreach (T t in dict.Values)
            {
                type = t.GetType();
                dr = table.NewRow();

                for (i = 0; i < j; i++)
                {
                    dr[i] = type.GetProperty(columns[i]).GetValue(t, null);
                }

                table.Rows.Add(dr);
            }

            //返回
            return table;
        }

        /// <summary>
        /// 根据ILIST字典模型转换为DataTable对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable Get<T>(List<T> list, string keyId) where T : class
        {
            //从缓存中获取
            int hash = list.GetHashCode();
            if (_tables.ContainsKey(hash))
            {
                return _tables[hash];
            }

            //获取第一条记录
            T first = list[0];
            PropertyInfo[] pis = first.GetType().GetProperties();

            //声时结构
            string columnName;
            DataTable table = new DataTable();
            List<string> columns = new List<string>();

            //添加第一行主键
            foreach (PropertyInfo pi in pis)
            {
                columnName = pi.Name;
                if (columnName == keyId)
                {
                    columns.Add(columnName);
                    table.Columns.Add(columnName, pi.PropertyType);
                    break;
                }
            }

            //添加其它行
            foreach (PropertyInfo pi in pis)
            {
                columnName = pi.Name;
                if (columnName == keyId)
                {
                    continue;
                }
                columns.Add(columnName);
                table.Columns.Add(columnName, pi.PropertyType);
            }

            //添加数据
            int i = 0;
            int j = columns.Count;
            Type type;
            DataRow dr;

            foreach (T t in list)
            {
                type = t.GetType();
                dr = table.NewRow();

                for (i = 0; i < j; i++)
                {
                    dr[i] = type.GetProperty(columns[i]).GetValue(t, null);
                }

                table.Rows.Add(dr);
            }

            //返回
            return table;
        }
    }
}
