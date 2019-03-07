using System.Collections;
using System.Text.RegularExpressions;

namespace LT.Component.Utility
{

    /// <summary>
    /// Regexs 通用类
    /// </summary>
    public class Regexs
    {
        /// <summary>
        /// 网页地址正则
        /// </summary>
        public static Regex UrlReg = new Regex(@"^(?<PROTOCOL>\w+)://(?<SERVER>[^/^:]+)(:(?<PORT>\d+))?(?<PATH>(/[^/]+)*)/(?<FILE>.*?)$", RegexOptions.IgnoreCase);

        /// <summary>
        /// 网页文件页头内容正则
        /// </summary>
        public static Regex ContentTypeImageRegex = new Regex(@"image/(\w+)", RegexOptions.IgnoreCase);
        public static Regex ContentTypeTextRegex = new Regex(@"text/(\w+)", RegexOptions.IgnoreCase);
        public static Regex ContentTypeCharsetRegex = new Regex(@"charset=(\w+)", RegexOptions.IgnoreCase);

        /// <summary>
        /// 图片正则
        /// </summary>
        private const string imagePattern = @"<img[\s]+[^>]*?((src*?[\s]?=(?<SRC>.[^\\""\\']*?))|(src*?[\s]?=[\s\\""\\'](?<SRC>.*?)[\s\\""\\']+.*?))(\s|>)";
        public static Regex ImageRegex = new Regex(imagePattern, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);

        /// <summary>
        /// 通用获取内容正则
        /// </summary>
        public static string MatchGroupValue(string pattern, string groupName, string input)
        {
            string groupValue = "";
            Regex reg = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);

            Match m = reg.Match(input);
            if (m.Success)
            {
                groupValue = m.Groups[groupName].Value;
            }

            return groupValue;
        }

        /// <summary>
        /// 获取指定内容所有包括的图片
        /// </summary>
        public static ArrayList ContainImages(string input)
        {
            string src = null;
            ArrayList al = new ArrayList();
            MatchCollection mc = ImageRegex.Matches(input);

            foreach (Match m in mc)
            {
                src = m.Groups["SRC"].Value;
                if (src != null && src.Length > 0 && al.IndexOf(src) == -1)
                {
                    al.Add(src);
                }
            }

            return al;
        }
    }
}
