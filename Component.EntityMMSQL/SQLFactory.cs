using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using LT.Component.Entity;


namespace LT.Component.EntityMSSQL
{
    /// <summary>
    /// SQLFactory 通用类
    /// </summary>
    public class SQLFactory : ISQLFactory
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private string connection = null;

        /// <summary>
        ///构造函数
        /// </summary>
        /// <param name="connection"></param>
        public SQLFactory(string connection)
        {
            this.connection = connection;
        }

        #region GetExecuteScalarByKey 根据主键返回指定一个字段的一个值
        /// <summary>
        /// 根据主键返回指定一个字段的一个值
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="keyValue">数据表主键的值</param>
        /// <param name="customKey">自定义临时主键。如不设置则使用表默认值</param>
        /// <param name="customColumn">自定义的字段名，只能一个</param>
        /// <returns></returns>
        object ISQLFactory.GetExecuteScalarByKey<T>(object keyValue, string customKey, string customColumn)
        {
            Type type = typeof(T);

            //获取表名
            string tableName = EntityHelper.GetTableName(type);

            //获取字段
            List<string> columns = new List<string> { customColumn };

            //获取指定的字段
            bool quote = false;
            PropertyInfo keyColumn = (customKey == null) ? EntityHelper.GetTableIdentity(type) : type.GetProperty(customKey);
            GetDbType(keyColumn.PropertyType, ref quote);

            //生成SELECT语句
            StringBuilder sqlText = new StringBuilder();

            //获取基本SELECT部分
            GetBaseSelect(sqlText, columns, tableName, "", 1);

            //处理 WHERE 语句
            sqlText.Append(" WHERE ");
            sqlText.Append(keyColumn.Name);
            sqlText.Append("=");

            //处理是否需要'连接
            if (quote)
            {
                sqlText.Append("'");
                sqlText.Append(keyValue);
                sqlText.Append("'");
            }
            else
            {
                sqlText.Append(keyValue);
            }

            object value = null;

            //从数据库中获取数据
            SqlDataReader sdr = SQLHelper.DataReader(connection, sqlText.ToString(), CommandType.Text, null);
            if (sdr.Read())
            {
                value = sdr.GetValue(0);
            }
            sdr.Close();

            //返回数据
            return value;
        }
        #endregion

        #region GetExecuteScalarByWhere 根据条件返回指定一个字段的一个值
        /// <summary>
        /// 根据条件返回指定一个字段的一个值
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="where">查询条件，必须以 AND 开头，如 AND SortId=1</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=[@SortId] or [@SortId]</param>
        /// <param name="orderBy">排序，如 LogId DESC 或 LogId DESC,IsShow ASC</param>
        /// <param name="customColumn">自定义的字段名，只能一个</param>
        /// <returns></returns>
        object ISQLFactory.GetExecuteScalarByWhere<T>(string where, DbParameter[] parms, string orderBy, string customColumn)
        {
            Type type = typeof(T);

            //获取表名
            string tableName = EntityHelper.GetTableName(type);

            //获取指定的字段
            List<string> columns = new List<string> { customColumn };

            //处理 WHERE 语句
            if (!string.IsNullOrEmpty(where))
            {
                where = " WHERE " + where.Substring(4);
            }

            //生成SELECT语句
            StringBuilder sqlText = new StringBuilder();

            //获取基本SELECT部分
            GetBaseSelect(sqlText, columns, tableName, where, 1);

            //形成排序语句
            if (!string.IsNullOrEmpty(orderBy))
            {
                sqlText.Append(" ORDER BY ");
                sqlText.Append(orderBy);
            }

            object value = null;

            //从数据库中获取数据
            SqlDataReader sdr = SQLHelper.DataReader(connection, sqlText.ToString(), CommandType.Text, parms);
            if (sdr.Read())
            {
                value = sdr.GetValue(0);
            }
            sdr.Close();

            //返回数据
            return value;
        }
        #endregion

        #region GetModelSingleByWhere 根据条件获取首行记录对象
        /// <summary>
        /// 根据条件获取首行记录对象
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="where">查询条件，必须以 AND 开头，如 AND SortId=1</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=[@SortId] or [@SortId]</param>
        /// <param name="orderBy">排序，如 LogId DESC 或 LogId DESC,IsShow ASC</param>
        /// <param name="customColumns">自定义查询列名集合，使用逗号分隔。如不需要则为null</param>
        /// <returns></returns>
        T ISQLFactory.GetModelSingleByWhere<T>(string where, DbParameter[] parms, string orderBy, string customColumns)
        {
            Type type = typeof(T);

            //获取表名
            string tableName = EntityHelper.GetTableName(type);

            //获取所有字段和主键名称
            List<string> columns = EntityHelper.GetTableColumns(type, ColumnTypes.Extend, customColumns);

            //获取主键
            PropertyInfo keyColumn = EntityHelper.GetTableIdentity(type);

            //处理 WHERE 语句
            if (!string.IsNullOrEmpty(where))
            {
                where = " WHERE " + where.Substring(4);
            }

            //生成SELECT语句
            StringBuilder sqlText = new StringBuilder();

            //获取基本SELECT部分
            GetBaseSelect(sqlText, columns, tableName, where, 1);

            //形成排序语句
            if (!string.IsNullOrEmpty(orderBy))
            {
                sqlText.AppendFormat(" ORDER BY {0}", orderBy);
            }

            string typeName = type.Namespace + "." + type.Name;
            T model = null;

            //从数据库中获取数据
            SqlDataReader sdr = SQLHelper.DataReader(connection, sqlText.ToString(), CommandType.Text, parms);
            if (sdr.Read())
            {
                model = EntityHelper.GetDataReaderObject<T>(type, typeName, columns, sdr);
            }
            sdr.Close();

            //返回数据
            return model;
        }
        #endregion

        #region GetTableSingleByWhere 根据条件获取首行记录对象
        /// <summary>
        /// 根据条件获取首行记录对象
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="where">查询条件，必须以 AND 开头，如 AND SortId=1</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=[@SortId] or [@SortId]</param>
        /// <param name="orderBy">排序，如 LogId DESC 或 LogId DESC,IsShow ASC</param>
        /// <param name="customColumns">自定义查询列名集合，使用逗号分隔。如不需要则为null</param>
        /// <returns></returns>
        DataTable ISQLFactory.GetTableSingleByWhere<T>(string where, DbParameter[] parms, string orderBy, string customColumns)
        {
            Type type = typeof(T);

            //获取表名
            string tableName = EntityHelper.GetTableName(type);

            //获取所有字段和主键名称
            List<string> columns = EntityHelper.GetTableColumns(type, ColumnTypes.Extend, customColumns);

            //获取主键
            PropertyInfo keyColumn = EntityHelper.GetTableIdentity(type);

            //处理 WHERE 语句
            if (!string.IsNullOrEmpty(where))
            {
                where = " WHERE " + where.Substring(4);
            }

            //生成SELECT语句
            StringBuilder sqlText = new StringBuilder();

            //获取基本SELECT部分
            GetBaseSelect(sqlText, columns, tableName, where, 1);

            //形成排序语句
            if (!string.IsNullOrEmpty(orderBy))
            {
                sqlText.AppendFormat(" ORDER BY {0}", orderBy);
            }

            //返回数据
            return SQLHelper.DataSet(connection, sqlText.ToString(), CommandType.Text, parms).Tables[0];
        }
        #endregion

        #region GetModelSingleByKey 通过主键获取单个对象
        /// <summary>
        /// 通过主键获取单个对象
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="keyValue">数据表主键的值</param>
        /// <param name="customKey">自定义临时主键。如不设置则使用表默认值</param>
        /// <param name="customColumns">自定义查询列名集合，使用逗号分隔。如不需要则为null</param>
        /// <returns></returns>
        T ISQLFactory.GetModelSingleByKey<T>(object keyValue, string customKey, string customColumns)
        {
            Type type = typeof(T);

            //获取表名
            string tableName = EntityHelper.GetTableName(type);

            //获取所有字段和主键名称
            List<string> columns = EntityHelper.GetTableColumns(type, ColumnTypes.Extend, customColumns);

            //获取主键
            bool quote = false;
            PropertyInfo keyColumn = (customKey == null) ? EntityHelper.GetTableIdentity(type) : type.GetProperty(customKey);
            GetDbType(keyColumn.PropertyType, ref quote);

            //生成SELECT语句
            StringBuilder sqlText = new StringBuilder();

            //获取基本SELECT部分
            GetBaseSelect(sqlText, columns, tableName, "", 1);

            //处理 WHERE 语句
            sqlText.Append(" WHERE ");
            sqlText.Append(keyColumn.Name);
            sqlText.Append("=");

            //处理是否需要'连接
            if (quote)
            {
                sqlText.Append("'");
                sqlText.Append(keyValue);
                sqlText.Append("'");
            }
            else
            {
                sqlText.Append(keyValue);
            }

            string typeName = type.Namespace + "." + type.Name;
            T model = null;

            //从数据库中获取数据
            SqlDataReader sdr = SQLHelper.DataReader(connection, sqlText.ToString(), CommandType.Text, null);
            if (sdr.Read())
            {
                model = EntityHelper.GetDataReaderObject<T>(type, typeName, columns, sdr);
            }
            sdr.Close();

            //返回数据
            return model;
        }
        #endregion

