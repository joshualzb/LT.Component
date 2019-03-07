using System.IO;

namespace LT.Component.Socket
{

    /// <summary>
    /// ICommandAction 通用类
    /// </summary>
    public interface ICommandAction
    {
        /// <summary>
        /// 执行操作接口
        /// </summary>
        /// <param name="stream">网络数据流</param>
        /// <param name="length">数据长度</param>
        /// <param name="ip">请求来源IP</param>
        void Excute(Stream stream, int length, string ip);
    }
}
