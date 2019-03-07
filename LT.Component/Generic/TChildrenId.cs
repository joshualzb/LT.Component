using System.Collections.Generic;
using System.Text;

namespace LT.Component.Generic
{

    /// <summary>
    /// TChildrenId 通用类
    /// </summary>
    public class TChildrenId : TBase
    {
        private object _id;
        private string _spliter = ",";
        private StringBuilder _ids = new StringBuilder();

        public string Get<T>(object startId, List<T> ts) where T : class
        {
            return Get<T>(startId, ts, false);
        }

        /// <summary>
        /// 获取子数据集，形成4,6,9的字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="startId"></param>
        /// <param name="ts"></param>
        /// <param name="IsIncludeStartId"></param>
        /// <returns></returns>
        public string Get<T>(object startId, List<T> ts, bool isIncludeStartId) where T : class
        {
            if (isIncludeStartId)
            {
                _ids.Append(_spliter);
                _ids.Append(startId);
            }

            GetIds(startId, ts, true);

            if (_ids.ToString().Length > 0)
            {
                _ids.Remove(0, 1);
            }

            return _ids.ToString();
        }

        /// <summary>
        /// 获取子数据集，形成4,6,9的字符串；只获取子级的数据。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="startId"></param>
        /// <param name="ts"></param>
        /// <returns></returns>
        public string GetNextIds<T>(object startId, List<T> ts, bool isIncludeStartId) where T : class
        {
            if (isIncludeStartId)
            {
                _ids.Append(_spliter);
                _ids.Append(startId);
            }

            GetIds(startId, ts, false);

            if (_ids.ToString().Length > 0)
            {
                _ids.Remove(0, 1);
            }

            return _ids.ToString();
        }

        private void GetIds<T>(object startId, List<T> ts, bool isGetSubIds) where T : class
        {
            foreach (T t in ts)
            {
                if (t.GetType().GetProperty("ParentId").GetValue(t, null).Equals(startId))
                {
                    _id = t.GetType().GetProperty(KeyName).GetValue(t, null);

                    _ids.Append(_spliter);
                    _ids.Append(_id);

                    if (isGetSubIds)
                    {
                        GetIds(_id, ts, isGetSubIds);
                    }
                }
            }
        }

        /// <summary>
        /// 字符集的分隔符，默认为英文状态的逗号
        /// </summary>
        public string Spliter
        {
            set { _spliter = value; }
        }
    }
}
