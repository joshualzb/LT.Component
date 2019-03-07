using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace LT.Component.Socket
{
    /// <summary>
    /// SocketHelper 通用类
    /// </summary>
    public class SocketHelper
    {
        public static byte[] ReceiveByte(Stream stream, int length)
        {
            byte[] b = new byte[length];

            if (length < 1024)
            {
                stream.Read(b, 0, length);
            }
            else
            {
                int readed = 0;
                int received = 0;
                int receiving = 0;
                byte[] temp;

                while (received < length)
                {
                    if ((received + 1024) <= length)
                    {
                        receiving = 1024;
                    }
                    else
                    {
                        receiving = length - received;
                    }

                    temp = new byte[receiving];
                    readed = stream.Read(temp, 0, receiving);

                    temp.CopyTo(b, received);
                    received += readed;
                }
            }

            return b;
        }

        public static string ReceiveString(Stream stream, int length)
        {
            byte[] b = ReceiveByte(stream, length);
            return Encoding.Default.GetString(b);
        }

        public static int ReceiveInt32(Stream stream, int length)
        {
            string s = ReceiveString(stream, length);
            if (s.Length == 0 || Regex.IsMatch(s, @"^\d+$") == false)
            {
                return 0;
            }
            return Int32.Parse(s);
        }

        public static byte[] ReceiveBytes(Stream stream)
        {
            int length = 0;

            //取命令长位长
            length = ReceiveInt32(stream, 1);
            if (length == 0)
            {
                return null;
            }

            //取命令位长
            length = ReceiveInt32(stream, length);
            if (length == 0)
            {
                return null;
            }

            //取命令
            return ReceiveByte(stream, length);
        }

        public static byte[] ReceiveBytes(Stream stream, int length)
        {
            return ReceiveByte(stream, length);
        }

        /// <summary>
        /// 与 SendString(NetworkStream stream, string str) 配对
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string ReceiveString(Stream stream)
        {
            int length = 0;

            //取命令长位长
            length = ReceiveInt32(stream, 1);
            if (length == 0)
            {
                return null;
            }

            //取命令位长
            length = ReceiveInt32(stream, length);
            if (length == 0)
            {
                return null;
            }

            //取命令
            return ReceiveString(stream, length);
        }

        /// <summary>
        /// 通过指定全路径名接收文件
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="fullname">全路径名</param>
        public static void ReceiveFile(Stream stream, string fullname)
        {
            byte[] bytes = ReceiveBytes(stream);

            FileStream fs = new FileStream(fullname, FileMode.Create, FileAccess.Write, FileShare.Write);
            fs.Write(bytes, 0, bytes.Length);
            fs.Flush();
            fs.Close();
        }


        /// <summary>
        /// 通过流发送文件
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fullname"></param>
        public static void SendFile(Stream stream, string fullname)
        {
            //获取文件流
            FileStream fs = new FileStream(fullname, FileMode.Open, FileAccess.Read);
            int len = (int)fs.Length;
            byte[] bytes = new byte[len];
            fs.Read(bytes, 0, len);
            fs.Close();

            //通过流发送
            SendBytes(stream, bytes);
        }

        /// <summary>
        /// 与 ReceiveString(NetworkStream stream) 配对
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="str"></param>
        public static void SendString(Stream stream, string str)
        {
            SendBytes(stream, Encoding.Default.GetBytes(str));
        }

        /// <summary>
        /// 与 ReceiveString(NetworkStream stream) 配对
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="bytes"></param>
        public static void SendBytes(Stream stream, byte[] bytes)
        {
            byte[] bThird = bytes; //第三部分(内容本身)
            int nThird = bThird.Length; //命令位长

            byte[] bSecond = Encoding.Default.GetBytes(nThird.ToString()); //第二部分(内容位长)
            int nSecond = bSecond.Length; //命令长位长

            byte[] bFirst = Encoding.Default.GetBytes(nSecond.ToString()); //第一部分(内容长位长)
            int nFirst = bFirst.Length; //肯定是1位数

            //形成最终要发送的bytes
            int nTotal = nThird + nSecond + nFirst;
            byte[] bTotals = new byte[nTotal];
            bFirst.CopyTo(bTotals, 0);
            bSecond.CopyTo(bTotals, nFirst);
            bThird.CopyTo(bTotals, nFirst + nSecond);

            stream.Write(bTotals, 0, nTotal);
        }
    }
}
