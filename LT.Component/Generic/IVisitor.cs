using System;

namespace LT.Component.Generic
{
    public interface IVisitor<T>
    {
        /// <summary>
        /// 是否已运行
        /// </summary>
        bool HasDone { get; }

        /// <summary>
        /// 访问指定的对象
        /// </summary>
        void Visit(T obj);
    }
}