        #region GetTableSingleByKey 通过主键获取单个对象
        /// <summary>
        /// 通过主键获取单个对象
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="keyValue">数据表主键的值</param>
        /// <param name="customKey">自定义临时主键。如不设置则使用表默认值</param>
        /// <param name="customColumns">自定义查询列名集合，使用逗号分隔。如不需要则为null</param>
        /// <returns></returns>
        DataTable ISQLFactory.GetTableSingleByKey<T>(object keyValue, string customKey, string customColumns)
        {
            Type type = typeof(T);

            //获取表名
            string tableName = EntityHelper.GetTableName(type);

            //获取所有字段和主键名称
            List<string> columns = EntityHelper.GetTableColumns(type, ColumnTypes.Extend, customColumns);

            //获取主键
            bool quote = false;
            PropertyInfo keyColumn = (customKey == null) ? EntityHelper.GetTableIdentity(type) : type.GetProperty(customKey);
            GetDbType(keyColumn.PropertyType, ref quote);

            //生成SELECT语句
            StringBuilder sqlText = new StringBuilder();

            //获取基本SELECT部分
            GetBaseSelect(sqlText, columns, tableName, "", 1);

            //处理 WHERE 语句
            sqlText.Append(" WHERE ");
            sqlText.Append(keyColumn.Name);
            sqlText.Append("=");

            //处理是否需要'连接
            if (quote)
            {
                sqlText.Append("'");
                sqlText.Append(keyValue);
                sqlText.Append("'");
            }
            else
            {
                sqlText.Append(keyValue);
            }

            //返回数据
            return SQLHelper.DataSet(connection, sqlText.ToString(), CommandType.Text, null).Tables[0];
        }
        #endregion

        #region GetModelIntDictionary 获取数据字典对象（Key值限制为INT型，且在Model模型中排第一位）
        /// <summary>
        /// 获取数据字典对象（Key值限制为INT型，且在Model模型中排第一位）
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="pageSize">记录条数，0表示全部读取</param>
        /// <param name="where">查询条件，必须以 AND 开头，如 AND SortId=1 AND IsShow=1</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=@SortId</param>
        /// <param name="orderBy">排序，如 LogId DESC 或 LogId DESC,IsShow ASC</param>
        /// <param name="customColumns">自定义查询列名集合，使用逗号分隔。如不需要则为null</param>
        /// <returns></returns>
        Dictionary<int, T> ISQLFactory.GetModelIntDictionary<T>(int pageSize, string where, DbParameter[] parms, string orderBy, string customColumns)
        {
            Type type = typeof(T);

            //获取表名
            string tableName = EntityHelper.GetTableName(type);

            //获取所有字段
            List<string> columns = EntityHelper.GetTableColumns(type, ColumnTypes.Extend, customColumns);

            //获取主键
            PropertyInfo keyColumn = EntityHelper.GetTableIdentity(type);

            //查找主键索引位置
            int keyIndex = columns.IndexOf(keyColumn.Name);
            if (keyIndex == -1)
            {
                throw new ArgumentException("No key column!");
            }

            //处理 where 语句
            if (!string.IsNullOrEmpty(where))
            {
                where = " WHERE " + where.Substring(4);
            }

            //生成SELECT语句
            StringBuilder sqlText = new StringBuilder();

            //获取基本SELECT部分
            GetBaseSelect(sqlText, columns, tableName, where, pageSize);

            //形成排序语句
            if (!string.IsNullOrEmpty(orderBy))
            {
                sqlText.Append(" ORDER BY ");
                sqlText.Append(orderBy);
            }

            Dictionary<int, T> dict = new Dictionary<int, T>();
            string typeName = type.Namespace + "." + type.Name;

            //从数据库中获取数据
            SqlDataReader sdr = SQLHelper.DataReader(connection, sqlText.ToString(), CommandType.Text, parms);
            while (sdr.Read())
            {
                dict.Add(sdr.GetInt32(keyIndex), EntityHelper.GetDataReaderObject<T>(type, typeName, columns, sdr));
            }
            sdr.Close();

            //返回数据
            return dict;
        }
        #endregion

        #region GetModelStringDictionary 获取数据字典对象（Key值限制为String型，且在Model模型中排第一位）
        /// <summary>
        /// 获取数据字典对象（Key值限制为String型，且在Model模型中排第一位）
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="pageSize">记录条数，0表示全部读取</param>
        /// <param name="where">查询条件，必须以 AND 开头，如 AND SortId=1 AND IsShow=1</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=@SortId</param>
        /// <param name="orderBy">排序，如 LogId DESC 或 LogId DESC,IsShow ASC</param>
        /// <param name="customKey">自定义临时主键。如不设置则使用表默认值</param>
        /// <param name="customColumns">自定义查询列名集合，使用逗号分隔。如不需要则为null</param>
        /// <returns></returns>
        Dictionary<string, T> ISQLFactory.GetModelStringDictionary<T>(int pageSize, string where, DbParameter[] parms, string orderBy, string customKey, string customColumns)
        {
            Type type = typeof(T);

            //获取表名
            string tableName = EntityHelper.GetTableName(type);

            //获取所有字段
            List<string> columns = EntityHelper.GetTableColumns(type, ColumnTypes.Extend, customColumns);

            //获取主键
            PropertyInfo keyColumn = (customKey == null) ? EntityHelper.GetTableIdentity(type) : type.GetProperty(customKey);

            //查找主键索引位置
            int keyIndex = columns.IndexOf(keyColumn.Name);
            if (keyIndex == -1)
            {
                throw new ArgumentException("No key column!");
            }

            //处理 where 语句
            if (!string.IsNullOrEmpty(where))
            {
                where = " WHERE " + where.Substring(4);
            }

            //生成SELECT语句
            StringBuilder sqlText = new StringBuilder();

            //获取基本SELECT部分
            GetBaseSelect(sqlText, columns, tableName, where, pageSize);

            //形成排序语句
            if (!string.IsNullOrEmpty(orderBy))
            {
                sqlText.Append(" ORDER BY ");
                sqlText.Append(orderBy);
            }

            Dictionary<string, T> dict = new Dictionary<string, T>();
            string typeName = type.Namespace + "." + type.Name;

            //从数据库中获取数据
            SqlDataReader sdr = SQLHelper.DataReader(connection, sqlText.ToString(), CommandType.Text, parms);
            while (sdr.Read())
            {
                dict.Add(sdr.GetString(keyIndex), EntityHelper.GetDataReaderObject<T>(type, typeName, columns, sdr));
            }
            sdr.Close();

            //返回数据
            return dict;
        }
        #endregion

        #region GetModelList 获取单页列表数据对象
        /// <summary>
        /// 获取单页列表数据对象
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="pageSize">记录条数，0表示全部读取</param>
        /// <param name="where">查询条件，必须以 AND 开头，如 AND SortId=1 AND IsShow=1</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=@SortId</param>
        /// <param name="orderBy">排序，如 LogId DESC 或 LogId DESC,IsShow ASC</param>
        /// <param name="customColumns">自定义查询列名集合，使用逗号分隔。如不需要则为null</param>
        /// <returns></returns>
        List<T> ISQLFactory.GetModelList<T>(int pageSize, string where, DbParameter[] parms, string orderBy, string customColumns)
        {
            Type type = typeof(T);

            //获取表名
            string tableName = EntityHelper.GetTableName(type);

            //获取所有字段
            List<string> columns = EntityHelper.GetTableColumns(type, ColumnTypes.Extend, customColumns);

            //处理 where 语句
            if (!string.IsNullOrEmpty(where))
            {
                where = " WHERE " + where.Substring(4);
            }

            //生成SELECT语句
            StringBuilder sqlText = new StringBuilder();

            //获取基本SELECT部分
            GetBaseSelect(sqlText, columns, tableName, where, pageSize);

            //形成排序语句
            if (!string.IsNullOrEmpty(orderBy))
            {
                sqlText.Append(" ORDER BY ");
                sqlText.Append(orderBy);
            }

            var models = new List<T>();
            string typeName = type.Namespace + "." + type.Name;

            //从数据库中获取数据
            SqlDataReader sdr = SQLHelper.DataReader(connection, sqlText.ToString(), CommandType.Text, parms);
            while (sdr.Read())
            {
                models.Add(EntityHelper.GetDataReaderObject<T>(type, typeName, columns, sdr));
            }
            sdr.Close();

            //返回数据
            return models;
        }
        #endregion

