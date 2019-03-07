using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace LT.Component.HardwareRuntime
{

    /// <summary>
    /// InfoCollector 通用类
    /// </summary>
    public static class InfoCollector
    {
        private static PerformanceCounter CPU { get; set; }
        private static PerformanceCounter AllRAM { get; set; }
        private static PerformanceCounter UsedRAM { get; set; }

        private static PerformanceCounter NetworkReceived { get; set; }
        private static PerformanceCounter NetworkSend { get; set; }

        private static string NetworkInterfaceName { get; set; }
        private static long Speed { get; set; }

        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }
        [DllImport("kernel32")]
        static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

        static InfoCollector()
        {
            CPU = new PerformanceCounter("Processor", "% Processor Time", "_Total", ".");           

            Timer timer = new Timer(new TimerCallback(Tick), null, 0, 1000);
        }
        /// <summary>
        /// 获取当前的硬盘使用情况
        /// </summary>
        /// <returns></returns>
        public static Info Get()
        {
            Info info = new Info();
            info.UsedRateOfCPU = CPU.NextValue();
            MEMORYSTATUSEX MemInfo = new MEMORYSTATUSEX();
            MemInfo.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            GlobalMemoryStatusEx(ref MemInfo);
            info.TotalRAM = MemInfo.ullTotalPhys;
            info.UsedRAM = MemInfo.ullTotalPhys - MemInfo.ullAvailPhys;
            return info;
        }

        private static void Tick(object obj)
        {
            CPU.NextValue();
        }
    }
}
