/*******************************************************************************************
* 00: ---------------------------------------------------------------
* 01: LT.Component.Generic.TTreeDepath
* 02: TTreeDepath 通用类
* 03: Devia
* 04: 2010-08-26 10 03 58
* 05: 20100826-10
* 06: 
*******************************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace LT.Component.Generic
{
    /// <summary>
    /// TTreeDepath 通用类
    /// </summary>
    public class TTreeDepath : TBase
    {
        private int _depth;

        /// <summary>
        /// 获取当前对象在树型目录是的深度
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict"></param>
        /// <param name="keyId"></param>
        /// <returns></returns>
        public int Get<T>(Dictionary<int, T> dict, int keyId) where T : class
        {
            _depth = 0;

            GetDepth(dict, keyId);

            return _depth;
        }

        private void GetDepth<T>(Dictionary<int, T> dict, int keyId) where T : class
        {
            _depth++;

            T t = dict[keyId];
            int rootId = Convert.ToInt32(t.GetType().GetProperty("ParentId").GetValue(t, null));
            if (rootId > 0)
            {
                GetDepth<T>(dict, rootId);
            }
        }
    }
}
