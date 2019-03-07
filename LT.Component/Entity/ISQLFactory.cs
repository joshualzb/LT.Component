using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace LT.Component.Entity
{
    /// <summary>
    /// ISQLFactory 通用类
    /// </summary>
    public interface ISQLFactory
    {
        /// <summary>
        /// 根据主键返回指定一个字段的一个值
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="keyValue">数据表主键的值</param>
        /// <param name="customKey">自定义临时主键。如不设置则使用表默认值</param>
        /// <param name="customColumn">自定义的字段名，只能一个</param>
        /// <returns></returns>
        object GetExecuteScalarByKey<T>(object keyValue, string customKey, string customColumn) where T : class;

        /// <summary>
        /// 根据条件返回指定一个字段的一个值
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="where">查询条件，必须以 AND 开头，如 AND SortId=1</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=[?SortId] or [@SortId]</param>
        /// <param name="orderBy">排序，如 LogId DESC 或 LogId DESC,IsShow ASC</param>
        /// <param name="customColumn">自定义的字段名，只能一个</param>
        /// <returns></returns>
        object GetExecuteScalarByWhere<T>(string where, DbParameter[] parms, string orderBy, string customColumn) where T : class;

        /// <summary>
        /// 根据条件获取首行记录对象
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="where">查询条件，必须以 AND 开头，如 AND SortId=1</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=[?SortId] or [@SortId]</param>
        /// <param name="orderBy">排序，如 LogId DESC 或 LogId DESC,IsShow ASC</param>
        /// <param name="customColumns">自定义查询列名集合，使用逗号分隔。如不需要则为null</param>
        /// <returns></returns>
        T GetModelSingleByWhere<T>(string where, DbParameter[] parms, string orderBy, string customColumns) where T : class;

        /// <summary>
        /// 根据条件获取首行记录对象
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="where">查询条件，必须以 AND 开头，如 AND SortId=1</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=[?SortId] or [@SortId]</param>
        /// <param name="orderBy">排序，如 LogId DESC 或 LogId DESC,IsShow ASC</param>
        /// <param name="customColumns">自定义查询列名集合，使用逗号分隔。如不需要则为null</param>
        /// <returns></returns>
        DataTable GetTableSingleByWhere<T>(string where, DbParameter[] parms, string orderBy, string customColumns) where T : class;

        /// <summary>
        /// 通过主键获取单个对象
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="keyValue">数据表主键的值</param>
        /// <param name="customKey">自定义临时主键。如不设置则使用表默认值</param>
        /// <param name="customColumns">自定义查询列名集合，使用逗号分隔。如不需要则为null</param>
        /// <returns></returns>
        T GetModelSingleByKey<T>(object keyValue, string customKey, string customColumns) where T : class;

        /// <summary>
        /// 通过主键获取单个对象
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="keyValue">数据表主键的值</param>
        /// <param name="customKey">自定义临时主键。如不设置则使用表默认值</param>
        /// <param name="customColumns">自定义查询列名集合，使用逗号分隔。如不需要则为null</param>
        /// <returns></returns>
        DataTable GetTableSingleByKey<T>(object keyValue, string customKey, string customColumns) where T : class;

        /// <summary>
        /// 获取数据字典对象（Key值限制为INT型，且在Model模型中排第一位）
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="pageSize">记录条数，0表示全部读取</param>
        /// <param name="where">查询条件，必须以 AND 开头，如 AND SortId=1 AND IsShow=1</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=?SortId</param>
        /// <param name="orderBy">排序，如 LogId DESC 或 LogId DESC,IsShow ASC</param>
        /// <param name="customColumns">自定义查询列名集合，使用逗号分隔。如不需要则为null</param>
        /// <returns></returns>
        Dictionary<int, T> GetModelIntDictionary<T>(int pageSize, string where, DbParameter[] parms, string orderBy, string customColumns) where T : class;

        /// <summary>
        /// 获取数据字典对象（Key值限制为String型，且在Model模型中排第一位）
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="pageSize">记录条数，0表示全部读取</param>
        /// <param name="where">查询条件，必须以 AND 开头，如 AND SortId=1 AND IsShow=1</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=?SortId</param>
        /// <param name="orderBy">排序，如 LogId DESC 或 LogId DESC,IsShow ASC</param>
        /// <param name="customKey">自定义临时主键。如不设置则使用表默认值</param>
        /// <param name="customColumns">自定义查询列名集合，使用逗号分隔。如不需要则为null</param>
        /// <returns></returns>
        Dictionary<string, T> GetModelStringDictionary<T>(int pageSize, string where, DbParameter[] parms, string orderBy, string customKey, string customColumns) where T : class;

        /// <summary>
        /// 获取单页列表数据对象
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="pageSize">记录条数，0表示全部读取</param>
        /// <param name="where">查询条件，必须以 AND 开头，如 AND SortId=1 AND IsShow=1</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=?SortId</param>
        /// <param name="orderBy">排序，如 LogId DESC 或 LogId DESC,IsShow ASC</param>
        /// <param name="customColumns">自定义查询列名集合，使用逗号分隔。如不需要则为null</param>
        /// <returns></returns>
        List<T> GetModelList<T>(int pageSize, string where, DbParameter[] parms, string orderBy, string customColumns) where T : class;

        /// <summary>
        /// 通过完整的Sql语句获得列表数据对象
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="commandType">执行语句的类型</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=?SortId</param>
        /// <returns></returns>
        DataSet GetDataSet(string sqlText, CommandType commandType, DbParameter[] parms);

        /// <summary>
        /// 通过完整的Sql语句获得列表数据对象
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="commandType">执行语句的类型</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=?SortId</param>
        /// <returns></returns>
        DataTable GetTableList(string sqlText, CommandType commandType, DbParameter[] parms);

        /// <summary>
        /// 获取单页列表数据列表
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="pageSize">记录条数，0表示全部读取</param>
        /// <param name="where">查询条件，必须以 AND 开头，如 AND SortId=1 AND IsShow=1</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=?SortId</param>
        /// <param name="orderBy">排序，如 LogId DESC 或 LogId DESC,IsShow ASC</param>
        /// <param name="customColumns">自定义查询列名集合，使用逗号分隔。如不需要则为null</param>
        /// <returns></returns>
        DataTable GetTableList<T>(int pageSize, string where, DbParameter[] parms, string orderBy, string customColumns) where T : class;

        /// <summary>
        /// 获取翻页列表数据对象
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="pageSize">记录条数，0表示全部读取</param>
        /// <param name="currentPage">当前页码</param>
        /// <param name="records">返回当前条件下总部的记录数</param>
        /// <param name="where">查询条件，必须以 AND 开头，如 AND SortId=1 AND IsShow=1</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=?SortId</param>
        /// <param name="orderBy">排序，如 LogId DESC 或 LogId DESC,IsShow ASC</param>
        /// <param name="customColumns">自定义查询列名集合，使用逗号分隔。如不需要则为null</param>
        /// <returns></returns>
        List<T> GetModelPager<T>(int pageSize, int currentPage, ref int records, string where, DbParameter[] parms, string orderBy, string customColumns) where T : class;

        /// <summary>
        /// 获取翻页列表数据对象
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="pageSize">记录条数，0表示全部读取</param>
        /// <param name="currentPage">当前页码</param>
        /// <param name="records">返回当前条件下总部的记录数</param>
        /// <param name="where">查询条件，必须以 AND 开头，如 AND SortId=1 AND IsShow=1</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=?SortId</param>
        /// <param name="orderBy">排序，如 LogId DESC 或 LogId DESC,IsShow ASC</param>
        /// <param name="columns">输出的列名集合，使用逗号分隔</param>
        /// <param name="tableName">数据表或视图名</param>
        /// <returns></returns>
        DataTable GetTablePager(int pageSize, int currentPage, ref int records, string where, DbParameter[] parms, string orderBy, string columns, string tableName);

        /// <summary>
        /// 获取翻页列表数据对象
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="pageSize">记录条数，0表示全部读取</param>
        /// <param name="currentPage">当前页码</param>
        /// <param name="records">返回当前条件下总部的记录数</param>
        /// <param name="where">查询条件，必须以 AND 开头，如 AND SortId=1 AND IsShow=1</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=?SortId</param>
        /// <param name="orderBy">排序，如 LogId DESC 或 LogId DESC,IsShow ASC</param>
        /// <param name="customColumns">自定义查询列名集合，使用逗号分隔。如不需要则为null</param>
        /// <returns></returns>
        DataTable GetTablePager<T>(int pageSize, int currentPage, ref int records, string where, DbParameter[] parms, string orderBy, string customColumns) where T : class;

        /// <summary>
        /// 获取数据对象
        /// </summary>
        /// <param name="queryString">sql语句</param>
        /// <param name="parms">参数</param>
        DataTable ExecuteSQL(string queryString, DbParameter[] parms);

        /// <summary>
        /// 获取满足条件的数量
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="where">查询条件，必须以 AND 开头，如 AND SortId=1 AND IsShow=1</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=?SortId</param>
        /// <returns></returns>
        int GetCount<T>(string where, DbParameter[] parms) where T : class;

        /// <summary>
        /// 获取数据表整型的Distinct数据集合
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="column">输出的字段，只能一个</param>
        /// <param name="where">查询条件，必须以 AND 开头，如 AND SortId=1 AND IsShow=1</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=?SortId</param>
        /// <returns></returns>
        List<int> GetIntDistinct<T>(string column, string where, DbParameter[] parms) where T : class;

        /// <summary>
        /// 获取数据表字符型的Distinct数据集合
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="column">输出的字段，只能一个</param>
        /// <param name="where">查询条件，必须以 AND 开头，如 AND SortId=1 AND IsShow=1</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=?SortId</param>
        /// <returns></returns>
        List<string> GetStringDistinct<T>(string column, string where, DbParameter[] parms) where T : class;

        /// <summary>
        /// 把对象内容保存到数据库中
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="model">更新对象模型</param>
        /// <param name="returnType">
        ///     返回的数据类型：
        ///     EffectRow： 为返回受影响行数；
        ///     Identity： 返回最新插入主键值
        ///     OrderId：更新OrderId值，并返回最后插入的主键值
        ///     None：不需要返回值
        /// </param>
        /// <returns></returns>
        object Insert<T>(T model, ReturnTypes returnType) where T : class;

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
        int Insert<T>(string sql, DbParameter[] parms, ReturnTypes returnType) where T : class;

        /// <summary>
        /// 根据主键值更新对象数据
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="model">更新对象模型</param>
        /// <param name="customColumns">自定义查询列名集合，使用逗号分隔。如不需要则为null</param>
        void Update<T>(T model, string customColumns) where T : class;

        /// <summary>
        /// 根据条件更新符合条件的数据
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="model">更新对象模型</param>
        /// <param name="where">更新的where条件，如果没有，则使用null（where条件使用 AND 开头）</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=?SortId</param>
        /// <param name="customColumns">自定义查询列名集合，使用逗号分隔。如不需要则为null</param>
        void Update<T>(T model, string where, DbParameter[] parms, string customColumns) where T : class;

        /// <summary>
        /// 根据条件更新符合条件的SQL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">待执行更新语句的后半部分（从SET开始并含有一个空格）</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=?SortId</param>
        void Update<T>(string sql, DbParameter[] parms) where T : class;

        /// <summary>
        /// 更新多个字段的内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns">目标字段的数组</param>
        /// <param name="contents">需要更新内容的数组</param>
        void Update<T>(string[] columns, object[] contents) where T : class;

        /// <summary>
        /// 根据主键更新指定字段的内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyValue">主键值</param>
        /// <param name="column">目标字段</param>
        /// <param name="content">需要更新的内容</param>
        void Update<T>(object keyValue, string column, object content) where T : class;

        /// <summary>
        /// 根据主键更新多个字段的内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyValue">主键值</param>
        /// <param name="columns">目标字段的数组</param>
        /// <param name="contents">需要更新内容的数组</param>
        void Update<T>(object keyValue, string[] columns, object[] contents) where T : class;

        /// <summary>
        /// 根据主键更新排序ID
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="keyValue">数据表主键的值</param>
        void UpdateOrderId<T>(object keyValue) where T : class;

        /// <summary>
        /// 根据主键删除记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyValue">数据表主键的值</param>
        /// <param name="customKey">自定义临时主键。如不设置则使用表默认值</param>
        /// <returns></returns>
        int DeleteByKey<T>(object keyValue, string customKey) where T : class;

        /// <summary>
        /// 根据条件删除符合条件的记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where">删除的where条件，如果没有，则使用null（where条件使用 AND 开头）</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=?SortId</param>
        /// <returns></returns>
        int DeleteByWhere<T>(string where, DbParameter[] parms) where T : class;

        /// <summary>
        /// 根据SQL语句执行
        /// </summary>
        /// <param name="sqlText">全SQL执行语句</param>
        /// <param name="commandType">执行语句的类型</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=?SortId</param>
        /// <returns></returns>
        int NonQuery(string sqlText, CommandType commandType, DbParameter[] parms);

        /// <summary>
        /// 根据SQL语句执行返回DataReader
        /// </summary>
        /// <typeparam name="D">DataReader类型</typeparam>
        /// <param name="sqlText">待执行的SQL语句</param>
        /// <param name="commandType">执行语句的类型</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=?SortId</param>
        /// <returns></returns>
        D DataReader<D>(string sqlText, CommandType commandType, DbParameter[] parms) where D : class;

        /// <summary>
        /// 根据SQL语句执行返回DataReader
        /// </summary>
        /// <typeparam name="D">DataReader类型</typeparam>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="pageSize">记录条数，0表示全部读取</param>
        /// <param name="where">读取的where条件，如果没有，则使用null（where条件使用 AND 开头）</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=?SortId</param>
        /// <param name="orderBy">排序，如 LogId DESC 或 LogId DESC,IsShow ASC</param>
        /// <param name="customColumns">自定义查询列名集合，使用逗号分隔（不能为空）</param>
        /// <returns></returns>
        D DataReader<D, T>(int pageSize, string where, DbParameter[] parms, string orderBy, string customColumns)
            where D : class
            where T : class;

        /// <summary>
        /// 根据主键值判断是否存在记录
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="keyValue">数据表主键的值</param>
        /// <returns></returns>
        bool IsExist<T>(object keyValue) where T : class;

        /// <summary>
        /// 根据字段值判断是否存在记录
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="keyValue">数据表字段的值</param>
        /// <param name="customColumns">自定义临时主键</param>
        /// <returns></returns>
        bool IsExist<T>(object keyValue, string customColumns) where T : class;

        /// <summary>
        /// 根据条件判断是否符合要求的记录
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="where">删除的where条件，如果没有，则使用null（where条件使用 AND 开头）</param>
        /// <param name="parms">查询条件集合，当使用此项时，where语句变成 AND SortId=?SortId</param>
        /// <returns></returns>
        bool IsExist<T>(string where, DbParameter[] parms) where T : class;

        /// <summary>
        /// 根据主键值把表的OrderId值进行更换
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="currentId">当前数据ID</param>
        /// <param name="exchangeId">待更换数据ID</param>
        void OrderAct<T>(object currentId, object exchangeId) where T : class;

        /// <summary>
        /// 获取当前数据连接对象
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <returns></returns>
        T GetConnection<T>() where T : class;
    }
}
