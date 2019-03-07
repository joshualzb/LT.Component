using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LT.Component.TcpSocket
{
    /// <summary>
    /// Tcp Client 通用类
    /// </summary>
    public class TcpClient : IDisposable
    {
        /// <summary>
        /// client sender
        /// </summary>
        private System.Net.Sockets.Socket sender = null;

        /// <summary>
        /// 客户端连接
        /// </summary>
        /// <param name="listenIP">所连接服务器的IP地址</param>
        /// <param name="listenPort">所连接服务器的端口</param>
        public TcpClient(string listenIP, int listenPort)
        {
            // Establish the remote endpoint for the socket.
            // This example uses port 11000 on the local computer.
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(listenIP), listenPort);

            // Create a TCP/IP  socket.
            sender = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Connect the socket to the remote endpoint. Catch any errors.
            sender.Connect(remoteEP);
        }

        /// <summary>
        /// send command only to server
        /// </summary>
        /// <param name="cmd"></param>
        public void SendCommand(string cmd)
        {
            SendCommand(cmd, new byte[0]);
        }

        /// <summary>
        /// send command to server with string data
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="data"></param>
        public void SendCommand(string cmd, string data)
        {
            SendCommand(cmd, Encoding.Default.GetBytes(data));
        }

        /// <summary>
        /// send command to server with byte[] data
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="datas"></param>
        public void SendCommand(string cmd, byte[] datas)
        {
            byte[] cmds = Encoding.Default.GetBytes(cmd);
            if (cmds.Length != 4)
            {
                throw new ArgumentException();
            }

            int len = datas.Length;
            byte[] lens = Encoding.Default.GetBytes(len.ToString().PadLeft(9, '0'));

            len = 13 + len;
            byte[] bytes = new byte[len];

            cmds.CopyTo(bytes, 0);
            lens.CopyTo(bytes, 4);
            datas.CopyTo(bytes, 13);

            sender.Send(bytes);
        }

        /// <summary>
        /// 发送字符
        /// </summary>
        /// <param name="str"></param>
        public void SendString(string str)
        {
            SocketHelper.SendString(sender, str);
        }

        /// <summary>
        /// 发送Byte[]
        /// </summary>
        /// <param name="bytes"></param>
        public void SendBytes(byte[] bytes)
        {
            SocketHelper.SendBytes(sender, bytes);
        }

        /// <summary>
        /// 发送文件
        /// </summary>
        /// <param name="fullname"></param>
        public void SendFile(string fullname)
        {
            SocketHelper.SendFile(sender, fullname);
        }

        /// <summary>
        /// 接收Byte[]
        /// </summary>
        /// <returns></returns>
        public byte[] ReceiveBytes()
        {
            return SocketHelper.ReceiveBytes(sender);
        }

        /// <summary>
        /// 接收字符
        /// </summary>
        /// <returns></returns>
        public string ReceiveString()
        {
            return SocketHelper.ReceiveString(sender);
        }

        /// <summary>
        /// 接收文件，并保存到指定文件下
        /// </summary>
        /// <param name="fullname"></param>
        public void ReceiveFile(string fullname)
        {
            SocketHelper.ReceiveFile(sender, fullname);
        }

        #region IDisposable 成员

        void IDisposable.Dispose()
        {
            // Release the socket.
            if (sender != null)
            {
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
        }

        #endregion
    }
}
