using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace LT.Component.TcpSocket
{
    /// <summary>
    /// SocketHelper 通用类
    /// </summary>
    public class SocketHelper
    {
        /// <summary>
        /// 接收Byte[]
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        static byte[] ReceiveByte(System.Net.Sockets.Socket handler, int len)
        {
            byte[] b = new byte[len];
            if (len == 0)
            {
                return b;
            }
            else if (len < 1024)
            {
                handler.Receive(b);
            }
            else
            {
                int readed = 0;
                int received = 0;
                int receiving = 0;
                byte[] temp;

                while (received < len)
                {
                    if ((received + 1024) <= len)
                    {
                        receiving = 1024;
                    }
                    else
                    {
                        receiving = len - received;
                    }

                    temp = new byte[receiving];
                    readed = handler.Receive(temp, 0, receiving, System.Net.Sockets.SocketFlags.None);
                    temp.CopyTo(b, received);
                    received += readed;
                }
            }

            return b;
        }

        /// <summary>
        /// 接收字符串
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string ReceiveString(System.Net.Sockets.Socket handler, int len)
        {
            byte[] b = ReceiveByte(handler, len);
            return Encoding.Default.GetString(b);
        }

        /// <summary>
        /// 接收INT
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static int ReceiveInt32(System.Net.Sockets.Socket handler, int len)
        {
            string s = ReceiveString(handler, len);
            if (s.Length == 0 || Regex.IsMatch(s, @"^\d+$") == false)
            {
                return 0;
            }
            return Int32.Parse(s);
        }

        /// <summary>
        /// 接收Byte[]
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] ReceiveBytes(System.Net.Sockets.Socket handler, int length)
        {
            return ReceiveByte(handler, length);
        }

        /// <summary>
        /// 接收Byte[]
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static byte[] ReceiveBytes(System.Net.Sockets.Socket handler)
        {
            int length = 0;

            //取命令长位长
            length = ReceiveInt32(handler, 1);
            if (length == 0)
            {
                return null;
            }

            //取命令位长
            length = ReceiveInt32(handler, length);
            if (length == 0)
            {
                return null;
            }

            //取命令
            return ReceiveByte(handler, length);
        }

        /// <summary>
        /// 发送字符串
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static string ReceiveString(System.Net.Sockets.Socket handler)
        {
            int len = 0;

            //取命令长位长
            len = ReceiveInt32(handler, 1);
            if (len == 0)
            {
                return null;
            }

            //取命令位长
            len = ReceiveInt32(handler, len);
            if (len == 0)
            {
                return null;
            }

            //取命令
            return ReceiveString(handler, len);
        }

        /// <summary>
        /// 接收文件
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="fullname"></param>
        public static void ReceiveFile(System.Net.Sockets.Socket handler, string fullname)
        {
            byte[] bytes = ReceiveBytes(handler);
            FileStream fs = new FileStream(fullname, FileMode.Create, FileAccess.Write, FileShare.Write);
            if (bytes != null)
            {
                fs.Write(bytes, 0, bytes.Length);
            }
            fs.Flush();
            fs.Close();
        }

        /// <summary>
        /// 通过Socket发送文件
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="fullname"></param>
        public static void SendFile(System.Net.Sockets.Socket handler, string fullname)
        {
            //获取文件流
            FileStream fs = new FileStream(fullname, FileMode.Open, FileAccess.Read);
            int len = (int)fs.Length;
            byte[] bytes = new byte[len];
            fs.Read(bytes, 0, len);
            fs.Close();

            //通过流发送
            SendBytes(handler, bytes);
        }

        /// <summary>
        /// 使用Socket发送字符
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="str"></param>
        public static void SendString(System.Net.Sockets.Socket handler, string str)
        {
            SendBytes(handler, Encoding.Default.GetBytes(str));
        }

        /// <summary>
        /// 使用Socket发送Byte[]
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="bytes"></param>
        public static void SendBytes(System.Net.Sockets.Socket handler, byte[] bytes)
        {
            //合并成发送字节
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

            if (nTotal < 1024)
            {
                //发送
                handler.Send(bTotals);
            }
            else
            {
                int sending = 0;
                int sended = 0;
                int send = 0;
                byte[] temp;
                while (sended < nTotal)
                {
                    if (sending + 1024 <= nTotal)
                    {
                        sending = 1024;
                    }
                    else
                    {
                        sending = nTotal - sended;
                    }
                    temp = new byte[sending];
                    send = handler.Send(temp, 0, temp.Length, System.Net.Sockets.SocketFlags.None);
                    sended += send;
                }
            }

        }

        /// <summary>
        /// 从HTTP请求头中获取参数
        /// </summary>
        /// <param name="handler"></param>
        public static string[] GetWebParam(System.Net.Sockets.Socket handler)
        {
            //转换成字符串类型
            var header = SocketHelper.ReceiveString(handler, 1000);

            //外来参数
            //0: url
            //1: ver
            //2: host
            var parms = new string[3];

            //URL和VER
            var match = Regex.Match(header, @"^([^\s]+)\s([^\r\n]+)");
            if (match.Success)
            {
                parms[0] = match.Result("$1");
                parms[1] = match.Result("$2");
            }

            //Host
            match = Regex.Match(header, @"Host:\s?([^\r\n]+)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                parms[2] = match.Result("$1");
            }

            //返回结果
            return parms;
        }

        /// <summary>
        /// 把字符发到Web浏览器去
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="parms"></param>
        /// <param name="str"></param>
        public static void SendWebString(System.Net.Sockets.Socket handler, string[] parms, string str)
        {
            //组装HTTP头
            var header = new StringBuilder();
            header.AppendFormat("{0} 200 OK\r\n", parms[1]);
            header.AppendFormat("Server: {0}\r\n", parms[2]);
            header.AppendFormat("Content-Type: {0}\r\n", MIMES.HTML);
            header.AppendFormat("Content-Length: {0}\r\n", Encoding.Default.GetBytes(str).Length);
            header.Append("\r\n");
            header.Append(str);

            //发送流
            var bytes = Encoding.Default.GetBytes(header.ToString());
            handler.Send(bytes);
        }

        /// <summary>
        /// 发送图片到Web浏览器去
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="parms"></param>
        /// <param name="file"></param>
        public static void SendWebImage(System.Net.Sockets.Socket handler, string[] parms, string file)
        {
            int len = 0;
            byte[] bytes = null;

            if (File.Exists(file))
            {
                //获取文件流
                FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
                len = (int)fs.Length;
                bytes = new byte[len];
                fs.Read(bytes, 0, len);
                fs.Close();

                //发送头
                SendWebHeader(parms[1], parms[2], "200 OK", MIMES.JPG, len, handler);
            }
            else
            {
                //转换成流
                bytes = Encoding.Default.GetBytes("404 Not Found");

                //发送头
                SendWebHeader(parms[1], parms[2], "404 Not Found", MIMES.HTML, bytes.Length, handler);
            }

            //发送流
            handler.Send(bytes);
        }

        /// <summary>
        /// 把HTTP头发到Web浏览器去
        /// </summary>
        /// <param name="ver"></param>
        /// <param name="host"></param>
        /// <param name="status"></param>
        /// <param name="mime"></param>
        /// <param name="len"></param>
        /// <param name="handler"></param>
        static void SendWebHeader(string ver, string host, string status, string mime, int len, System.Net.Sockets.Socket handler)
        {
            //satus
            //  200 OK
            //  404 Not Found

            //组装HTTP头
            var header = new StringBuilder();
            header.AppendFormat("{0} {1}\r\n", ver, status);
            header.AppendFormat("Server: {0}\r\n", host);
            header.AppendFormat("Content-Type: {0}\r\n", mime);
            header.AppendFormat("Content-Length: {0}\r\n\r\n", len);
            //header.AppendFormat("Accept-Ranges: bytes 0-{0}/{1}\r\n\r\n", len-1, len);

            //发送HTTP头
            var bytes = Encoding.Default.GetBytes(header.ToString());
            handler.Send(bytes);
        }

        /// <summary>
        /// MIME类型
        /// </summary>
        public struct MIMES
        {
            /// <summary>
            /// GIF
            /// </summary>
            public static string GIF = "image/gif";

            /// <summary>
            /// JPG
            /// </summary>
            public static string JPG = "image/jpeg";

            /// <summary>
            /// HTML
            /// </summary>
            public static string HTML = "text/html";

            /// <summary>
            /// TXT
            /// </summary>
            public static string TXT = "text/plain";
        }
    }
}
