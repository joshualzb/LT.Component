using System.Collections.Generic;
using System.Reflection;

namespace LT.Component.Entity
{
    public class ObjectCache
    {
        static Dictionary<string, string> stringCache;
        static Dictionary<string, PropertyInfo> propertyInfoCache;
        static Dictionary<string, List<string>> columnsCache;

        static object stringCacheLocker = new object();
        static object propertyInfoCacheLocker = new object();
        static object columnsCacheLocker = new object();

        static ObjectCache()
        {
            stringCache = new Dictionary<string, string>();
            propertyInfoCache = new Dictionary<string, PropertyInfo>();
            columnsCache = new Dictionary<string, List<string>>();
        }

        #region string
        public static string GetString(string key)
        {
            if (stringCache.ContainsKey(key))
            {
                return stringCache[key];
            }
            return null;
        }

        public static void SetString(string key, string value)
        {
            if (!stringCache.ContainsKey(key))
            {
                lock (stringCacheLocker)
                {
                    if (!stringCache.ContainsKey(key))
                    {
                        stringCache.Add(key, null);
                    }
                }
            }

            lock (stringCacheLocker)
            {
                stringCache[key] = value;
            }
        }
        #endregion

        #region PropertyInfo
        public static PropertyInfo GetPropertyInfo(string key)
        {
            if (propertyInfoCache.ContainsKey(key))
            {
                return propertyInfoCache[key];
            }
            return null;
        }

        public static void SetPropertyInfo(string key, PropertyInfo value)
        {
            if (!propertyInfoCache.ContainsKey(key))
            {
                lock (propertyInfoCacheLocker)
                {
                    if (!propertyInfoCache.ContainsKey(key))
                    {
                        propertyInfoCache.Add(key, null);
                    }
                }
            }

            lock (propertyInfoCacheLocker)
            {
                propertyInfoCache[key] = value;
            }
        }
        #endregion

        #region Columns
        public static List<string> GetColumns(string key)
        {
            if (columnsCache.ContainsKey(key))
            {
                return columnsCache[key];
            }
            return null;
        }

        public static void SetColumns(string key, List<string> value)
        {
            if (!columnsCache.ContainsKey(key))
            {
                lock (columnsCacheLocker)
                {
                    if (!columnsCache.ContainsKey(key))
                    {
                        columnsCache.Add(key, null);
                    }
                }
            }

            lock (columnsCacheLocker)
            {
                columnsCache[key] = value;
            }
        }
        #endregion
    }
}
