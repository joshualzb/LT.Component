using System;
using System.Collections;
using System.Data.Common;

namespace LT.Component.Entity
{
    public static class DbParamHelper
    {
        // Hashtable to store cached parameters
        private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// add parameter array to the cache
        /// </summary>
        /// <param name="cacheKey">Key to the parameter cache</param>
        /// <param name="cmdParms">an array of SqlParamters to be cached</param>
        public static void CacheParameters(string cacheKey, params DbParameter[] commandParameters)
        {
            parmCache[cacheKey] = commandParameters;
        }

        /// <summary>
        /// Retrieve cached parameters
        /// </summary>
        /// <param name="cacheKey">key used to lookup parameters</param>
        /// <returns>Cached SqlParamters array</returns>
        public static DbParameter[] GetCachedParameters(string cacheKey)
        {
            DbParameter[] cachedParms = (DbParameter[])parmCache[cacheKey];
            if (cachedParms == null)
            {
                return null;
            }

            DbParameter[] clonedParms = new DbParameter[cachedParms.Length];
            for (int i = 0, j = cachedParms.Length; i < j; i++)
            {
                clonedParms[i] = (DbParameter)((ICloneable)cachedParms[i]).Clone();
            }

            return clonedParms;
        }
    }
}