        #region GetDataSet 通过完整的Sql语句获得列表数据对象
        /// <summary>
        /// 通过完整的Sql语句获得列表数据对象
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="commandType">执行语句的类型</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=@SortId</param>
        /// <returns></returns>
        DataSet ISQLFactory.GetDataSet(string sqlText, CommandType commandType, DbParameter[] parms)
        {
            return SQLHelper.DataSet(connection, sqlText, commandType, parms);
        }
        #endregion

        #region GetTableList 通过完整的Sql语句获得列表数据对象
        /// <summary>
        /// 通过完整的Sql语句获得列表数据对象
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="commandType">执行语句的类型</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=@SortId</param>
        /// <returns></returns>
        DataTable ISQLFactory.GetTableList(string sqlText, CommandType commandType, DbParameter[] parms)
        {
            return SQLHelper.DataSet(connection, sqlText, commandType, parms).Tables[0];
        }
        #endregion

        #region GetTableList 获取单页列表数据对象
        /// <summary>
        /// 获取单页列表数据列表
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="pageSize">记录条数，0表示全部读取</param>
        /// <param name="where">查询条件，必须以 AND 开头，如 AND SortId=1 AND IsShow=1</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=@SortId</param>
        /// <param name="orderBy">排序，如 LogId DESC 或 LogId DESC,IsShow ASC</param>
        /// <param name="customColumns">自定义查询列名集合，使用逗号分隔。如不需要则为null</param>
        /// <returns></returns>
        DataTable ISQLFactory.GetTableList<T>(int pageSize, string where, DbParameter[] parms, string orderBy, string customColumns)
        {
            Type type = typeof(T);

            //获取表名
            string tableName = EntityHelper.GetTableName(type);

            //处理空值字段
            if (customColumns == null)
            {
                customColumns = "*";
            }

            //获取所有字段
            List<string> columns = EntityHelper.GetTableColumns(type, ColumnTypes.Extend, customColumns);

            //处理 where 语句
            if (!string.IsNullOrEmpty(where))
            {
                where = " WHERE " + where.Substring(4);
            }

            //生成SELECT语句
            StringBuilder sqlText = new StringBuilder();

            //获取基本SELECT部分
            GetBaseSelect(sqlText, columns, tableName, where, pageSize);

            //形成排序语句
            if (!string.IsNullOrEmpty(orderBy))
            {
                sqlText.Append(" ORDER BY ");
                sqlText.Append(orderBy);
            }

            //返回数据
            return SQLHelper.DataSet(connection, sqlText.ToString(), CommandType.Text, parms).Tables[0];
        }
        #endregion

        #region GetModelPager 获取翻页列表数据对象
        /// <summary>
        /// 获取翻页列表数据对象
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="pageSize">记录条数，0表示全部读取</param>
        /// <param name="currentPage">当前页码</param>
        /// <param name="records">返回当前条件下总部的记录数</param>
        /// <param name="where">查询条件，必须以 AND 开头，如 AND SortId=1 AND IsShow=1</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=@SortId</param>
        /// <param name="orderBy">排序，如 LogId DESC 或 LogId DESC,IsShow ASC</param>
        /// <param name="customColumns">自定义查询列名集合，使用逗号分隔。如不需要则为null</param>
        /// <returns></returns>
        List<T> ISQLFactory.GetModelPager<T>(int pageSize, int currentPage, ref int records, string where, DbParameter[] parms, string orderBy, string customColumns)
        {
            Type type = typeof(T);

            //获取表名
            string tableName = EntityHelper.GetTableName(type);

            //获取需要输出的字段
            var columns = EntityHelper.GetTableColumns(type, ColumnTypes.Extend, customColumns);
            var outputlist = new StringBuilder();

            var loop = false;

            foreach (string val in columns)
            {
                outputlist.Append(",");
                outputlist.Append(val);

                if (!string.IsNullOrEmpty(orderBy) && !loop)
                {
                    if (orderBy.Contains(val))
                    {
                        loop = true;
                    }
                }
            }

            outputlist = outputlist.Remove(0, 1);

            //处理 where 语句
            if (where == null)
            {
                where = "";
            }
            else if (where.Length > 0)
            {
                where = " WHERE " + where.Substring(4);
            }

            /**如果没有指定排序规则，则默认用主键的倒序排序*/

            if (orderBy == null || !loop)
            {
                var identity = LT.Component.Entity.EntityHelper.GetTableIdentity(typeof(T));
                orderBy = identity.Name;
            }

            /**如果没有指定排序规则，则默认用主键的倒序排序*/

            //强制需要排序字段
            //if (orderBy == null || orderBy.Length == 0)
            //{
            //    throw new ArgumentNullException("OrderBy");
            //}

            //储存过程参数
            parms = new SqlParameter[]{
                new SqlParameter("@TableName", SqlDbType.VarChar),
                new SqlParameter("@Columns", SqlDbType.VarChar),
                new SqlParameter("@PageSize", SqlDbType.Int),
                new SqlParameter("@CurrentPage", SqlDbType.Int),
                new SqlParameter("@Wheres", SqlDbType.NVarChar),
                new SqlParameter("@OrderBy", SqlDbType.VarChar)                
            };

            parms[0].Value = tableName;
            parms[1].Value = outputlist.ToString();
            parms[2].Value = pageSize;
            parms[3].Value = currentPage;
            parms[4].Value = where;

            parms[5].Value = orderBy;

            var models = new List<T>();
            string typeName = type.Namespace + "." + type.Name;

            //从数据库中获取数据（返回两个数据集：记录数和列表数据）
            var sdr = SQLHelper.DataReader(connection, "pub_data_pager_list", CommandType.StoredProcedure, parms);

            //读取记录数
            sdr.Read();
            records = sdr.GetInt32(0);

            //读取列表数据
            sdr.NextResult();
            while (sdr.Read())
            {
                models.Add(EntityHelper.GetDataReaderObject<T>(type, typeName, columns, sdr));
            }
            sdr.Close();

            //返回数据
            return models;

        }
        #endregion

        #region GetTablePager 获取翻页列表数据对象
        /// <summary>
        /// 获取翻页列表数据对象
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="pageSize">记录条数，0表示全部读取</param>
        /// <param name="currentPage">当前页码</param>
        /// <param name="records">返回当前条件下总部的记录数</param>
        /// <param name="where">查询条件，必须以 AND 开头，如 AND SortId=1 AND IsShow=1</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=@SortId</param>
        /// <param name="orderBy">排序，如 LogId DESC 或 LogId DESC,IsShow ASC</param>
        /// <param name="columns">输出的列名集合，使用逗号分隔</param>
        /// <param name="tableName">数据表或视图名</param>
        /// <returns></returns>
        DataTable ISQLFactory.GetTablePager(int pageSize, int currentPage, ref int records, string where, DbParameter[] parms, string orderBy, string columns, string tableName)
        {
            //处理 where 语句
            if (!string.IsNullOrEmpty(where))
            {
                where = " WHERE " + where.Substring(4);
            }
            else
            {
                where = "";
            }

            //强制需要排序字段
            if (orderBy == null || orderBy.Length == 0)
            {
                throw new ArgumentNullException("OrderBy");
            }

            //储存过程参数
            parms = new SqlParameter[]{
                new SqlParameter("@TableName", SqlDbType.VarChar),
                new SqlParameter("@Columns", SqlDbType.VarChar),
                new SqlParameter("@PageSize", SqlDbType.Int),
                new SqlParameter("@CurrentPage", SqlDbType.Int),
                new SqlParameter("@Wheres", SqlDbType.NVarChar),
                new SqlParameter("@OrderBy", SqlDbType.VarChar)                
            };
            parms[0].Value = tableName;
            parms[1].Value = columns;
            parms[2].Value = pageSize;
            parms[3].Value = currentPage;
            parms[4].Value = where;
            parms[5].Value = orderBy;

            //从数据库中获取数据（返回两个数据集：记录数和列表数据）
            DataTableCollection dtc = SQLHelper.DataSet(connection, "pub_data_pager_list", CommandType.StoredProcedure, parms).Tables;

            //读取记录数（第一个表）
            if (dtc[0].Rows.Count > 0)
            {
                records = Convert.ToInt32(dtc[0].Rows[0][0]);
            }

            //返回数据（第三个表）
            return dtc[1];
        }
        #endregion

