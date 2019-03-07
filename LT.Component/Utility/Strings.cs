using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Security;

namespace LT.Component.Utility
{
    /// <summary>
    /// Strings 通用类
    /// </summary>
    public class Strings
    {
        #region MD5加密
        /// <summary>
        /// MD5加密字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string MD5(string str)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5");
        }
        #endregion

        #region 获取文件MD5值

        /// <summary>
        /// 计算文件的MD5校验
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetMD5Hash(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);

                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

                byte[] retVal = md5.ComputeHash(file);

                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }

        /// <summary>
        /// 计算文件的MD5校验
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetMD5Hash(Stream stream)
        {
            try
            {
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

                byte[] retVal = md5.ComputeHash(stream);

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }

        #endregion

        #region GuidID
        public static string GuidID()
        {
            return Guid.NewGuid().ToString("N");
        }
        #endregion

        #region 获取文本真实长度（一个中文当两个英文）
        /// <summary>
        /// 获取文本真实长度（一个中文当两个英文）
        /// </summary>
        public static int StrLength(string str)
        {
            if (str == null)
            {
                return 0;
            }
            else
            {
                return Encoding.Default.GetBytes(str).Length;
            }
        }

        /// <summary>
        /// 根据实际字符长度截长度（按英文长度）
        /// </summary>
        /// <param name="str">输入的字符</param>
        /// <param name="length">最大输出字符长</param>
        /// <returns></returns>
        public static string StrSubstring(string str, int length)
        {
            if (str == null || str.Length == 0)
            {
                return "";
            }

            int oLen = StrLength(str);
            if (oLen < length)
            {
                return str;
            }

            StringBuilder sb = new StringBuilder();
            int sLen = str.Length;
            int count = 0;
            string s = null;

            for (int i = 0; i < sLen; i++)
            {
                s = str.Substring(i, 1);
                count += StrLength(s);
                sb.Append(s);

                if (count >= length)
                {
                    break;
                }
            }

            return sb.ToString();
        }
        #endregion

        #region IsInString 检查某个字符是否在一串字符串中
        /// <summary>
        /// 检查某字符是否在一串字符串中
        /// Spliter是分隔的符号（Char类型）
        /// 如：字符串格式 3,5,7,78,13
        /// </summary>
        public static bool IsInString(string val, string strs, char spliter)
        {
            if (strs == null || val == null)
            {
                return false;
            }

            bool b = false;
            string[] arr = strs.Split(spliter);
            int count = arr.Length;
            for (int i = 0; i < count; i++)
            {
                if (arr[i].Equals(val))
                {
                    b = true;
                    break;
                }
            }
            return b;
        }

        public static bool IsInArray(string val, string[] arr)
        {
            if (arr == null || val == null)
            {
                return false;
            }

            bool b = false;
            int count = arr.Length;
            for (int i = 0; i < count; i++)
            {
                if (arr[i].Equals(val))
                {
                    b = true;
                    break;
                }
            }
            return b;
        }
        #endregion

        #region ConvertFromStringToList 把逗号分隔的字符串转成列表集合
        /// <summary>
        /// 把逗号分隔的字符串转成列表集合
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static List<string> ConvertFromStringToList(string val, char splitChart = ',')
        {
            List<string> list = new List<string>();

            if (val == null)
            {
                return list;
            }

            var vals = val.Split(splitChart);
            foreach (var item in vals)
            {
                list.Add(item);
            }

            return list;
        }
        #endregion

        #region 检查是否满足某种正则表达式
        /// <summary>
        /// 检查是否满足某种正则表达式
        /// </summary>
        public static bool IsRegexMatch(string str, string Express)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            return Regex.IsMatch(str, Express);
        }

        /// <summary>
        /// 检查目标字符串是否是数字形式
        /// </summary>
        public static bool IsNumber(string str)
        {
            return IsRegexMatch(str, @"^\d+$");
        }

        /// <summary>
        /// 检查字符串是否为数字集合
        /// 如 3,4,5,7
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNumberArray(string str)
        {
            return IsRegexMatch(str, @"^(\d+)(,\d+)*$");
        }

        /// <summary>
        /// 检查字符串是否为字符串集合（仅为数字和字母）
        /// 如 erv34,dfg23,45fdf,r_t5
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsStringArray(string str)
        {
            return IsRegexMatch(str, @"^([a-zA-Z0-9_]+)(,[a-zA-Z0-9_]+)*$");
        }

        /// <summary>
        /// 检查字符串是否为字符串集合（仅为数字和字母）
        /// 如 erv_34 或 84_jjf_88_ssf
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsKeyString(string str)
        {
            return IsRegexMatch(str, @"^([a-zA-Z0-9_\.\-:\s,]+)$");
        }

        /// <summary>
        /// 检查字符串是否为合法Email字符串集合
        /// 如 djjf@jsinf.cjkdi
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsEmail(string str)
        {
            return IsRegexMatch(str, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
        }
        #endregion

        #region 字符HTML编码
        public static string RemoveChars(string str)
        {
            if (str == null || str.Length == 0)
            {
                return "";
            }

            str = Regex.Replace(str, "=", "");
            str = Regex.Replace(str, "'", "");
            str = Regex.Replace(str, ":", "");
            str = Regex.Replace(str, ",", "");
            str = Regex.Replace(str, "\"", "");
            str = Regex.Replace(str, @"\(", "");
            str = Regex.Replace(str, @"\)", "");
            str = Regex.Replace(str, @"\.", "");
            str = Regex.Replace(str, @"\?", "");
            str = Regex.Replace(str, @"\*", "");
            str = Regex.Replace(str, @"\[", "");
            str = Regex.Replace(str, @"\]", "");
            str = Regex.Replace(str, "$", "");
            str = Regex.Replace(str, "^", "");
            str = Regex.Replace(str, @"\\", "");
            str = Regex.Replace(str, "/", "");
            return str;
        }

        public static string SqlEncode(string str)
        {
            if (str == null || str.Length == 0)
            {
                return "";
            }

            str = Regex.Replace(str, "=", "%3D");
            str = Regex.Replace(str, "'", "''");
            str = Regex.Replace(str, "<", "");
            str = Regex.Replace(str, ">", "");
            str = Regex.Replace(str, @"\(", "");
            str = Regex.Replace(str, @"\)", "");

            return str;
        }

        public static string HtmlAmpCode(string str)
        {
            if (str == null || str.Length == 0)
            {
                return "";
            }
            return Regex.Replace(str, "&", "&amp;");
        }

        public static string HtmlEncode(string str)
        {
            if (str == null || str.Length == 0)
            {
                return "";
            }

            str = Regex.Replace(str, "<", "&lt;");
            str = Regex.Replace(str, ">", "&gt;");
            str = Regex.Replace(str, "\"", "&quot;");
            str = Regex.Replace(str, " ", "&nbsp;");
            str = Regex.Replace(str, "\n", "<br />");
            str = str.Trim();

            return str;
        }

        public static string HtmlDecode(string str)
        {
            if (str == null || str.Length == 0)
            {
                return "";
            }

            str = Regex.Replace(str, "&lt;", "<");
            str = Regex.Replace(str, "&gt;", ">");
            str = Regex.Replace(str, "&quot;", "\"");
            str = Regex.Replace(str, "&nbsp;", " ");
            str = Regex.Replace(str, "<br />", "\n");

            return str;
        }
        #endregion

        #region 把字符串转换成INT

        /// <summary>
        /// 把字符串转换成Byte
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static byte ConvertToByte(string val)
        {
            return Convert.ToByte(ConvertToInt(val, 0));
        }

        /// <summary>
        /// 把字符串转换成INT
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int ConvertToInt(string val)
        {
            return ConvertToInt(val, 0);
        }

        /// <summary>
        /// 把字符串转换成INT
        /// </summary>
        /// <param name="val"></param>
        /// <param name="dft"></param>
        /// <returns></returns>
        public static int ConvertToInt(string val, int dft)
        {
            if (string.IsNullOrEmpty(val))
            {
                return dft;
            }
            else if (!Regex.IsMatch(val, @"^\d+$"))
            {
                return dft;
            }
            return int.Parse(val);
        }

        /// <summary>
        /// 把字符串转成Long
        /// </summary>
        /// <param name="val"></param>
        /// <param name="dft"></param>
        /// <returns></returns>
        public static long ConvertToLong(string val, long dft = 0)
        {
            if (string.IsNullOrEmpty(val))
            {
                return dft;
            }
            else if (!Regex.IsMatch(val, @"^\d+$"))
            {
                return dft;
            }
            return long.Parse(val);
        }

        #endregion

        #region 把字符串转换成Decimal
        /// <summary>
        /// 把字符串转换成Decimal
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static decimal ConvertToDecimal(string val)
        {
            return ConvertToDecimal(val, 0M);
        }

        /// <summary>
        /// 把字符串转换成Decimal
        /// </summary>
        /// <param name="val"></param>
        /// <param name="dft"></param>
        /// <returns></returns>
        public static decimal ConvertToDecimal(string val, decimal dft)
        {
            if (string.IsNullOrEmpty(val))
            {
                return dft;
            }

            val = Regex.Replace(val, ",| ", "");

            return decimal.Parse(val);
        }
        #endregion

        #region 把字符串转换成日期类型
        /// <summary>
        /// 把字符串转换成日期类型
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(string datetime)
        {
            DateTime dt = new DateTime(1900, 1, 1);
            if (string.IsNullOrEmpty(datetime))
            {
                return dt;
            }

            datetime = datetime.Trim();
            if (Regex.IsMatch(datetime.Trim(), @"^\d{4}-\d{1,2}-\d{1,2}$"))
            {
                string[] vals = datetime.Split('-');
                dt = new DateTime(int.Parse(vals[0]), int.Parse(vals[1]), int.Parse(vals[2]));
            }
            else
            {
                DateTime.TryParse(datetime, out dt);
            }

            return dt;
        }
        #endregion

        #region 把日期转换成字符串
        /// <summary>
        /// 把日期转换成字符串
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ConvertToString(DateTime datetime, string format = "yyyy-MM-dd")
        {
            if (datetime.Year <= 1900)
            {
                return "";
            }
            return datetime.ToString(format);
        }
        #endregion

        #region 把字符串转换成日期类型
        /// <summary>
        /// 把空间大小（数据大小）转化成 Byte，KB，MB，TB 方式显示
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string DataSizeToString(decimal size)
        {
            string str = "";
            if (size < 1024)
            {
                str = size + " Byte";
            }
            else if (size < 1048576)
            {
                str = (size / 1024M).ToString("F3") + " KB";
            }
            else if (size < 1073741824)
            {
                str = (size / 1048576M).ToString("F3") + " MB";
            }
            else
            {
                str = (size / 1073741824M).ToString("F3") + " TB";
            }
            return str;
        }
        #endregion

        #region 半角转全角的函数(SBC case)
        /// <summary>
        /// 半角转全角的函数(SBC case) 
        /// 全角空格为12288，半角空格为32 
        /// 其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248 
        /// </summary> 
        /// <param name="input">任意字符串</param> 
        /// <returns>全角字符串</returns> 
        public static string ToDBC(string input)
        {
            //半角转全角： 
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new string(c);
        }
        #endregion

        #region  全角转半角的函数(DBC case)
        /// <summary> 
        /// 全角转半角的函数(DBC case) 
        /// 全角空格为12288，半角空格为32 
        /// 其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248 
        /// </summary> 
        /// <param name="input">任意字符串</param> 
        /// <returns>半角字符串</returns>
        public static string ToSBC(string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }
        #endregion

        #region  将 Stream 转成 byte[]
        /// <summary> 
        /// 将 Stream 转成 byte[] 
        /// </summary> 
        public static byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始 
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }
        #endregion

        #region 将 byte[] 转成 Stream
        /// <summary> 
        /// 将 byte[] 转成 Stream 
        /// </summary> 
        public static Stream BytesToStream(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            return stream;
        }
        #endregion

        #region 将bool类型转换 符号显示

        /// <summary>
        /// 返回True与False状态
        /// </summary>
        /// <param name="trueOrFalse"></param>
        /// <returns></returns>
        public static string GetBooleanState(object trueOrFalse)
        {
            if (trueOrFalse == null)
            {
                return "×";
            }
            if (Convert.ToBoolean(trueOrFalse))
            {
                return "√";
            }
            return "×";
        }

        #endregion

        #region 字符串操作

        /// <summary>
        /// 字符串如果操过指定长度则将超出的部分用指定字符串代替
        /// </summary>
        /// <param name="p_SrcString">要检查的字符串</param>
        /// <param name="p_Length">指定长度</param>
        /// <param name="p_TailString">用于替换的字符串</param>
        /// <returns>截取后的字符串</returns>
        public static string GetSubString(string p_SrcString, int p_Length, string p_TailString)
        {
            return GetSubString(p_SrcString, 0, p_Length, p_TailString);
        }

        /// <summary>
        /// 获取Unicode子字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len"></param>
        /// <param name="p_TailString"></param>
        /// <returns></returns>
        public static string GetUnicodeSubString(string str, int len, string p_TailString)
        {
            str = str.TrimEnd();
            string result = string.Empty;// 最终返回的结果
            int byteLen = System.Text.Encoding.Default.GetByteCount(str);// 单字节字符长度
            int charLen = str.Length;// 把字符平等对待时的字符串长度
            int byteCount = 0;// 记录读取进度
            int pos = 0;// 记录截取位置
            if (byteLen > len)
            {
                for (int i = 0; i < charLen; i++)
                {
                    if (Convert.ToInt32(str.ToCharArray()[i]) > 255)// 按中文字符计算加2
                        byteCount += 2;
                    else// 按英文字符计算加1
                        byteCount += 1;
                    if (byteCount > len)// 超出时只记下上一个有效位置
                    {
                        pos = i;
                        break;
                    }
                    else if (byteCount == len)// 记下当前位置
                    {
                        pos = i + 1;
                        break;
                    }
                }

                if (pos >= 0)
                    result = str.Substring(0, pos) + p_TailString;
            }
            else
                result = str;

            return result;
        }

        /// <summary>
        /// 取指定长度的字符串
        /// </summary>
        /// <param name="p_SrcString">要检查的字符串</param>
        /// <param name="p_StartIndex">起始位置</param>
        /// <param name="p_Length">指定长度</param>
        /// <param name="p_TailString">用于替换的字符串</param>
        /// <returns>截取后的字符串</returns>
        public static string GetSubString(string p_SrcString, int p_StartIndex, int p_Length, string p_TailString)
        {
            string myResult = p_SrcString;

            Byte[] bComments = Encoding.UTF8.GetBytes(p_SrcString);
            foreach (char c in Encoding.UTF8.GetChars(bComments))
            {    //当是日文或韩文时(注:中文的范围:\u4e00 - \u9fa5, 日文在\u0800 - \u4e00, 韩文为\xAC00-\xD7A3)
                if ((c > '\u0800' && c < '\u4e00') || (c > '\xAC00' && c < '\xD7A3'))
                {
                    //if (System.Text.RegularExpressions.Regex.IsMatch(p_SrcString, "[\u0800-\u4e00]+") || System.Text.RegularExpressions.Regex.IsMatch(p_SrcString, "[\xAC00-\xD7A3]+"))
                    //当截取的起始位置超出字段串长度时
                    if (p_StartIndex >= p_SrcString.Length)
                        return "";
                    else
                        return p_SrcString.Substring(p_StartIndex,
                                                       ((p_Length + p_StartIndex) > p_SrcString.Length) ? (p_SrcString.Length - p_StartIndex) : p_Length);
                }
            }

            if (p_Length >= 0)
            {
                byte[] bsSrcString = Encoding.Default.GetBytes(p_SrcString);

                //当字符串长度大于起始位置
                if (bsSrcString.Length > p_StartIndex)
                {
                    int p_EndIndex = bsSrcString.Length;

                    //当要截取的长度在字符串的有效长度范围内
                    if (bsSrcString.Length > (p_StartIndex + p_Length))
                    {
                        p_EndIndex = p_Length + p_StartIndex;
                    }
                    else
                    {   //当不在有效范围内时,只取到字符串的结尾

                        p_Length = bsSrcString.Length - p_StartIndex;
                        p_TailString = "";
                    }

                    int nRealLength = p_Length;
                    int[] anResultFlag = new int[p_Length];
                    byte[] bsResult = null;

                    int nFlag = 0;
                    for (int i = p_StartIndex; i < p_EndIndex; i++)
                    {
                        if (bsSrcString[i] > 127)
                        {
                            nFlag++;
                            if (nFlag == 3)
                                nFlag = 1;
                        }
                        else
                            nFlag = 0;

                        anResultFlag[i] = nFlag;
                    }

                    if ((bsSrcString[p_EndIndex - 1] > 127) && (anResultFlag[p_Length - 1] == 1))
                        nRealLength = p_Length + 1;

                    bsResult = new byte[nRealLength];

                    Array.Copy(bsSrcString, p_StartIndex, bsResult, 0, nRealLength);

                    myResult = Encoding.Default.GetString(bsResult);
                    myResult = myResult + p_TailString;
                }
            }

            return myResult;
        }

        /// <summary>
        /// 获取整型数组
        /// </summary>
        /// <param name="str">用","分割的字符串</param>
        /// <returns>int[]</returns>
        public static int[] ConvertToIntArray(string str)
        {
            List<int> list = new List<int>();

            var array = str.Split(',');

            foreach (string item in array)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    list.Add(ConvertToInt(item));
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// 格式化时间并过滤数据库默认时间
        /// </summary>
        /// <param name="date"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string FilterDefaultTime(DateTime date, string format = "yyyy-MM-dd HH:mm:ss fff")
        {
            if (date.Year <= 1900)
            {
                return string.Empty;
            }
            return date.ToString(format);
        }

        /// <summary>
        /// 数据库默认时间（1900-01-01 00:00:00）
        /// </summary>
        public static DateTime DefaultDateTime { get { return new DateTime(1900, 01, 01); } }

        #endregion

        #region 中文周显示

        /// <summary>
        /// 获取中文星期文字
        /// </summary>
        /// <param name="week"></param>
        /// <returns>周一、周二、周三、周四、周五、周六、周日</returns>
        public static string GetCHSDayOfWeek(DayOfWeek week)
        {
            switch (week)
            {
                case DayOfWeek.Friday:
                    return "周五";
                case DayOfWeek.Monday:
                    return "周一";
                case DayOfWeek.Saturday:
                    return "周六";
                case DayOfWeek.Sunday:
                    return "周日";
                case DayOfWeek.Thursday:
                    return "周四";
                case DayOfWeek.Tuesday:
                    return "周二";
                case DayOfWeek.Wednesday:
                    return "周三";
            }
            return string.Empty;
        }

        #endregion

        #region JSON转换方法
        /// <summary>
        /// 将对象转换为JSON对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string SerializeToJSON<T>(T t)
        {
            try
            {
                Newtonsoft.Json.Converters.IsoDateTimeConverter timeConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter();

                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy'/'MM'/'dd' 'HH':'mm':'ss";

                //JsonConvert.SerializeObject(quoOdrs, Formatting.Indented, timeConverter);
                return JsonConvert.SerializeObject(t, timeConverter);//t 是对象集合
            }
            catch
            {
                return JsonConvert.SerializeObject(t, new DataTableConverter());
            }
        }

        /// <summary>
        /// 将JSON格式的字符串转换成指定对象
        /// </summary>
        public static T DeserializeFromJSON<T>(string input)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(input);
            }
            catch
            {
                return default(T);
            }
        }
        #endregion
    }
}
