using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace LT.Component.Utility
{
    /// <summary>
    /// Serialization 通用类
    /// </summary>
    public class Serialization
    {
        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static byte[] FromModelToByte<T>(T t)
        {
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, t);
                byte[] byt = new byte[stream.Length];
                return stream.ToArray();
            }
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static MemoryStream FromModelToStream<T>(T t)
        {
            IFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, t);
            return stream;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T FromStreamToModel<T>(Stream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            stream.Position = 0;

            return (T)formatter.Deserialize(stream);
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static MemoryStream FromStringsToStream<T>(T t)
        {
            IFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, t);
            return stream;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T FromStreamToStrings<T>(Stream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            stream.Position = 0;
            return (T)formatter.Deserialize(stream);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T FromByteToModel<T>(byte[] data)
        {
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new MemoryStream(data, 0, data.Length))
            {
                return (T)formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// 把字节流转成字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string FromByteToString(byte[] data)
        {
            return Convert.ToBase64String(data, 0, data.Length);
        }

        /// <summary>
        /// 根据对象序列化成为字符串
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="t">需要序列化的对象</param>
        /// <returns>返回的字符串</returns>
        public static string FromModelToString<T>(T t)
        {
            byte[] data = FromModelToByte<T>(t);
            return Convert.ToBase64String(data, 0, data.Length);
        }

        /// <summary>
        /// 根据对象序列化成为HTML符号
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="t">需要序列化的对象</param>
        /// <returns>返回的HTML符号</returns>
        public static string FromModelToHtml<T>(T t) where T : class
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            Type type = t.GetType();

            System.Reflection.PropertyInfo[] pis = type.GetProperties();
            foreach (System.Reflection.PropertyInfo pi in pis)
            {
                sb.Append("<div><b>");
                sb.Append(pi.Name);
                sb.Append("：</b>");
                sb.Append(pi.GetValue(t, null));
                sb.Append("<div>");
            }

            return sb.ToString();
        }

        /// <summary>
        /// 根据字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T FromStringToModel<T>(string str)
        {
            byte[] data = Convert.FromBase64String(str);
            return FromByteToModel<T>(data);
        }
    }
}