        #region GetTablePager 获取翻页列表数据对象(由对象获取表和字段)
        /// <summary>
        /// 获取翻页列表数据对象(由对象获取表和字段)
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="pageSize">记录条数，0表示全部读取</param>
        /// <param name="currentPage">当前页码</param>
        /// <param name="records">返回当前条件下总部的记录数</param>
        /// <param name="where">查询条件，必须以 AND 开头，如 AND SortId=1 AND IsShow=1</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=@SortId</param>
        /// <param name="orderBy">排序，如 LogId DESC 或 LogId DESC,IsShow ASC</param>
        /// <param name="customColumns">自定义查询列名集合，使用逗号分隔。如不需要则为null</param>
        /// <returns></returns>
        DataTable ISQLFactory.GetTablePager<T>(int pageSize, int currentPage, ref int records, string where, DbParameter[] parms, string orderBy, string customColumns)
        {
            Type type = typeof(T);

            //获取表名
            string tableName = EntityHelper.GetTableName(type);

            //处理空值字段
            if (customColumns == null)
            {
                customColumns = "*";
            }

            //获取需要输出的字段
            var columns = EntityHelper.GetTableColumns(type, ColumnTypes.Extend, customColumns);
            StringBuilder outputlist = new StringBuilder();
            foreach (string val in columns)
            {
                outputlist.Append(",");
                outputlist.Append(val);
            }
            outputlist = outputlist.Remove(0, 1);

            //处理 where 语句
            if (!string.IsNullOrEmpty(where))
            {
                where = " WHERE " + where.Substring(4);
            }
            else
            {
                where = "";
            }

            //强制需要排序字段
            if (orderBy == null || orderBy.Length == 0)
            {
                throw new ArgumentNullException("OrderBy");
            }

            //储存过程参数
            parms = new SqlParameter[]{
                new SqlParameter("@TableName", SqlDbType.VarChar),
                new SqlParameter("@Columns", SqlDbType.VarChar),
                new SqlParameter("@PageSize", SqlDbType.Int),
                new SqlParameter("@CurrentPage", SqlDbType.Int),
                new SqlParameter("@Wheres", SqlDbType.NVarChar),
                new SqlParameter("@OrderBy", SqlDbType.VarChar)                
            };
            parms[0].Value = tableName;
            parms[1].Value = outputlist.ToString();
            parms[2].Value = pageSize;
            parms[3].Value = currentPage;
            parms[4].Value = where;
            parms[5].Value = orderBy;

            //从数据库中获取数据（返回两个数据集：记录数和列表数据）
            DataTableCollection dtc = SQLHelper.DataSet(connection, "pub_data_pager_list", CommandType.StoredProcedure, parms).Tables;

            //读取记录数（第一个表）
            if (dtc[0].Rows.Count > 0)
            {
                records = Convert.ToInt32(dtc[0].Rows[0][0]);
            }
            else
            {
                return null;
            }

            //返回数据（第三个表）
            return dtc[1];
        }
        #endregion

        #region GetCount 获取满足条件的数量
        /// <summary>
        /// 获取满足条件的数量
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="where">查询条件，必须以 AND 开头，如 AND SortId=1 AND IsShow=1</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=@SortId</param>
        /// <returns></returns>
        int ISQLFactory.GetCount<T>(string where, DbParameter[] parms)
        {
            Type type = typeof(T);

            //获取表名
            string tableName = EntityHelper.GetTableName(type);

            //生成UPDATE语句
            StringBuilder sqlText = new StringBuilder();
            sqlText.Append("SELECT COUNT(*) FROM ");
            sqlText.Append(tableName);

            //处理 WHERE 语句
            if (!string.IsNullOrEmpty(where))
            {
                sqlText.Append(" WHERE ");
                sqlText.Append(where.Substring(4));
            }

            int count = 0;

            //执行语句
            SqlDataReader sdr = SQLHelper.DataReader(connection, sqlText.ToString(), CommandType.Text, parms);
            if (sdr.Read())
            {
                count = sdr.GetInt32(0);
            }
            sdr.Close();

            //返回结果
            return count;
        }
        #endregion

        #region GetIntDistinct 获取数据表整型的Distinct数据集合
        /// <summary>
        /// 获取数据表整型的Distinct数据集合
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="column">输出的字段，只能一个</param>
        /// <param name="where">查询条件，必须以 AND 开头，如 AND SortId=1 AND IsShow=1</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=@SortId</param>
        /// <returns></returns>
        List<int> ISQLFactory.GetIntDistinct<T>(string column, string where, DbParameter[] parms)
        {
            Type type = typeof(T);

            //获取表名
            string tableName = EntityHelper.GetTableName(type);

            //生成UPDATE语句
            StringBuilder sqlText = new StringBuilder();
            sqlText.Append("SELECT DISTINCT(");
            sqlText.Append(column);
            sqlText.Append(") FROM ");
            sqlText.Append(tableName);

            //处理 WHERE 语句
            if (!string.IsNullOrEmpty(where))
            {
                sqlText.Append(" WHERE ");
                sqlText.Append(where.Substring(4));
            }

            var list = new List<int>();

            //执行语句
            var sdr = SQLHelper.DataReader(connection, sqlText.ToString(), CommandType.Text, parms);
            while (sdr.Read())
            {
                if (sdr.IsDBNull(0))
                {
                    continue;
                }
                list.Add(sdr.GetInt32(0));
            }
            sdr.Close();

            //返回结果
            return list;
        }
        #endregion

        #region GetStringDistinct 获取数据表字符型的Distinct数据集合
        /// <summary>
        /// 获取数据表字符型的Distinct数据集合
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="column">输出的字段，只能一个</param>
        /// <param name="where">查询条件，必须以 AND 开头，如 AND SortId=1 AND IsShow=1</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=@SortId</param>
        /// <returns></returns>
        List<string> ISQLFactory.GetStringDistinct<T>(string column, string where, DbParameter[] parms)
        {
            Type type = typeof(T);

            //获取表名
            string tableName = EntityHelper.GetTableName(type);

            //生成UPDATE语句
            StringBuilder sqlText = new StringBuilder();
            sqlText.Append("SELECT DISTINCT(");
            sqlText.Append(column);
            sqlText.Append(") FROM ");
            sqlText.Append(tableName);

            //处理 WHERE 语句
            if (!string.IsNullOrEmpty(where))
            {
                sqlText.Append(" WHERE ");
                sqlText.Append(where.Substring(4));
            }

            var list = new List<string>();

            //执行语句
            var sdr = SQLHelper.DataReader(connection, sqlText.ToString(), CommandType.Text, parms);
            while (sdr.Read())
            {
                if (sdr.IsDBNull(0))
                {
                    continue;
                }
                list.Add(sdr.GetString(0));
            }
            sdr.Close();

            //返回结果
            return list;
        }
        #endregion

        /// <summary>
        ///  根据sql语句，查询数据列表
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        DataTable ISQLFactory.ExecuteSQL(string queryString, DbParameter[] parms)
        {
            var sda = new SqlDataAdapter();

            var dataset = new DataTable();

            //返回数据
            return SQLHelper.DataSet(connection, queryString.ToString(), CommandType.Text, parms).Tables[0];
        }

        #region Insert 把对象内容保存到数据库中
        /// <summary>
        /// 把对象内容保存到数据库中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">更新对象模型</param>
        /// <param name="returnType">
        ///     返回的数据类型：
        ///     EffectRow： 为返回受影响行数；
        ///     Identity： 返回最新插入主键值
        ///     OrderId：更新OrderId值，并返回最后插入的主键值
        ///     None：不需要返回值
        /// </param>
        /// <returns></returns>
        object ISQLFactory.Insert<T>(T model, ReturnTypes returnType)
        {
            int i = 0;
            object obj = null;
            Type type = typeof(T);

            //获取表名
            string tableName = EntityHelper.GetTableName(type);

            //获取所有字段和主键名称
            List<string> columns = EntityHelper.GetTableColumns(type, ColumnTypes.Increment | ColumnTypes.Extend | ColumnTypes.Read | ColumnTypes.ReadUpdate, null);

            //生成INSERT语句
            StringBuilder sqlText = new StringBuilder();
            sqlText.Append("INSERT INTO ");
            sqlText.Append(tableName);
            sqlText.Append(" (");

            //第一个字段
            sqlText.Append(columns[0]);

            //第二个起所有字段
            int loop = columns.Count;
            for (i = 1; i < loop; i++)
            {
                sqlText.Append(",");
                sqlText.Append(columns[i]);
            }

