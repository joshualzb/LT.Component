using System.Net.Sockets;

namespace LT.Component.TcpSocket
{
    /// <summary>
    /// ICommandAction 通用类
    /// </summary>
    public interface ICommandAction
    {
        /// <summary>
        /// 执行操作接口
        /// </summary>
        /// <param name="handler">Socket处理器</param>
        /// <param name="data">命令数据</param>
        /// <param name="ip">请求来源IP</param>
        void Excute(System.Net.Sockets.Socket handler, string data, string ip);
    }
}
