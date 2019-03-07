using System;
using System.Collections.Generic;

namespace LT.Component.Generic
{
    public class TQueue<T> : System.Collections.Generic.Queue<T>, ICustomCollection<T>
    {
        #region 构造函数
        public TQueue() : base() { }
        public TQueue(IEnumerable<T> collection) : base(collection) { }
        public TQueue(int capacity) : base(capacity) { }
        #endregion


        /// <summary>
        /// 返回当前实例
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// 是否为空
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return this.Count == 0;
            }
        }

        private int _fixedsize = default(int);
        /// <summary>
        /// 固定大小属性
        /// </summary>
        public int FixedSize
        {
            get
            {
                return _fixedsize;
            }
            set
            {
                _fixedsize = value;
            }
        }

        /// <summary>
        /// 当前队列是否已满
        /// </summary>
        public bool IsFull
        {
            get
            {
                if ((FixedSize != default(int)) && (this.Count >= FixedSize))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 版本
        /// </summary>
        public string Version
        {
            get
            {
                return "1.0";
            }
        }

        /// <summary>
        /// 作者
        /// </summary>
        public string Author
        {
            get
            {
                return "XiaoChen";
            }
        }

        /// <summary>
        /// 接受指定的访问方式(访问者模式)
        /// </summary>
        /// <param name="visitor"></param>
        public void Accept(IVisitor<T> visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException("访问对象为空");
            }

            System.Collections.Generic.Queue<T>.Enumerator enumerator = this.GetEnumerator();

            while (enumerator.MoveNext())
            {
                visitor.Visit(enumerator.Current);

                if (visitor.HasDone)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// 追加元素
        /// </summary>
        /// <param name="value"></param>
        public new void Enqueue(T value)
        {
            if (!this.IsFull)
            {
                base.Enqueue(value);
            }
        }


        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 比较对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("比较对象为空");
            }

            if (obj.GetType() == this.GetType())
            {
                TQueue<T> d = obj as TQueue<T>;
                return this.Count.CompareTo(d.Count);
            }
            else
            {
                return this.GetType().FullName.CompareTo(obj.GetType().FullName);
            }
        }

        /// <summary>
        /// 是否只读
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

    }
}