            sqlText.Append(") VALUES (");

            //第一个字段
            sqlText.Append("@");
            sqlText.Append(columns[0]);

            //第二个起所有字段
            for (i = 1; i < loop; i++)
            {
                sqlText.Append(",@");
                sqlText.Append(columns[i]);
            }

            sqlText.Append(");");

            //生成SqlParamter
            bool quote = false;
            PropertyInfo propertyInfo = null;

            SqlParameter[] paras = new SqlParameter[loop];
            for (i = 0; i < loop; i++)
            {
                propertyInfo = type.GetProperty(columns[i]);
                paras[i] = new SqlParameter("@" + columns[i], GetDbType(propertyInfo.PropertyType, ref quote));
                paras[i].Value = propertyInfo.GetValue(model, null);
            }

            //根据两种情况返回不同的值
            if (returnType == ReturnTypes.Identity || returnType == ReturnTypes.OrderIdForIntAuto || returnType == ReturnTypes.OrderIdForString)
            {
                //如果是OrderId类型，则再更新当前的OrderId值
                if (returnType == ReturnTypes.OrderIdForIntAuto)
                {
                    //插入数据并获最后插入的ID号
                    sqlText.Append(" SELECT @@IDENTITY;");
                    SqlDataReader sdr = SQLHelper.DataReader(connection, sqlText.ToString(), CommandType.Text, paras);
                    if (sdr.Read())
                    {
                        obj = sdr.GetValue(0);
                    }
                    sdr.Close();

                    //获取主键
                    PropertyInfo keyColumn = EntityHelper.GetTableIdentity(type);

                    //生成更新语句
                    sqlText.Length = 0;
                    sqlText.Append("UPDATE ");
                    sqlText.Append(tableName);
                    sqlText.Append(" SET OrderId=");
                    sqlText.Append(obj);
                    sqlText.Append(" WHERE ");
                    sqlText.Append(keyColumn.Name);
                    sqlText.Append("=");
                    sqlText.Append(obj);
                    sqlText.Append(";");

                    //执行语句
                    SQLHelper.NonQuery(connection, sqlText.ToString(), CommandType.Text, null);
                }
                else if (returnType == ReturnTypes.OrderIdForString)
                {
                    //累加获取当前记录数
                    sqlText.Append("SELECT MAX(OrderId) FROM ");
                    sqlText.Append(tableName);
                    sqlText.Append(";");

                    //获取目前最大的排序ID
                    int maxOrderId = 0;

                    //执行语句
                    SqlDataReader sdr = SQLHelper.DataReader(connection, sqlText.ToString(), CommandType.Text, paras);
                    if (sdr.Read())
                    {
                        maxOrderId = sdr.GetInt32(0);
                    }
                    sdr.Close();

                    //生成下一个排序ID
                    int nextOrderId = maxOrderId + 1;

                    //获取主键
                    PropertyInfo keyColumn = EntityHelper.GetTableIdentity(type);
                    SqlDbType keyType = GetDbType(keyColumn.PropertyType, ref quote);

                    //获取主键值
                    obj = keyColumn.GetValue(model, null);

                    //更新当前表的OrderId
                    sqlText.Length = 0;
                    sqlText.Append("UPDATE ");
                    sqlText.Append(tableName);
                    sqlText.Append(" SET OrderId=");
                    sqlText.Append(nextOrderId);
                    sqlText.Append(" WHERE ");
                    sqlText.Append(keyColumn.Name);
                    sqlText.Append("='");
                    sqlText.Append(obj);
                    sqlText.Append("'");

                    //执行语句
                    SQLHelper.NonQuery(connection, sqlText.ToString(), CommandType.Text, null);
                }
                else
                {
                    //插入数据并获最后插入的ID号
                    sqlText.Append(" SELECT @@IDENTITY;");
                    SqlDataReader sdr = SQLHelper.DataReader(connection, sqlText.ToString(), CommandType.Text, paras);
                    if (sdr.Read())
                    {
                        obj = sdr.GetValue(0);
                    }
                    sdr.Close();
                }
            }
            else
            {
                obj = SQLHelper.NonQuery(connection, sqlText.ToString(), CommandType.Text, paras);
            }

            //返回值
            return obj;
        }
        #endregion

        #region Insert 把SQL内容保存到数据库中
        /// <summary>
        /// 把SQL内容保存到数据库中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">INSERT 表后的一段SQL，如 (字段1,字段2) VALUES (1, 2)</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=?SortId</param>
        /// <param name="returnType">
        ///     返回的数据类型：
        ///     EffectRow： 为返回受影响行数；
        ///     Identity： 返回最新插入主键值
        ///     OrderId：更新OrderId值，并返回最后插入的主键值
        ///     None：不需要返回值
        /// </param>
        /// <returns></returns>
        int ISQLFactory.Insert<T>(string sql, DbParameter[] parms, ReturnTypes returnType)
        {
            int rel = 0;
            Type type = typeof(T);

            //获取表名
            string tableName = EntityHelper.GetTableName(type);

            //生成INSERT语句
            StringBuilder sqlText = new StringBuilder();
            sqlText.Append("INSERT INTO ");
            sqlText.Append(tableName);
            sqlText.Append(sql);
            sqlText.Append(";");

            //根据两种情况返回不同的值
            if (returnType == ReturnTypes.Identity || returnType == ReturnTypes.OrderIdForIntAuto)
            {
                //累加获取当前加入
                sqlText.Append("SELECT @@IDENTITY;");

                //获最后插入的ID号
                SqlDataReader sdr = SQLHelper.DataReader(connection, sqlText.ToString(), CommandType.Text, parms);
                if (sdr.Read())
                {
                    rel = sdr.GetInt32(0);
                }
                sdr.Close();

                //如果是OrderId类型，则再更新当前的OrderId值
                if (returnType == ReturnTypes.OrderIdForIntAuto)
                {
                    //获取主键
                    PropertyInfo keyColumn = EntityHelper.GetTableIdentity(type);

                    //生成更新语句
                    sqlText.Length = 0;
                    sqlText.Append("UPDATE ");
                    sqlText.Append(tableName);
                    sqlText.Append(" SET OrderId=");
                    sqlText.Append(rel);
                    sqlText.Append(" WHERE ");
                    sqlText.Append(keyColumn.Name);
                    sqlText.Append("=");
                    sqlText.Append(rel);
                    sqlText.Append(";");

                    //执行语句
                    SQLHelper.NonQuery(connection, sqlText.ToString(), CommandType.Text, null);
                }
            }
            else
            {
                rel = SQLHelper.NonQuery(connection, sqlText.ToString(), CommandType.Text, parms);
            }

            //返回值
            return rel;
        }
        #endregion

        #region Update 根据主键值更新对象数据
        /// <summary>
        /// 根据主键值更新对象数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">更新对象模型</param>
        /// <param name="customColumns">自定义查询列名集合，使用逗号分隔。如不需要则为null</param>
        void ISQLFactory.Update<T>(T model, string customColumns)
        {
            int i = 0;
            Type type = typeof(T);

            //获取表名
            string tableName = EntityHelper.GetTableName(type);

            //获取所有字段
            List<string> columns = EntityHelper.GetTableColumns(type, ColumnTypes.Identity | ColumnTypes.Read | ColumnTypes.ReadInsert | ColumnTypes.Extend, customColumns);

            //获取主键
            bool quote = false;
            PropertyInfo keyColumn = EntityHelper.GetTableIdentity(type);
            GetDbType(keyColumn.PropertyType, ref quote);

            //生成UPDATE语句
            StringBuilder sqlText = new StringBuilder();
            sqlText.Append("UPDATE ");
            sqlText.Append(tableName);
            sqlText.Append(" SET ");

            //第一个字段
            sqlText.Append(columns[0]);
            sqlText.Append("=@");
            sqlText.Append(columns[0]);

            //第二个起所有字段
            int loop = columns.Count;
            for (i = 1; i < loop; i++)
            {
                sqlText.Append(",");
                sqlText.Append(columns[i]);
                sqlText.Append("=@");
                sqlText.Append(columns[i]);
            }

            //WHERE条件
            sqlText.Append(" WHERE ");
            sqlText.Append(keyColumn.Name);
            sqlText.Append("=");

            //处理是否需要'连接
            if (quote)
            {
                sqlText.Append("'");
                sqlText.Append(keyColumn.GetValue(model, null));
                sqlText.Append("'");
            }
            else
            {
                sqlText.Append(keyColumn.GetValue(model, null));
            }

            //生成SqlParamter
            PropertyInfo propertyInfo = null;
            SqlParameter[] paras = new SqlParameter[loop];
            for (i = 0; i < loop; i++)
            {
                propertyInfo = type.GetProperty(columns[i]);
                paras[i] = new SqlParameter("@" + columns[i], GetDbType(propertyInfo.PropertyType, ref quote));
                paras[i].Value = propertyInfo.GetValue(model, null);
            }

            //执行语句
            SQLHelper.NonQuery(connection, sqlText.ToString(), CommandType.Text, paras);
        }
        #endregion

