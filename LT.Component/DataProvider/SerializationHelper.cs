using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace LT.Component.DataProvider
{
    /// <summary>
    /// SerializationHelper 通用类
    /// </summary>
    public class SerializationHelper
    {
        // Fields
        private static Dictionary<int, XmlSerializer> serializer_dict = new Dictionary<int, XmlSerializer>();

        public static object DeSerialize(Type type, string s)
        {
            object obj2;
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            try
            {
                obj2 = GetSerializer(type).Deserialize(new MemoryStream(bytes));
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return obj2;
        }

        public static XmlSerializer GetSerializer(Type t)
        {
            int hashCode = t.GetHashCode();
            if (!serializer_dict.ContainsKey(hashCode))
            {
                serializer_dict.Add(hashCode, new XmlSerializer(t));
            }
            return serializer_dict[hashCode];
        }

        /// <summary>
        /// 从XML文件中加载对象
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fullfile"></param>
        /// <returns></returns>
        public static object Load(Type type, string fullfile)
        {
            FileStream stream = null;
            object obj2;

            try
            {
                stream = new FileStream(fullfile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                obj2 = new XmlSerializer(type).Deserialize(stream);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }

            return obj2;
        }

        /// <summary>
        /// 把对象保存到XML文件去
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fullfile"></param>
        /// <returns></returns>
        public static bool Save(object obj, string fullfile)
        {
            bool flag = false;
            FileStream stream = null;

            try
            {
                stream = new FileStream(fullfile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                new XmlSerializer(obj.GetType()).Serialize((Stream)stream, obj);
                flag = true;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }

            return flag;
        }

        public static string Serialize(object obj)
        {
            string str = "";
            XmlSerializer serializer = GetSerializer(obj.GetType());
            MemoryStream stream = new MemoryStream();
            XmlTextWriter writer = null;
            StreamReader reader = null;

            try
            {
                writer = new XmlTextWriter(stream, Encoding.UTF8);
                writer.Formatting = Formatting.Indented;
                serializer.Serialize((XmlWriter)writer, obj);
                stream.Seek(0L, SeekOrigin.Begin);
                reader = new StreamReader(stream);
                str = reader.ReadToEnd();
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
                if (reader != null)
                {
                    reader.Close();
                }
                stream.Close();
            }

            return str;
        }
    }
}
