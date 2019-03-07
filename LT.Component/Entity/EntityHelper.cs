using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace LT.Component.Entity
{
    /// <summary>
    /// EntityHelper 通用类
    /// </summary>
    public class EntityHelper
    {
        /// <summary>
        /// 获取对象的属性
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PropertyAttribute GetProperty(Type type)
        {
            return (PropertyAttribute)(type.GetCustomAttributes(false)[0]);
        }

        /// <summary>
        /// 从Model模型中获取数据表名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTableName(Type type)
        {
            var key = "TN_" + type.FullName;
            var name = ObjectCache.GetString(key);
            if (name == null)
            {
                var attributes = type.GetCustomAttributes(false);

                if (attributes != null && attributes.Length > 0)
                {
                    name = ((PropertyAttribute)(attributes[0])).TableName;
                    ObjectCache.SetString(key, name);
                }
            }
            return name;
        }

        /// <summary>
        /// 获取表中的主键
        /// </summary>
        /// <param name="pis"></param>
        /// <returns></returns>
        public static PropertyInfo GetTableIdentity(Type type)
        {
            var key = type.FullName;
            var info = ObjectCache.GetPropertyInfo(key);
            if (info == null)
            {
                PropertyAttribute attribute = null;

                foreach (PropertyInfo pi in type.GetProperties())
                {
                    if (Attribute.IsDefined(pi, typeof(PropertyAttribute)))
                    {
                        attribute = (PropertyAttribute)Attribute.GetCustomAttribute(pi, typeof(PropertyAttribute));

                        if (ColumnTypes.Identity == (ColumnTypes.Identity & attribute.ColumnType))
                        {
                            info = pi;
                        }
                    }
                }
                ObjectCache.SetPropertyInfo(key, info);
            }
            return info;
        }

        /// <summary>
        /// 获取需要的读取数据源的字段集
        /// </summary>
        /// <param name="pis">Model模型所有属性集合</param>
        /// <param name="filter"></param>
        /// <param name="customColumns">自定义查询列名集合，使用逗号分隔。如不需要则为null</param>
        /// <returns></returns>
        public static List<string> GetTableColumns(Type type, ColumnTypes filter, string customColumns)
        {
            var key = type.FullName + "_" + filter.ToString() + "_" + customColumns;
            var columns = ObjectCache.GetColumns(key);
            if (columns == null)
            {
                columns = new List<string>();
                if (customColumns == "*")
                {
                    columns.Add("*");
                }
                else if (customColumns != null && customColumns.Length > 0)
                {
                    string[] strs = customColumns.Split(',');
                    foreach (string str in strs)
                    {
                        columns.Add(str.Trim());
                    }
                }
                else
                {
                    PropertyInfo[] pis = type.GetProperties();
                    if (pis != null)
                    {
                        PropertyAttribute attribute;
                        ColumnTypes ct;
                        bool flag = false;
                        foreach (PropertyInfo pi in pis)
                        {
                            if (Attribute.IsDefined(pi, typeof(PropertyAttribute)))
                            {
                                attribute = (PropertyAttribute)Attribute.GetCustomAttribute(pi, typeof(PropertyAttribute));
                                string[] cts = attribute.ColumnType.ToString().Split(',');
                                foreach (string item in cts)
                                {
                                    ct = (ColumnTypes)Enum.Parse(typeof(ColumnTypes), item);
                                    if (ct == (filter & ct))
                                    {
                                        flag = true;
                                        break;
                                    }
                                }
                                if (flag == true)
                                {
                                    flag = false;
                                    continue;
                                }
                            }
                            columns.Add(pi.Name);
                        }
                    }
                }
                ObjectCache.SetColumns(key, columns);
            }
            return columns;
        }

        /// <summary>
        /// 获取DataReader中的数据集，形成对象返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="typeName"></param>
        /// <param name="columns"></param>
        /// <param name="sdr"></param>
        /// <returns></returns>
        public static T GetDataReaderObject<T>(Type type, string typeName, List<string> columns, IDataReader sdr) where T : class
        {
            int i = 0;
            PropertyInfo propertyInfo = null;
            T model = type.Assembly.CreateInstance(typeName) as T;
            foreach (string column in columns)
            {
                if (!sdr.IsDBNull(i))
                {
                    propertyInfo = type.GetProperty(column, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    propertyInfo.SetValue(model, sdr.GetValue(i), null);
                }
                i++;
            }

            return model;
        }
    }
}
