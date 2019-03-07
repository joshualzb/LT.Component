using System;
using System.Collections.Generic;

namespace LT.Component.Generic
{
    public interface ICustomCollection<T> : ICollection<T>, IComparable
    {
        /// <summary>
        /// 固定大小
        /// </summary>
        int FixedSize { get; }

        /// <summary>
        /// 集合类是否为空
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// 集合类是否已满
        /// </summary>
        bool IsFull { get; }

        /// <summary>
        /// 版本
        /// </summary>
        string Version { get; }

        /// <summary>
        /// 作者
        /// </summary>
        string Author { get; }
    }
}
