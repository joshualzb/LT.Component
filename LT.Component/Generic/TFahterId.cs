using System;
using System.Collections.Generic;
using System.Text;

namespace LT.Component.Generic
{
    /// <summary>
    /// TFahterId 通用类
    /// 在对象模型中必段含有 ParentId 字段
    /// </summary>
    public class TFahterId : TBase
    {
        private string _spliter = ",";
        private StringBuilder _ids;

        /// <summary>
        /// 根据树型对象模型，获取指定ID的树路径ID（包括当前ID）
        /// 返回数据格式 ,4,5,6, (逗号开始和结束)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict"></param>
        /// <param name="lastId">树目录中最后的节点标识值</param>
        /// <returns></returns>
        public string Get<T>(Dictionary<int, T> dict, int lastId) where T : class
        {
            _ids = new StringBuilder();
            //递归从树终点往上获取
            GetIds(dict, lastId);
          
            //补充最后分隔符
            _ids.Append(_spliter);

            //返回数据
            return _ids.ToString();
        }

        private void GetIds<T>(Dictionary<int, T> dict, int lastId) where T : class
        {
            //添加本循环中最后节点
            if (dict.ContainsKey(lastId))
            {
                T t = dict[lastId];

                //往前获取
                lastId = Convert.ToInt32(t.GetType().GetProperty("ParentId").GetValue(t, null));
                GetIds(dict, lastId);

                //添加记录
                _ids.Append(_spliter);
                _ids.Append(t.GetType().GetProperty(KeyName).GetValue(t, null));
            }
            else
            {
                return;
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