        #region Update 根据条件更新符合条件的数据
        /// <summary>
        /// 根据条件更新符合条件的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">更新对象模型</param>
        /// <param name="where">更新的where条件，如果没有，则使用null（where条件使用 AND 开头）</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=@SortId</param>
        /// <param name="customColumns">自定义查询列名集合，使用逗号分隔。如不需要则为null</param>
        void ISQLFactory.Update<T>(T model, string where, DbParameter[] parms, string customColumns)
        {
            int i = 0;
            Type type = typeof(T);

            //获取表名
            string tableName = EntityHelper.GetTableName(type);

            //获取所有字段
            List<string> columns = EntityHelper.GetTableColumns(type, ColumnTypes.Identity | ColumnTypes.Read | ColumnTypes.ReadInsert | ColumnTypes.Extend, customColumns);

            //生成UPDATE语句
            StringBuilder sqlText = new StringBuilder();
            sqlText.Append("UPDATE ");
            sqlText.Append(tableName);
            sqlText.Append(" SET ");

            //第一个字段
            sqlText.Append(columns[0]);
            sqlText.Append("=@");
            sqlText.Append(columns[0]);

            //第二个起所有字段
            int loop = columns.Count;
            for (i = 1; i < loop; i++)
            {
                sqlText.Append(",");
                sqlText.Append(columns[i]);
                sqlText.Append("=@");
                sqlText.Append(columns[i]);
            }

            //处理 where 语句
            if (where != null && where.Length > 0)
            {
                sqlText.Append(" WHERE ");
                sqlText.Append(where.Substring(4));
            }

            //合并参数（扩容loop）
            int size = loop;
            int maxto = 0;
            if (parms != null && parms.Length > 0)
            {
                maxto = parms.Length;
                size += maxto;
            }

            //生成SqlParamter
            bool quote = false;
            PropertyInfo propertyInfo = null;
            SqlParameter[] paras = new SqlParameter[size];
            for (i = 0; i < loop; i++)
            {
                propertyInfo = type.GetProperty(columns[i]);
                paras[i] = new SqlParameter("@" + columns[i], GetDbType(propertyInfo.PropertyType, ref quote));
                paras[i].Value = propertyInfo.GetValue(model, null);
            }

            //合并参数（扩容内容）
            for (i = 0; i < maxto; i++)
            {
                paras[i + loop] = (SqlParameter)parms[i];
            }

            //执行语句
            SQLHelper.NonQuery(connection, sqlText.ToString(), CommandType.Text, paras);
        }
        #endregion

        #region Update 根据条件更新符合条件的SQL
        /// <summary>
        /// 根据条件更新符合条件的SQL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">待执行更新语句的后半部分（从SET开始并含有一个空格）</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=@SortId</param>
        void ISQLFactory.Update<T>(string sql, DbParameter[] parms)
        {
            Type type = typeof(T);

            //获取表名
            string tableName = EntityHelper.GetTableName(type);

            //生成UPDATE语句
            StringBuilder sqlText = new StringBuilder();
            sqlText.Append("UPDATE ");
            sqlText.Append(tableName);
            sqlText.Append(sql);

            //执行语句
            SQLHelper.NonQuery(connection, sqlText.ToString(), CommandType.Text, parms);
        }
        #endregion

        #region Update 更新多个字段的内容
        /// <summary>
        /// 更新多个字段的内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns">目标字段的数组</param>
        /// <param name="contents">需要更新内容的数组</param>
        void ISQLFactory.Update<T>(string[] columns, object[] contents)
        {
            Type type = typeof(T);

            //获取表名
            string tableName = EntityHelper.GetTableName(type);

            //主键字段
            bool quote = false;
            PropertyInfo keyColumn = EntityHelper.GetTableIdentity(type);
            GetDbType(keyColumn.PropertyType, ref quote);

            //生成UPDATE语句
            StringBuilder sqlText = new StringBuilder();
            sqlText.Append("UPDATE ");
            sqlText.Append(tableName);
            sqlText.Append(" SET ");

            //处理 SET 语句
            bool quote1 = false;
            int count = columns.Length;
            SqlParameter[] parms = new SqlParameter[count];

            string column;
            PropertyInfo valColumn;
            SqlDbType valType;

            //分拆各个字段
            for (int i = 0; i < count; i++)
            {
                column = columns[i];

                //目标字段
                valColumn = type.GetProperty(column);
                valType = GetDbType(valColumn.PropertyType, ref quote1);

                //处理参数
                sqlText.Append(column);
                sqlText.Append("=@");
                sqlText.Append(column);
                sqlText.Append(",");

                parms[i] = new SqlParameter("@" + column, valType);
                parms[i].Value = contents[i];
            }

            //去掉最后,
            sqlText = sqlText.Remove(sqlText.Length - 1, 1);

            //执行语句
            SQLHelper.NonQuery(connection, sqlText.ToString(), CommandType.Text, parms);
        }
        #endregion

        #region Update 根据主键更新指定字段的内容
        /// <summary>
        /// 根据主键更新指定字段的内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyValue">主键值</param>
        /// <param name="column">目标字段</param>
        /// <param name="content">需要更新的内容</param>
        void ISQLFactory.Update<T>(object keyValue, string column, object content)
        {
            Type type = typeof(T);

            //获取表名
            string tableName = EntityHelper.GetTableName(type);

            //主键字段
            bool quote = false;
            PropertyInfo keyColumn = EntityHelper.GetTableIdentity(type);
            GetDbType(keyColumn.PropertyType, ref quote);

            //生成UPDATE语句
            StringBuilder sqlText = new StringBuilder();
            sqlText.Append("UPDATE ");
            sqlText.Append(tableName);
            sqlText.Append(" SET ");
            sqlText.Append(column);
            sqlText.Append("=@");
            sqlText.Append(column);

            //处理 WHERE 语句
            sqlText.Append(" WHERE ");
            sqlText.Append(keyColumn.Name);
            sqlText.Append("=");

            //处理是否需要'连接
            if (quote)
            {
                sqlText.Append("'");
                sqlText.Append(keyValue);
                sqlText.Append("'");
            }
            else
            {
                sqlText.Append(keyValue);
            }

            //目标字段
            PropertyInfo valColumn = type.GetProperty(column);
            SqlDbType valType = GetDbType(valColumn.PropertyType, ref quote);

            //更新参数
            SqlParameter[] parms = new SqlParameter[]{
                new SqlParameter("@"+ column, valType)
            };
            parms[0].Value = content;

            //执行语句
            SQLHelper.NonQuery(connection, sqlText.ToString(), CommandType.Text, parms);
        }
        #endregion

        #region Update 根据主键更新多个字段的内容
        /// <summary>
        /// 根据主键更新多个字段的内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyValue">主键值</param>
        /// <param name="columns">目标字段的数组</param>
        /// <param name="contents">需要更新内容的数组</param>
        void ISQLFactory.Update<T>(object keyValue, string[] columns, object[] contents)
        {
            Type type = typeof(T);

            //获取表名
            string tableName = EntityHelper.GetTableName(type);

            //主键字段
            bool quote = false;
            PropertyInfo keyColumn = EntityHelper.GetTableIdentity(type);
            GetDbType(keyColumn.PropertyType, ref quote);

            //生成UPDATE语句
            StringBuilder sqlText = new StringBuilder();
            sqlText.Append("UPDATE ");
            sqlText.Append(tableName);
            sqlText.Append(" SET ");

            //处理 SET 语句
            bool quote1 = false;
            int count = columns.Length;
            SqlParameter[] parms = new SqlParameter[count];

            string column;
            PropertyInfo valColumn;
            SqlDbType valType;

            //分拆各个字段
            for (int i = 0; i < count; i++)
            {
                column = columns[i];

                //目标字段
                valColumn = type.GetProperty(column);
                valType = GetDbType(valColumn.PropertyType, ref quote1);

                //处理参数
                sqlText.Append(column);
                sqlText.Append("=@");
                sqlText.Append(column);
                sqlText.Append(",");

                parms[i] = new SqlParameter("@" + column, valType);
                parms[i].Value = contents[i];
            }

            //去掉最后,
            sqlText = sqlText.Remove(sqlText.Length - 1, 1);

            //处理 WHERE 语句
            sqlText.Append(" WHERE ");
            sqlText.Append(keyColumn.Name);
            sqlText.Append("=");

            //处理是否需要'连接
            if (quote)
            {
                sqlText.Append("'");
                sqlText.Append(keyValue);
                sqlText.Append("'");
            }
            else
            {
                sqlText.Append(keyValue);
            }

            //执行语句
            SQLHelper.NonQuery(connection, sqlText.ToString(), CommandType.Text, parms);
        }
        #endregion

