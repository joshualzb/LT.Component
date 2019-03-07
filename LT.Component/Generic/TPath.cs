using System;
using System.Collections.Generic;

namespace LT.Component.Generic
{
    /// <summary>
    /// TPath 通用类
    /// 在对象模型中必段含有 ParentId 字段
    /// </summary>
    public class TPath : TBase
    {
        /// <summary>
        /// 根据树型对象模型，获取指定ID的树路径排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict"></param>
        /// <param name="lastId">树目录中最后的节点标识值</param>
        /// <returns></returns>
        public List<T> Get<T>(Dictionary<int, T> dict, int lastId) where T : class
        {
            //创建一个新容器List
            List<T> lists = new List<T>();

            //递归从树终点往上获取
            GetPath(dict, lastId, ref lists);

            return lists;
        }

        private void GetPath<T>(Dictionary<int, T> dict, int lastId, ref List<T> lists) where T : class
        {
            //添加本循环中最后节点
            if (dict.ContainsKey(lastId))
            {
                T t = dict[lastId];

                //往前获取
                lastId = Convert.ToInt32(t.GetType().GetProperty("ParentId").GetValue(t, null));
                GetPath(dict, lastId, ref lists);

                //添加记录
                lists.Add(t);
            }
            else
            {
                return;
            }
        }
    }
}
