using System.Collections.Generic;

namespace LT.Component.Generic
{
    /// <summary>
    /// TRootId 通用类
    /// </summary>
    public class TRootId : TBase
    {
        public TRootId(string keyName)
        {
            this.KeyName = keyName;
        }

        /// <summary>
        /// 根据某节点获取根节点
        /// 根节点必须为 0
        /// </summary>
        /// <typeparam name="T">根据树型对象模型</typeparam>
        /// <param name="list">节点集合</param>
        /// <param name="currentId">子节点值</param>
        /// <returns>节点的根节点</returns>
        public object Get<T>(List<T> list, object currentId) where T : class
        {
            T model = null;
            var type = typeof(T);
            var key = type.GetProperty(KeyName);
            var parent = type.GetProperty("ParentId");

            for (int i = 0; i < list.Count; i++)
            {
                model = list[i];
                if (currentId.Equals(key.GetValue(model, null)))
                {
                    var parentId = parent.GetValue(model, null);

                    //根节点的时候
                    if (parentId.Equals("0"))
                    {
                        break;
                    }
                    else
                    {
                        currentId = parentId;
                        i = -1;
                        continue;
                    }
                }
            }
            return currentId;
        }
    }
}
