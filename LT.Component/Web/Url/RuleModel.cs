using System;

namespace LT.Component.Web.Url
{

    /// <summary>
    /// RuleModel 通用类
    /// </summary>
    public class RuleModel
    {
        private string _from;
        private string _to;
        private string _op;

        public RuleModel(string from, string to, string op)
        {
            _from = from;
            _to = to;
            _op = op;
        }
        
        public string From
        {
            get { return _from; }
        }

        public string To
        {
            get { return _to; }
        }

        public string OP
        {
            get { return _op; }
        }
    }
}
