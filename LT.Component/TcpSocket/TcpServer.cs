using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;

namespace LT.Component.TcpSocket
{
    /// <summary>
    /// state object for reading client data asynchronously
    /// </summary>
    public class StateObject
    {
        /// <summary>
        /// Client socket handler.
        /// </summary>
        public System.Net.Sockets.Socket Handler = null;

        /// <summary>
        /// Size of receive buffer of CMD.
        /// </summary>
        public const int Size = 13;

        /// <summary>
        /// Receive buffer of CMD.
        /// 数据格式：CMD + Len + Data
        /// CMD：命令，四位长度大写字母
        /// Len：数据长度，九位长度数字，不足前面补零
        /// Data：具体数据
        /// </summary>
        public byte[] Buffer = new byte[Size];
    }

    /// <summary>
    /// TCP Server
    /// </summary>
    public class TcpServer
    {
        /// <summary>
        /// Thread signal.
        /// </summary>
        ManualResetEvent allDone = new ManualResetEvent(false);

        /// <summary>
        /// listener
        /// </summary>
        System.Net.Sockets.Socket listener = null;

        /// <summary>
        /// Start Server
        /// </summary>
        /// <param name="port"></param>
        public void Start(int port)
        {
            // Establish the local endpoint for the socket.
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);

            // Create a TCP/IP socket.
            listener = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                listener.Bind(endPoint);
                listener.Listen(200);

                ThreadPool.QueueUserWorkItem(delegate(object state)
                {
                    while (true)
                    {
                        // Set the event to nonsignaled state.
                        allDone.Reset();

                        // Start an asynchronous socket to listen for connections.
                        listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                        // Wait until a connection is made before continuing.
                        allDone.WaitOne();
                    }
                });
            }
            catch (Exception ex)
            {
                Logs(ex.ToString());
            }
        }

        void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            allDone.Set();

            try
            {
                // Get the socket that handles the client request.
                System.Net.Sockets.Socket listener = (System.Net.Sockets.Socket)ar.AsyncState;
                System.Net.Sockets.Socket handler = listener.EndAccept(ar);

                // Create the state object.
                StateObject state = new StateObject();
                state.Handler = handler;
                handler.BeginReceive(new byte[0], 0, 0, 0, new AsyncCallback(ReadCallback), state);
            }
            catch (Exception ex)
            {
                Logs(ex.ToString());
            }
        }

        void ReadCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the handler socket
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                System.Net.Sockets.Socket handler = state.Handler;

                // Read data from the client socket. 
                handler.EndReceive(ar);

                //client ip
                var ip = ((IPEndPoint)handler.RemoteEndPoint).Address.ToString();

                //第一步：接收命令和数据
                string cmd = SocketHelper.ReceiveString(handler, 4).Trim();
                string data = null;
                if (cmd.Length == 4)
                {
                    var len = SocketHelper.ReceiveString(handler, 9);
                    if (Regex.IsMatch(len, @"^\d+$"))
                    {
                        data = SocketHelper.ReceiveString(handler, Convert.ToInt32(len));
                    }
                    else
                    {
                        data = len;
                    }
                }

                //第二步：使用EventHander解决执行
                if (CommandHandler.Instance.Actions.ContainsKey(cmd))
                {
                    CommandHandler.Instance.Actions[cmd].Excute(handler, data, ip);
                }

                //第三步：关闭
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception ex)
            {
                Logs(ex.ToString());
            }
        }

        void Logs(string message)
        {
            if (!EventLog.SourceExists("Components TcpServer"))
            {
                EventLog.CreateEventSource("Components TcpServer", "Application");
            }
            EventLog.WriteEntry("Components TcpServer", message, EventLogEntryType.Error);
        }
    }
}
