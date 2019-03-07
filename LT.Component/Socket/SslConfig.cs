/*******************************************************************************************
* 00: ---------------------------------------------------------------
* 01: LT.Component.Socket.SslConfig
* 02: SslConfig 通用类
* 03: 11/27/2009 22
* 04: 2010-08-26 10 03 59
* 05: 20100826-10
* 06: 
*******************************************************************************************/

/* ==============================================================
 * 
 * filename : LT.Component.Socket.SslConfig
 * created by : LANXE-F0B6942D1
 * created at : 11/27/2009 22:07:31
 * description : 
 * 
 * ==============================================================
*/
namespace LT.Component.Socket
{

    /// <summary>
    /// SslConfig 通用类
    /// </summary>
    public class SslConfig
    {
        public enum CertificateTypes
        {
            /// <summary>
            /// 不使用证书
            /// </summary>
            None = 0,

            /// <summary>
            /// 从本地文件获取
            /// </summary>
            LocalFile = 1,

            /// <summary>
            /// 从系统储存中获取
            /// </summary>
            SystemStore = 2
        }

        /// <summary>
        /// 设置和获取使用什么方式获取证书
        /// </summary>
        public CertificateTypes CertificateType
        {
            get
            {
                return certificateType;
            }
            set
            {
                certificateType = value;
            }
        }
        private CertificateTypes certificateType;

        /// <summary>
        /// 设置和获取证书颁发给的名称
        /// </summary>
        public string TargetHost
        {
            get
            {
                return targetHost;
            }
            set
            {
                targetHost = value;
            }
        }
        private string targetHost;

        /// <summary>
        /// 设置和获取证书的名称
        /// 当为本地文件时，为完全路径；当为系统储存时，为序列号
        /// </summary>
        public string CertName
        {
            get
            {
                return certName;
            }
            set
            {
                certName = value;
            }
        }
        private string certName;

        /// <summary>
        /// 设置和获取客户端是否必须为身份验证提供证书
        /// </summary>
        public bool ClientCertificateRequired
        {
            get
            {
                return clientCertificateRequired;
            }
            set
            {
                clientCertificateRequired = value;
            }
        }
        private bool clientCertificateRequired;
    }
}
