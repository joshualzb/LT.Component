using System;

namespace LT.Component.Entity
{

    /// <summary>
    /// PropertyAttribute 通用类
    /// </summary>
    public class PropertyAttribute : Attribute
    {
        /// <summary>
        /// 设置和获取数据库表名
        /// </summary>
        public string TableName
        {
            set
            {
                _tableName = value;
            }
            get
            {
                return _tableName;
            }
        }
        private string _tableName;

        /// <summary>
        /// 设置和获取字段类型
        /// </summary>
        public ColumnTypes ColumnType
        {
            set
            {
                _columnType = value;
            }
            get
            {
                return _columnType;
            }
        }
        private ColumnTypes _columnType;

        /// <summary>
        /// 重载设置数据库表
        /// </summary>
        /// <param name="tableName"></param>
        public PropertyAttribute(string tableName)
        {
            this._tableName = tableName;
        }

        /// <summary>
        /// 重载设置字段类型
        /// </summary>
        /// <param name="columnType"></param>
        public PropertyAttribute(ColumnTypes columnType)
        {
            this._columnType = columnType;
        }
    }
}
