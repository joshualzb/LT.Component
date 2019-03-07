using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace LT.Component.Socket
{
    /// <summary>
    /// TcpServer2 通用类
    /// </summary>
    public class TcpServer2
    {
        /// <summary>
        /// 挂起连接队列的最大数量
        /// </summary>
        public int Backlog = 50;

        /// <summary>
        /// 系统监听地址
        /// </summary>
        private IPEndPoint iep;

        /// <summary>
        /// SSL配置集
        /// </summary>
        private SslConfig sslConfig;

        /// <summary>
        /// 侦听器
        /// </summary>
        private TcpListener listener;

        /// <summary>
        /// Thread signal.
        /// </summary>
        static ManualResetEvent mre = new ManualResetEvent(false);

        /// <summary>
        /// 证书
        /// </summary>
        static X509Certificate serverCertificate = null;

        public TcpServer2(int listenPort, SslConfig sslConfig)
        {
            this.iep = new IPEndPoint(IPAddress.Any, listenPort);
            this.sslConfig = sslConfig;
        }

        public TcpServer2(string listenIP, int listenPort, SslConfig sslConfig)
        {
            this.iep = new IPEndPoint(IPAddress.Parse(listenIP), listenPort);
            this.sslConfig = sslConfig;
        }

        /// <summary>
        /// 启动Socket服务器
        /// </summary>
        public void Start()
        {
            //获取证书
            if (sslConfig != null)
            {
                switch (sslConfig.CertificateType)
                {
                    //从本地文件加载证书
                    case SslConfig.CertificateTypes.LocalFile:
                        serverCertificate = X509Certificate.CreateFromCertFile(sslConfig.CertName);
                        break;

                    //从系统储存中获取证书
                    case SslConfig.CertificateTypes.SystemStore:
                        X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                        store.Open(OpenFlags.ReadOnly);

                        X509Certificate2Collection cert = store.Certificates.Find(X509FindType.FindBySerialNumber, sslConfig.CertName, false);
                        serverCertificate = cert[0];
                        break;

                    //不符合要求（抛出错误）
                    default:
                        throw new Exception("It does not set GetCertType yet.");
                }
            }

            listener = new TcpListener(iep);
            listener.Start(Backlog);

            ThreadPool.QueueUserWorkItem(delegate(object state)
            {
                while (true)
                {
                    mre.Reset();

                    listener.BeginAcceptTcpClient(new AsyncCallback(SocketAsyncCallback), listener);

                    mre.WaitOne();
                }
            });
        }

        public void Stop()
        {
            if (listener != null)
            {
                listener.Stop();
            }
        }

        private void SocketAsyncCallback(IAsyncResult iar)
        {
            // Get the listener that handles the client request.
            TcpListener listener = (TcpListener)iar.AsyncState;

            // End the operation and display the received data on 
            // the console.
            System.Net.Sockets.TcpClient client = listener.EndAcceptTcpClient(iar);

            //获取网络流
            System.IO.Stream stream = client.GetStream();

            if (sslConfig != null && sslConfig.CertificateType != SslConfig.CertificateTypes.None)
            {
                // A client has connected. Create the 
                // SslStream using the client's network stream.
                SslStream sslStream = new SslStream(stream, false);

                // Authenticate the server but don't require the client to authenticate.
                sslStream.AuthenticateAsServer(serverCertificate, sslConfig.ClientCertificateRequired, SslProtocols.Tls, true);

                //set value to stream
                stream = sslStream;
            }

            //数据格式：CMD + Len + Data
            //CMD：命令，四位长度大写字母
            //Len：数据长度，九位长度数字，不足前面补零
            //Data：具体数据
            string ip = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

            //第一步：接收命令
            string cmd = SocketHelper.ReceiveString(stream, 4);

            //数据长度
            int len = SocketHelper.ReceiveInt32(stream, 9);

            //第二步：使用EventHander解决执行
            if (CommandHandler.Instance().Actions.ContainsKey(cmd))
            {
                try
                {
                    //执行实体
                    CommandHandler.Instance().Actions[cmd].Excute(stream, len, ip);
                }
                catch (Exception ex)
                {
                    SocketHelper.SendString(stream, ex.Message);
                }
            }
            else
            {
                SocketHelper.SendString(stream, "NULL");
            }

            // Signal the calling thread to continue.
            mre.Set();
        }
    }
}
