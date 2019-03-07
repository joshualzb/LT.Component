using System.IO;

namespace LT.Component.DataProvider
{
    /// <summary>
    /// FolderHelper 通用类
    /// </summary>
    public class FolderHelper
    {
        /// <summary>
        /// 检查指定的目录是否为空
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsEmpty(string path)
        {
            //不存在当前目录
            if (!Directory.Exists(path))
            {
                return true;
            }

            //递归检查
            return LoopCheckIsEmpty(path);
        }

        private static bool LoopCheckIsEmpty(string path)
        {
            //检查当前目录下是否有文件
            FileInfo info;
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                info = new FileInfo(file);
                if (info.Length > 0)
                {
                    return false;
                }
            }

            //检查所有子文件夹
            string[] folders = Directory.GetDirectories(path);
            foreach (string folder in folders)
            {
                return LoopCheckIsEmpty(folder);
            }

            //返回
            return true;
        }

        /// <summary>
        /// 删除指定的文件夹，包括所有子目录及文件
        /// </summary>
        /// <param name="path"></param>
        public static void Delete(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }
    }
}
