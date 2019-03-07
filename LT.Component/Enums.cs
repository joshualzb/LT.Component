using System;

namespace LT.Component
{
    /// <summary>
    /// Enums 通用类
    /// </summary>
    public class Enums
    {
        /// <summary>
        /// 真假值输出选择
        /// </summary>
        public enum BooleanStates
        {
            /// <summary>
            /// False
            /// </summary>
            False = -1,

            /// <summary>
            /// True OR Flase
            /// </summary>
            TrueFlase = 0,

            /// <summary>
            /// True
            /// </summary>
            True = 1
        }

        /// <summary>
        /// 后台对话框类型
        /// </summary>
        public enum ScriptMessageType
        {
            /// <summary>
            /// 默认
            /// </summary>
            defalut,

            /// <summary>
            /// 警告
            /// </summary>
            warn,

            /// <summary>
            /// 成功提示
            /// </summary>
            success,

            /// <summary>
            /// 错误警告
            /// </summary>
            error,

            /// <summary>
            /// 询问
            /// </summary>
            question,
        }
    }
}
