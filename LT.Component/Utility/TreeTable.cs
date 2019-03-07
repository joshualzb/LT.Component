using System.Data;

namespace LT.Component.Utility
{
    /// <summary>
    /// TreeTable 通用类
    /// </summary>
    public class TreeTable
    {
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
        /// 将DataReader转换成DataTable
        /// </summary>
        /// <param name="list">DataTable数据源</param>
        /// <param name="keyColumn">主键列名</param>
        /// <returns></returns>
        public DataTable Get(DataTable list, string keyColumn, object startId, object removeId)
        {
            if (list.Columns.IndexOf("TreePath") == -1)
            {
                list.Columns.Add("TreePath", typeof(string));
            }

            DataTable outs = list.Clone();
            int keyIndex = outs.Columns.IndexOf(keyColumn);
            int pathIndex = outs.Columns.Count - 1;

            GetTree(list, startId, removeId, ref outs, keyIndex, pathIndex, 0);

            return outs;
        }

        void GetTree(DataTable list, object startId, object removeId, ref DataTable outs, int keyIndex, int pathIndex, int depth)
        {
            int i = 0;
            int j = 0;
            int m = 0;

            DataRow[] drs = list.Select("[ParentId]='" + startId + "'");

            //count
            j = drs.Length;

            //add
            for (i = 0; i < j; i++)
            {
                //处理Path
                string treePath = "";
                for (m = 0; m < depth; m++)
                {
                    treePath += _spaceNode;
                }
                if (i < (j - 1))
                {
                    treePath += _branchNode;
                }
                else
                {
                    treePath += _endNode;
                }

                //过滤要排除的ID
                if (drs[i][keyIndex].Equals(removeId))
                {
                    continue;
                }

                startId = drs[i][keyIndex];
                drs[i][pathIndex] = treePath;
                outs.ImportRow(drs[i]);

                GetTree(list, startId, removeId, ref outs, keyIndex, pathIndex, depth + 1);
            }
        }
    }
}
