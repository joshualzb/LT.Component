using System.Web;
using LT.Component.HardwareRuntime;

namespace LT.Component.Web
{
    /// <summary>
    /// IP处理类
    /// </summary>
    public class IP
    {
        public static string GetRealIP()
        {
            string ip;
            HttpRequest request = HttpContext.Current.Request;
            if (request.ServerVariables["HTTP_VIA"] != null)
            {
                var HTTP_X_FORWARDED_FOR = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (HTTP_X_FORWARDED_FOR != null)
                {
                    ip = HTTP_X_FORWARDED_FOR.Split(',')[0].Trim();
                }
                else
                {
                    ip = request.UserHostAddress;
                }
            }
            else
            {
                ip = request.UserHostAddress;
            }

            //如果是本机，则换成真实绑定的IP
            if (ip == "127.0.0.1" || ip == "::1")
            {
                ip = WinManagement.GetLocalIP();
            }

            //返回结果
            return ip;
        }
    }
}
