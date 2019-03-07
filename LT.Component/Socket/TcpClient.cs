using System;
using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace LT.Component.Socket
{
    /// <summary>
    /// TcpClient 通用类
    /// </summary>
    public class TcpClient : IDisposable
    {
        private System.Net.Sockets.TcpClient tcpClient;
        private System.IO.Stream stream;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listenIP">所连接服务器的IP地址</param>
        /// <param name="listenPort">所连接服务器的端口</param>
        /// <param name="sslConfig">SSL的配置集，如果没有用使用SSL则设为null</param>
        public TcpClient(string listenIP, int listenPort, SslConfig sslConfig)
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(listenIP), listenPort);

            tcpClient = new System.Net.Sockets.TcpClient();
            tcpClient.Connect(iep);

            //获取网络流
            stream = tcpClient.GetStream();

            //处理SSL加密方式
            if (sslConfig != null)
            {
                //证书集合
                X509CertificateCollection cert = null;

                //Create a SSL stream that will close the client's stream.
                SslStream sslStream = new SslStream(stream, false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);

                switch (sslConfig.CertificateType)
                {
                    //不使用证书
                    case SslConfig.CertificateTypes.None:
                        break;

                    //从本地文件加载证书
                    case SslConfig.CertificateTypes.LocalFile:
                        X509Certificate cer = X509Certificate2.CreateFromCertFile(sslConfig.CertName);
                        cert = new X509CertificateCollection();
                        cert.Add(cer);
                        break;

                    //从系统储存中获取证书
                    case SslConfig.CertificateTypes.SystemStore:
                        X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                        store.Open(OpenFlags.ReadOnly);

                        cert = store.Certificates.Find(X509FindType.FindBySerialNumber, sslConfig.CertName, false);
                        break;
                }

                // The server name must match the name on the server certificate.
                if (cert == null || sslConfig.ClientCertificateRequired == false)
                {
                    sslStream.AuthenticateAsClient(sslConfig.TargetHost);
                }
                else
                {
                    sslStream.AuthenticateAsClient(sslConfig.TargetHost, cert, SslProtocols.Tls, false);
                }

                //转入到Stream下
                stream = sslStream;
            }
        }

        public void SendCommand(string cmd)
        {
            byte[] cmds = Encoding.Default.GetBytes(cmd);
            if (cmds.Length != 4)
            {
                throw new ArgumentException();
            }

            byte[] lens = Encoding.Default.GetBytes("000000000");
            byte[] bytes = new byte[13];

            cmds.CopyTo(bytes, 0);
            lens.CopyTo(bytes, 4);

            SendBytes(bytes, 13);
        }

        public void SendCommand(string cmd, string data)
        {
            byte[] cmds = Encoding.Default.GetBytes(cmd);
            if (cmds.Length != 4)
            {
                throw new ArgumentException();
            }

            byte[] datas = Encoding.Default.GetBytes(data);

            int len = datas.Length;
            byte[] lens = Encoding.Default.GetBytes(len.ToString().PadLeft(9, '0'));

            len = 13 + len;
            byte[] bytes = new byte[len];

            cmds.CopyTo(bytes, 0);
            lens.CopyTo(bytes, 4);
            datas.CopyTo(bytes, 13);

            SendBytes(bytes, len);
        }

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

            SendBytes(bytes, len);
        }

        private void SendBytes(byte[] bytes, int length)
        {
            stream.Write(bytes, 0, length);
        }

        public void SendString(string str)
        {
            SocketHelper.SendString(stream, str);
        }

        public void SendBytes(byte[] bytes)
        {
            SocketHelper.SendBytes(stream, bytes);
        }

        public void SendFile(string fullname)
        {
            SocketHelper.SendFile(stream, fullname);
        }

        public byte[] ReceiveBytes()
        {
            return SocketHelper.ReceiveBytes(stream);
        }

        public string ReceiveString()
        {
            return SocketHelper.ReceiveString(stream);
        }

        public void ReceiveFile(string fullname)
        {
            SocketHelper.ReceiveFile(stream, fullname);
        }

        /// <summary>
        /// The following method is invoked by the RemoteCertificateValidationDelegate.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }

        #region IDisposable 成员

        void IDisposable.Dispose()
        {
            stream.Close();
            tcpClient.Close();
        }

        #endregion
    }
}
