using System;
using System.IO;
using System.Text;
using System.Web;

namespace LT.Component.DataProvider
{
    /// <summary>
    /// FileHelper 通用类
    /// </summary>
    public class FileHelper
    {
        /// <summary>
        /// 将服务上物理文件发向客户端
        /// </summary>
        /// <param name="displayName">显示的名称，需要包括文件缀名</param>
        /// <param name="fileName">服务器物理文件全路径</param>
        /// <param name="isDeleteFile">下载完毕后，是否删除物理文件</param>
        public static void FlushFileToWeb(string displayName, string fileName, bool isDeleteFile)
        {
            if (!File.Exists(fileName))
            {
                return;
            }

            //文件信息
            FileInfo fileInfo = new FileInfo(fileName);
            string fileExt = fileInfo.Extension.ToLower();

            //重新添加文件扩展名
            int pos = displayName.LastIndexOf('.');
            if (pos == -1 || displayName.ToLower().Substring(pos) != fileExt)
            {
                displayName += fileExt;
            }

            //下载文件
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ClearHeaders();
            HttpContext.Current.Response.Buffer = false;
            HttpContext.Current.Response.ContentType = "application/octet-stream";
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(displayName, System.Text.Encoding.UTF8).Replace("+", " "));
            HttpContext.Current.Response.AppendHeader("Content-Length", fileInfo.Length.ToString());
            HttpContext.Current.Response.WriteFile(fileName);
            HttpContext.Current.Response.Flush();

            //删除文件
            if (isDeleteFile)
            {
                File.Delete(fileName);
            }
        }

        /// <summary>
        /// 结合得到的后缀名，生成文件名称
        /// </summary>
        /// <param name="fileExt"></param>
        /// <returns></returns>
        public static string CreatFileName(string fileExt)
        {
            string filename = DateTime.Now.ToString("yyyy-MM-dd,hh-mm-ss,");
            Random r = new Random();
            for (int i = 0; (i < 5); i++)
            {
                filename += r.Next(9);
            }

            return filename + fileExt;
        }

        /// <summary>
        /// 获取文件的文本内容
        /// </summary>
        /// <param name="fullfile">全路径的文件</param>
        /// <returns></returns>
        public static string ReadFileByFullname(string fullname)
        {
            if (!File.Exists(fullname))
            {
                return "";
            }

            //使用流的方式读取文本内容
            StreamReader reader = new StreamReader(fullname, Encoding.UTF8);
            string text = reader.ReadToEnd();
            reader.Close();

            //返回
            return text;
        }

        /// <summary>
        /// 写内容到指定文件中
        /// </summary>
        /// <param name="fullname"></param>
        /// <param name="text"></param>
        public static void WriteFileByFullname(string fullname, string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);