        #region UpdateOrderId 根据主键更新排序ID
        /// <summary>
        /// 根据主键更新排序ID
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="keyValue">数据表主键的值</param>
        void ISQLFactory.UpdateOrderId<T>(object keyValue)
        {
            Type type = typeof(T);

            //获取表名
            string tableName = EntityHelper.GetTableName(type);

            //获取主键
            bool quote = false;
            PropertyInfo keyColumn = EntityHelper.GetTableIdentity(type);
            GetDbType(keyColumn.PropertyType, ref quote);

            //获取目前最大的排序ID
            int maxOrderId = 0;
            string sqlText1 = "SELECT MAX(OrderId) FROM " + tableName + ";";
            SqlDataReader sdr = SQLHelper.DataReader(connection, sqlText1, CommandType.Text, null);
            if (sdr.Read())
            {
                maxOrderId = sdr.GetInt32(0);
            }
            sdr.Close();

            //生成下一个排序ID
            int nextOrderId = maxOrderId + 1;

            //更新当前表的OrderId
            StringBuilder sqlText2 = new StringBuilder();
            sqlText2.Append("UPDATE ");
            sqlText2.Append(tableName);
            sqlText2.Append(" SET OrderId=");
            sqlText2.Append(nextOrderId);
            sqlText2.Append(" WHERE ");
            sqlText2.Append(keyColumn.Name);
            sqlText2.Append("=");

            //处理是否需要'连接
            if (quote)
            {
                sqlText2.Append("'");
                sqlText2.Append(keyValue);
                sqlText2.Append("'");
            }
            else
            {
                sqlText2.Append(keyValue);
            }

            SQLHelper.NonQuery(connection, sqlText2.ToString(), CommandType.Text, null);
        }
        #endregion

        #region DeleteByKey 根据主键删除记录
        /// <summary>
        /// 根据主键删除记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyValue">数据表主键的值</param>
        /// <param name="customKey">自定义临时主键。如不设置则使用表默认值</param>
        /// <returns></returns>
        int ISQLFactory.DeleteByKey<T>(object keyValue, string customKey)
        {
            Type type = typeof(T);

            //获取表名
            string tableName = EntityHelper.GetTableName(type);

            //获取主键
            bool quote = false;
            PropertyInfo keyColumn = (customKey == null) ? EntityHelper.GetTableIdentity(type) : type.GetProperty(customKey);
            SqlDbType keyType = GetDbType(keyColumn.PropertyType, ref quote);

            //生成DELETE语句
            StringBuilder sqlText = new StringBuilder();
            sqlText.Append("DELETE FROM ");
            sqlText.Append(tableName);
            sqlText.Append(" WHERE ");
            sqlText.Append(keyColumn.Name);
            sqlText.Append("=");

            //处理是否需要'连接
            if (quote)
            {
                sqlText.Append("'");
                sqlText.Append(keyValue);
                sqlText.Append("'");
            }
            else
            {
                sqlText.Append(keyValue);
            }

            //执行语句并返回影响行数
            return SQLHelper.NonQuery(connection, sqlText.ToString(), CommandType.Text, null);
        }
        #endregion

        #region DeleteByWhere 根据条件删除符合条件的记录
        /// <summary>
        /// 根据条件删除符合条件的记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where">删除的where条件，如果没有，则使用null（where条件使用 AND 开头）</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=@SortId</param>
        /// <returns></returns>
        int ISQLFactory.DeleteByWhere<T>(string where, DbParameter[] parms)
        {
            Type type = typeof(T);

            //获取表名
            string tableName = EntityHelper.GetTableName(type);

            //生成DELETE语句
            StringBuilder sqlText = new StringBuilder();
            sqlText.Append("DELETE FROM ");
            sqlText.Append(tableName);

            //处理 where 语句
            if (!string.IsNullOrEmpty(where))
            {
                sqlText.Append(" WHERE ");
                sqlText.Append(where.Substring(4));
            }

            //执行语句并返回影响行数
            return SQLHelper.NonQuery(connection, sqlText.ToString(), CommandType.Text, parms);
        }
        #endregion

        #region NonQuery 根据SQL语句执行
        /// <summary>
        /// 根据SQL语句执行
        /// </summary>
        /// <param name="sqlText">全SQL执行语句</param>
        /// <param name="commandType">执行语句的类型</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=@SortId</param>
        /// <returns></returns>
        int ISQLFactory.NonQuery(string sqlText, CommandType commandType, DbParameter[] parms)
        {
            return SQLHelper.NonQuery(connection, sqlText, commandType, parms);
        }
        #endregion

        #region DataReader 根据SQL语句执行返回DataReader（自定义语句）
        /// <summary>
        /// 根据SQL语句执行返回DataReader（自定义语句）
        /// </summary>
        /// <typeparam name="D">DataReader类型</typeparam>
        /// <param name="sqlText">待执行的SQL语句</param>
        /// <param name="commandType">执行语句的类型</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=@SortId</param>
        /// <returns></returns>
        D ISQLFactory.DataReader<D>(string sqlText, CommandType commandType, DbParameter[] parms)
        {
            return SQLHelper.DataReader(connection, sqlText, commandType, parms) as D;
        }
        #endregion

        #region DataReader 根据SQL语句执行返回DataReader（自定义字段）
        /// <summary>
        /// 根据SQL语句执行返回DataReader（自定义字段）
        /// </summary>
        /// <typeparam name="D">DataReader类型</typeparam>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="pageSize">记录条数，0表示全部读取</param>
        /// <param name="where">读取的where条件，如果没有，则使用null（where条件使用 AND 开头）</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=@SortId</param>
        /// <param name="orderBy">排序，如 LogId DESC 或 LogId DESC,IsShow ASC</param>
        /// <param name="customColumns">自定义查询列名集合，使用逗号分隔（不能为空）</param>
        /// <returns></returns>
        D ISQLFactory.DataReader<D, T>(int pageSize, string where, DbParameter[] parms, string orderBy, string customColumns)
        {
            Type type = typeof(T);

            //获取表名
            string tableName = EntityHelper.GetTableName(type);

            //获取所有字段和主键名称
            List<string> columns = EntityHelper.GetTableColumns(type, ColumnTypes.Extend, customColumns);

            //处理 WHERE 语句
            if (!string.IsNullOrEmpty(where))
            {
                where = " WHERE " + where.Substring(4);
            }

            //生成SELECT语句
            StringBuilder sqlText = new StringBuilder();

            //获取基本SELECT部分
            GetBaseSelect(sqlText, columns, tableName, where, pageSize);

            //形成排序语句
            if (!string.IsNullOrEmpty(orderBy))
            {
                sqlText.Append(" ORDER BY ");
                sqlText.Append(orderBy);
            }

            //返回数据
            return SQLHelper.DataReader(connection, sqlText.ToString(), CommandType.Text, parms) as D;
        }
        #endregion

        #region IsExist 根据主键值判断是否存在记录
        /// <summary>
        /// 根据主键值判断是否存在记录
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="keyValue">数据表主键的值</param>
        /// <returns></returns>
        bool ISQLFactory.IsExist<T>(object keyValue)
        {
            Type type = typeof(T);

            //获取表名
            string tableName = EntityHelper.GetTableName(type);

            //主键字段
            bool quote = false;
            PropertyInfo keyColumn = EntityHelper.GetTableIdentity(type);
            GetDbType(keyColumn.PropertyType, ref quote);

            //生成Count语句
            StringBuilder sqlText = new StringBuilder();
            sqlText.Append("SELECT COUNT(0) FROM ");
            sqlText.Append(tableName);

            //处理 WHERE 语句
            sqlText.Append(" WHERE ");
            sqlText.Append(keyColumn.Name);
            sqlText.Append("=");

            //处理是否需要'连接
            if (quote)
            {
                sqlText.Append("'");
                sqlText.Append(keyValue);
                sqlText.Append("'");
            }
            else
            {
                sqlText.Append(keyValue);
            }

            //从数据库中获取数据
            int count = 0;
            SqlDataReader sdr = SQLHelper.DataReader(connection, sqlText.ToString(), CommandType.Text, null);
            if (sdr.Read())
            {
                count = sdr.GetInt32(0);
            }
            sdr.Close();

            //返回数据
            return (count > 0);
        }
        #endregion

