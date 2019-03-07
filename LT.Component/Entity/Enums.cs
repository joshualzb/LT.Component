using System;

namespace LT.Component.Entity
{
    [Serializable]
    [Flags]
    public enum ColumnTypes
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        Increment = 1,

        /// <summary>
        /// 标识为主键
        /// </summary>
        Identity = 2,

        /// <summary>
        /// 不参与读取、增加、修改
        /// </summary>
        Extend = 4,

        /// <summary>
        /// 参与读取，不参与增加、修改
        /// </summary>
        Read = 8,

        /// <summary>
        /// 参与读取和修改状态，不参与插入
        /// </summary>
        ReadUpdate = 16,

        /// <summary>
        /// 参与读取和插入状态，不参与修改状态
        /// </summary>
        ReadInsert = 32,

        /// <summary>
        /// 表示可以通过URL中action的值调用
        /// </summary>
        SaflyAction = 110
    }

    /// <summary>
    /// 数据库插入数据后返回类型
    /// </summary>
    public enum ReturnTypes
    {
        /// <summary>
        /// 返回受影响的行数
        /// </summary>
        EffectRow,

        /// <summary>
        /// 返回最后插入的主键值
        /// </summary>
        Identity,

        /// <summary>
        /// 更新OrderId值，并返回最后插入的主键值
        /// 适合于主键是INT自增型
        /// </summary>
        OrderIdForIntAuto,

        /// <summary>
        /// 更新OrderId值，并返回最后插入的主键值
        /// 适合于主键是String类型的
        /// </summary>
        OrderIdForString,

        /// <summary>
        /// 不需要返回值
        /// </summary>
        None
    }
}
