using System.Collections.Generic;

namespace LT.Component.Socket
{

    /// <summary>
    /// Socket服务器监听命令注册
    /// </summary>
    public class CommandHandler
    {
        private Dictionary<string, ICommandAction> actions;
        private static readonly CommandHandler handler = null;

        static CommandHandler()
        {
            handler = new CommandHandler();
        }

        public static CommandHandler Instance()
        {
            return handler;
        }

        private CommandHandler()
        {
            actions = new Dictionary<string, ICommandAction>();
        }

        public Dictionary<string, ICommandAction> Actions
        {
            get
            {
                return actions;
            }
        }

        /// <summary>
        /// 添加命令执行实体
        /// </summary>
        /// <param name="cmd">命令值，必须四位</param>
        /// <param name="action"></param>
        public void Add(string cmd, ICommandAction action)
        {
            if (cmd.Length > 0)
            {
                if (!actions.ContainsKey(cmd))
                {
                    actions.Add(cmd, action);
                }
            }
        }
    }
}
