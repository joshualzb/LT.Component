/*******************************************************************************************
* DES: Model 类
* UID: de1b5366-c5c3-4a95-9c37-eb17d79a563e
* AUT: Joshua
* DTE: 1/21/2013 6:58:11 PM
* VER: 1.00
*******************************************************************************************/

namespace LT.Component.Entity.ExtModel
{
    public class Model<T> where T : class
    {
        /// <summary>
        /// 复制对象
        /// </summary>
        /// <returns>T</returns>
        public T Clone()
        {
            return this.MemberwiseClone() as T;
        }

        /// <summary>
        /// 对应数据库表名
        /// </summary>
        [Property(ColumnTypes.Extend)]
        public string DB_TableName
        {
            get
            {
                return EntityHelper.GetTableName(typeof(T));
            }
        }
    }
}
