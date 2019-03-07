using System;

namespace LT.Component.HardwareRuntime
{
    /// <summary>
    /// 硬件使用信息
    /// </summary>
    public struct Info
    {
        /// <summary>
        /// CPU使用率(百分比)
        /// </summary>
        public float UsedRateOfCPU;

        /// <summary>
        /// 总共的内存量(字节)
        /// </summary>
        public ulong TotalRAM;

        /// <summary>
        /// 已使用的内存量(字节)
        /// </summary>
        public ulong UsedRAM;

        /// <summary>
        /// 占用的宽带量(百分比)
        /// </summary>
        public float UsedBandWidth;
    }
}
