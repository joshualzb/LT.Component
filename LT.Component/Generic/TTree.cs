/*******************************************************************************************
* 00: ---------------------------------------------------------------
* 01: LT.Component.Generic.TTree
* 02: TTree 通用类
* 02: 原对象模型中必含有TreePath属性
* 03: Devia
* 04: 2010-08-26 10 03 58
* 05: 20100826-10
* 06: 
*******************************************************************************************/

using System;
using System.Data;
using System.Reflection;
using System.Collections.Generic;

namespace LT.Component.Generic
{
    /// <summary>
    /// TTree 通用类
    /// 原对象模型中必含有TreePath属性
    /// </summary>
    public class TTree : TBase
    {
        private int _depth;
        private string _treePath;

        public TTree()
        {

        }

        public TTree(string keyName)
        {
            this.KeyName = keyName;
        }

        /// <summary>
        /// SpaceNode
        /// </summary>
        public string SpaceNode
        {
            set { _spaceNode = value; }
        }
        private string _spaceNode = "┆";

        /// <summary>
        /// EndNode
        /// </summary>
        public string EndNode
        {
            set { _endNode = value; }
        }
        private string _endNode = "└ ";

        /// <summary>
        /// BranchNode
        /// </summary>
        public string BranchNode
        {
            set { _branchNode = value; }
        }
        private string _branchNode = "├ ";

        /// <summary>
        /// 根据输入的列表，转换出树形排序（支持去掉指定的子）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">数据列表集合</param>
        /// <param name="startId">树目录起始ID</param>
        /// <param name="removeId">需要去除子的ID</param>
        /// <returns></returns>
        public List<T> Get<T>(List<T> list, object startId, object removeId) where T : class
        {
            _depth = 0;
            List<T> ts = new List<T>();

            GetTreeByList(list, startId, removeId, ref ts);

            return ts;
        }

        /// <summary>
        /// 根据输入的INT字典类，转换出树形排序（支持去掉指定的子）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict">INT数据字典集合</param>
        /// <param name="startId">树目录起始ID</param>
        /// <param name="removeId">需要去除子的ID</param>
        /// <returns></returns>
        public List<T> Get<T>(Dictionary<int, T> dict, object startId, object removeId) where T : class
        {
            _depth = 0;
            List<T> ts = new List<T>();

            GetTreeByIntDict(dict, startId, removeId, ref ts);

            return ts;
        }

        /// <summary>
        /// 根据输入的String字典类，转换出树形排序（支持去掉指定的子）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict">String数据字典集合</param>
        /// <param name="startId">树目录起始ID</param>
        /// <param name="removeId">需要去除子的ID</param>
        /// <returns></returns>
        public List<T> Get<T>(Dictionary<string, T> dict, object startId, object removeId) where T : class
        {
            _depth = 0;
            List<T> ts = new List<T>();

            GetTreeByStringDict(dict, startId, removeId, ref ts);

            return ts;
        }

        void GetTreeByList<T>(List<T> list, object startId, object removeId, ref List<T> ts) where T : class
        {
            _depth++;
            int i = 0;
            int j = 0;
            int m = 0;
            object keyId;

            //count
            foreach (T sort in list)
            {
                if (sort.GetType().GetProperty("ParentId").GetValue(sort, null).Equals(startId))
                {
                    j++;
                }
            }

            //add
            foreach (T t in list)
            {
                if (t.GetType().GetProperty("ParentId").GetValue(t, null).Equals(startId))
                {
                    keyId = t.GetType().GetProperty(KeyName).GetValue(t, null);
                    if (keyId.Equals(removeId))
                    {
                        j--;
                    }
                    else
                    {
                        m++;
                        _treePath = "";
                        for (i = 1; i < _depth; i++)
                        {
                            _treePath += _spaceNode;
                        }
                        if (m < j)
                        {
                            _treePath += _branchNode;
                        }
                        else
                        {
                            _treePath += _endNode;
                        }

                        t.GetType().GetProperty("TreePath").SetValue(t, _treePath, null);
                        ts.Add(t);

                        GetTreeByList(list, keyId, removeId, ref ts);
                    }
                }
            }
            _depth--;
        }

        void GetTreeByIntDict<T>(Dictionary<int, T> dict, object startId, object removeId, ref List<T> ts) where T : class
        {
            _depth++;
            int i = 0;
            int j = 0;
            int m = 0;
            object keyId;

            //count
            foreach (T sort in dict.Values)
            {
                if (sort.GetType().GetProperty("ParentId").GetValue(sort, null).Equals(startId))
                {
                    j++;
                }
            }

            //add
            foreach (T t in dict.Values)
            {
                if (t.GetType().GetProperty("ParentId").GetValue(t, null).Equals(startId))
                {
                    keyId = t.GetType().GetProperty(KeyName).GetValue(t, null);
                    if (keyId.Equals(removeId))
                    {
                        j--;
                    }
                    else
                    {
                        m++;
                        _treePath = "";
                        for (i = 1; i < _depth; i++)
                        {
                            _treePath += _spaceNode;
                        }
                        if (m < j)
                        {
                            _treePath += _branchNode;
                        }
                        else
                        {
                            _treePath += _endNode;
                        }

                        t.GetType().GetProperty("TreePath").SetValue(t, _treePath, null);
                        ts.Add(t);

                        GetTreeByIntDict(dict, keyId, removeId, ref ts);
                    }
                }
            }
            _depth--;
        }

        void GetTreeByStringDict<T>(Dictionary<string, T> dict, object startId, object removeId, ref List<T> ts) where T : class
        {
            _depth++;
            int i = 0;
            int j = 0;
            int m = 0;
            object keyId;

            //count
            foreach (T sort in dict.Values)
            {
                if (sort.GetType().GetProperty("ParentId").GetValue(sort, null).Equals(startId))
                {
                    j++;
                }
            }

            //add
            foreach (T t in dict.Values)
            {
                if (t.GetType().GetProperty("ParentId").GetValue(t, null).Equals(startId))
                {
                    keyId = t.GetType().GetProperty(KeyName).GetValue(t, null);
                    if (keyId.Equals(removeId))
                    {
                        j--;
                    }
                    else
                    {
                        m++;
                        _treePath = "";
                        for (i = 1; i < _depth; i++)
                        {
                            _treePath += _spaceNode;
                        }
                        if (m < j)
                        {
                            _treePath += _branchNode;
                        }
                        else
                        {
                            _treePath += _endNode;
                        }

                        t.GetType().GetProperty("TreePath").SetValue(t, _treePath, null);
                        ts.Add(t);

                        GetTreeByStringDict(dict, keyId, removeId, ref ts);
                    }
                }
            }
            _depth--;
        }
    }
}
