using System.Collections.Generic;
using System.Management;

namespace LT.Component.HardwareRuntime
{
    /// <summary>
    /// 系统硬盘信息类
    /// </summary>
    public class WinManagement
    {
        /// <summary>
        /// 获取本机的IP
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIP()
        {
            string ip = "127.0.0.1";

            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if ((bool)mo["IPEnabled"] == true)
                {
                    System.Array ar = (System.Array)(mo.Properties["IpAddress"].Value);
                    ip = ar.GetValue(0).ToString();
                    if (ip != "0.0.0.0")
                    {
                        break;
                    }
                }
            }
            moc = null;
            mc = null;

            return ip;
        }

        /// <summary>
        /// 获取本机的IP集合
        /// </summary>
        /// <returns></returns>
        public static List<string> GetLocalIPs()
        {
            List<string> ips = new List<string>();
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if ((bool)mo["IPEnabled"] == true)
                {
                    System.Array ar = (System.Array)(mo.Properties["IpAddress"].Value);
                    ips.Add(ar.GetValue(0).ToString());
                }
            }
            return ips;
        }
    }
}