            FileStream stream = new FileStream(fullname, FileMode.Create, FileAccess.Write, FileShare.Write);
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
            stream.Close();
        }

        /// <summary>
        /// 把byte内容写到指定文件中
        /// </summary>
        /// <param name="fullname"></param>
        /// <param name="data">流数据</param>
        public static void WriteFileByFullname(string fullname, byte[] data)
        {
            FileStream stream = new FileStream(fullname, FileMode.Create, FileAccess.Write, FileShare.Write);
            if (data != null)
            {
                stream.Write(data, 0, data.Length);
                stream.Flush();
            }
            stream.Close();
        }

        /// <summary>
        /// 重命名文件
        /// </summary>
        /// <param name="orgfile"></param>
        /// <param name="newfile"></param>
        /// <param name="overwrite">如果目标文件存在，是否覆盖</param>
        /// <returns></returns>
        public static bool Rename(string orgfile, ref string newfile, bool overwrite)
        {
            if (!File.Exists(orgfile))
            {
                return false;
            }

            //如果原文件是只读，则需要更改为正常类型
            FileInfo info = new FileInfo(orgfile);
            if (info.IsReadOnly)
            {
                info.IsReadOnly = false;
            }

            //判断新文件是否存
            if (File.Exists(newfile))
            {
                //如果覆盖，则删除
                if (overwrite)
                {
                    //如果删除不成功，则返回false
                    if (!Delete(newfile))
                    {
                        return false;
                    }
                }
                else
                {
                    //如果存在，则重命名保存
                    //重命名规划，原来名称 xxxx.jpg
                    //重命名后的名字是 xxxxx(1).jpg
                    int i = 0;
                    int pos = newfile.LastIndexOf(".") - 1;
                    string zerofile = newfile.Substring(0, pos) + "({0})" + newfile.Substring(pos + 1);

                    while (true)
                    {
                        i++;
                        string chkfile = string.Format(zerofile, i);
                        if (!File.Exists(chkfile))
                        {
                            newfile = chkfile;
                            break;
                        }
                    }
                }
            }

            //执行重命名
            try
            {
                File.Move(orgfile, newfile);
            }
            catch
            {
                return false;
            }

            //返回成功
            return true;
        }

        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="orgfile"></param>
        /// <param name="descfile"></param>
        /// <param name="overwrite">如果目标文件存在，是否覆盖</param>
        public static bool Move(string orgfile, string descfile, bool overwrite)
        {
            if (!File.Exists(orgfile))
            {
                return false;
            }

            //如果原文件是只读，则需要更改为正常类型
            FileInfo info = new FileInfo(orgfile);
            if (info.IsReadOnly)
            {
                info.IsReadOnly = false;
            }

            if (File.Exists(descfile))
            {
                //如果覆盖，则删除
                if (overwrite)
                {
                    //如果删除不成功，则返回false
                    if (!Delete(descfile))
                    {
                        return false;
                    }
                }
                else
                {
                    //如果存在，则重命名保存
                    //重命名规划，原来名称 xxxx.jpg
                    //重命名后的名字是 xxxxx(1).jpg
                    int i = 0;
                    int pos = descfile.LastIndexOf(".") - 1;
                    string zerofile = descfile.Substring(0, pos) + "({0})" + descfile.Substring(pos + 1);

                    while (true)
                    {
                        i++;
                        string chkfile = string.Format(zerofile, i);
                        if (!File.Exists(chkfile))
                        {
                            descfile = chkfile;
                            break;
                        }
                    }
                }
            }

            //执行移动
            try
            {
                File.Move(orgfile, descfile);
            }
            catch
            {
                return false;
            }

            //返回成功
            return true;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fullname">完整路径的文件</param>
        /// <returns></returns>
        public static bool Delete(string fullname)
        {
            //文件不存在，当作是删除成功
            if (!File.Exists(fullname))
            {
                return true;
            }

            //如果是只读，则需要更改为正常类型
            FileInfo info = new FileInfo(fullname);
            if (info.IsReadOnly)
            {
                info.IsReadOnly = false;
            }

            //执行删除操作
            try
            {
                File.Delete(fullname);
            }
            catch
            {
                return false;
            }

            //返回成功
            return true;
        }

        /// <summary>
        /// 返回不重复的的文件名称
        /// </summary>
        /// <param name="fullname">文件名称</param>
        /// <param name="filetype">要判断文件的格式。例如：'txt';如果全匹配输入 '*'</param>
        /// <returns></returns>
        public static string CheckFilename(string fullname, string filetype)
        {
            int i = 0;
            string path = Path.GetDirectoryName(fullname);
            string filenamewithoutextension = Path.GetFileNameWithoutExtension(fullname);
            string temp = filenamewithoutextension;
            string ext = Path.GetExtension(fullname);

        //重试检查文件名是否重复
        TryToCheckStepNameExist:
            if (Directory.GetFiles(path, temp + "." + filetype).Length > 0)
            {
                i++;
                temp = filenamewithoutextension + "(" + i + ")";
                goto TryToCheckStepNameExist;
            }

            return Path.Combine(path, temp + ext);
        }
    }
}