        #region IsExist 根据主键值判断是否存在记录
        /// <summary>
        /// 根据主键值判断是否存在记录
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="keyValue">数据表主键的值</param>
        /// <returns></returns>
        bool ISQLFactory.IsExist<T>(object keyValue, string customKey)
        {
            Type type = typeof(T);

            //获取表名
            string tableName = EntityHelper.GetTableName(type);

            //主键字段
            bool quote = false;
            PropertyInfo keyColumn = (customKey == null) ? EntityHelper.GetTableIdentity(type) : type.GetProperty(customKey);
            GetDbType(keyColumn.PropertyType, ref quote);

            //生成Count语句
            StringBuilder sqlText = new StringBuilder();
            sqlText.Append("SELECT COUNT(0) FROM ");
            sqlText.Append(tableName);

            //处理 WHERE 语句
            sqlText.Append(" WHERE ");
            sqlText.Append(keyColumn.Name);
            sqlText.Append("=");

            //处理是否需要'连接
            if (quote)
            {
                sqlText.Append("'");
                sqlText.Append(keyValue);
                sqlText.Append("'");
            }
            else
            {
                sqlText.Append(keyValue);
            }

            //从数据库中获取数据
            int count = 0;
            SqlDataReader sdr = SQLHelper.DataReader(connection, sqlText.ToString(), CommandType.Text, null);
            if (sdr.Read())
            {
                count = sdr.GetInt32(0);
            }
            sdr.Close();

            //返回数据
            return (count > 0);
        }
        #endregion

        #region IsExist 根据条件判断是否符合要求的记录
        /// <summary>
        /// 根据条件判断是否符合要求的记录
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="where">删除的where条件，如果没有，则使用null（where条件使用 AND 开头）</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=@SortId</param>
        /// <returns></returns>
        bool ISQLFactory.IsExist<T>(string where, DbParameter[] parms)
        {
            Type type = typeof(T);

            //获取表名
            string tableName = EntityHelper.GetTableName(type);

            //生成Count语句
            StringBuilder sqlText = new StringBuilder();
            sqlText.Append("SELECT COUNT(*) FROM ");
            sqlText.Append(tableName);

            //处理 where 语句
            if (!string.IsNullOrEmpty(where))
            {
                sqlText.Append(" WHERE ");
                sqlText.Append(where.Substring(4));
            }

            //从数据库中获取数据
            int count = 0;
            SqlDataReader sdr = SQLHelper.DataReader(connection, sqlText.ToString(), CommandType.Text, parms);
            if (sdr.Read())
            {
                count = sdr.GetInt32(0);
            }
            sdr.Close();

            //返回数据
            return (count > 0);
        }
        #endregion

        #region OrderAct 根据主键值把表的OrderId值进行更换
        /// <summary>
        /// 根据主键值把表的OrderId值进行更换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="currentId">当前数据ID</param>
        /// <param name="exchangeId">待更换数据ID</param>
        void ISQLFactory.OrderAct<T>(object currentId, object exchangeId)
        {
            Type type = typeof(T);

            //获取表名
            string tableName = EntityHelper.GetTableName(type);

            //获取主键
            bool quote = false;
            PropertyInfo keyColumn = EntityHelper.GetTableIdentity(type);
            SqlDbType keyType = GetDbType(keyColumn.PropertyType, ref quote);
            string keyName = keyColumn.Name;

            //生成Count语句
            StringBuilder sqlText = new StringBuilder();
            sqlText.Append("SELECT OrderId FROM ");
            sqlText.Append(tableName);
            sqlText.Append(" WHERE ");
            sqlText.Append(keyName);
            sqlText.Append("=@");
            sqlText.Append(keyName);
            sqlText.Append(";");

            //获取当前OrderId值
            int currentOrderId = -99999;

            SqlParameter[] parms1 = new SqlParameter[]{
                new SqlParameter("@"+ keyName, keyType)
            };
            parms1[0].Value = currentId;

            SqlDataReader sdr1 = SQLHelper.DataReader(connection, sqlText.ToString(), CommandType.Text, parms1);
            if (sdr1.Read())
            {
                currentOrderId = sdr1.GetInt32(0);
            }
            sdr1.Close();

            //获取等交换OrderId值
            int exchangeOrderId = -99999;

            SqlParameter[] parms2 = new SqlParameter[]{
                new SqlParameter("@"+ keyName, keyType)
            };
            parms2[0].Value = exchangeId;

            SqlDataReader sdr2 = SQLHelper.DataReader(connection, sqlText.ToString(), CommandType.Text, parms2);
            if (sdr2.Read())
            {
                exchangeOrderId = sdr2.GetInt32(0);
            }
            sdr2.Close();

            //进行交换更新数据
            if (currentOrderId != -99999 && exchangeOrderId != -99999)
            {
                sqlText.Remove(0, sqlText.Length);
                sqlText.Append("UPDATE ");
                sqlText.Append(tableName);
                sqlText.Append(" SET OrderId=@OrderId");
                sqlText.Append(" WHERE ");
                sqlText.Append(keyName);
                sqlText.Append("=@");
                sqlText.Append(keyName);
                sqlText.Append(";");

                //更新当前的
                parms1 = new SqlParameter[]{
                    new SqlParameter("@OrderId", DbType.Int32),
                    new SqlParameter("@"+ keyName, keyType)
                };
                parms1[0].Value = exchangeOrderId;
                parms1[1].Value = currentId;

                SQLHelper.NonQuery(connection, sqlText.ToString(), CommandType.Text, parms1);

                //更新待更换的
                parms2 = new SqlParameter[]{
                    new SqlParameter("@OrderId", DbType.Int32),
                    new SqlParameter("@"+ keyName, keyType)
                };
                parms2[0].Value = currentOrderId;
                parms2[1].Value = exchangeId;

                SQLHelper.NonQuery(connection, sqlText.ToString(), CommandType.Text, parms2);
            }
        }
        #endregion

        #region GetConnection 获取当前使用的数据库连接符
        /// <summary>
        /// 获取当前使用的数据库连接符
        /// </summary>
        /// <returns></returns>
        T ISQLFactory.GetConnection<T>()
        {
            //返回数据
            return (new SqlConnection(connection) as T);
        }
        #endregion

        #region GetBaseSelect 获取最基本的SELECT语句
        /// <summary>
        /// 获取最基本的SELECT语句，如 SELECT xx FROM xx WHERE xx
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="columns"></param>
        /// <param name="tableName"></param>
        /// <param name="where"></param>
        private static void GetBaseSelect(StringBuilder sqlText, List<string> columns, string tableName, string where, int pagesize)
        {
            sqlText.Append("SELECT ");

            if (pagesize > 0)
            {
                sqlText.Append("TOP ");
                sqlText.Append(pagesize);
                sqlText.Append(" ");
            }

            if (columns.Count > 0)
            {
                //第一个字段
                sqlText.Append(columns[0]);

                //第二个起所有字段
                int loop = columns.Count;
                for (int i = 1; i < loop; i++)
                {
                    sqlText.Append(",");
                    sqlText.Append(columns[i]);
                }
            }
            else
            {
                sqlText.Append("*");
            }

            sqlText.Append(" FROM ");
            sqlText.Append(tableName);
            sqlText.Append(where);
        }
        #endregion

        #region GetDbType 根据Type类型获取SQLSERVER的数据类型
        /// <summary>
        /// 根据Type类型获取SQL的数据类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="quote">返回在SQL语句中是否需要使用'</param>
        /// <returns></returns>
        private static SqlDbType GetDbType(Type type, ref bool quote)
        {
            quote = false;
            SqlDbType dbtype = SqlDbType.VarChar;

            if (type.Equals(typeof(string)))
            {
                quote = true;
            }
            else if (type.Equals(typeof(int)))
            {
                dbtype = SqlDbType.Int;
            }
            else if (type.Equals(typeof(bool)))
            {
                dbtype = SqlDbType.Bit;
            }
            else if (type.Equals(typeof(DateTime)))
            {
                quote = true;
                dbtype = SqlDbType.DateTime;
            }
            else if (type.Equals(typeof(decimal)))
            {
                dbtype = SqlDbType.Decimal;
            }
            else if (type.Equals(typeof(float)))
            {
                dbtype = SqlDbType.Float;
            }
            else if (type.Equals(typeof(double)))
            {
                dbtype = SqlDbType.Decimal;
            }
            else if (type.Equals(typeof(byte[])))
            {
                quote = true;
                dbtype = SqlDbType.Binary;
            }

            return dbtype;
        }

        #endregion
    }
}
