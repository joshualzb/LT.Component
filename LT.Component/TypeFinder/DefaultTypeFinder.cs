using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace LT.Component.TypeFinder
{
    public class DefaultTypeFinder : ITypeFinder
    {
        private string m_AssemblySkipLoadingPattern = "^System|^mscorlib|^Microsoft";
        private string m_SearchDirectoryPath = "";
        private string m_SearchPattern = "*.dll";
        private List<Assembly> m_Assemblies = new List<Assembly>();

        public DefaultTypeFinder()
        {
        }


        /// <summary>
        /// 程序集
        /// </summary>
        public List<Assembly> AssembyNames
        {
            get
            {
                return m_Assemblies;
            }
        }

        /// <summary>跳过的程序集</summary>
        public string AssemblySkipLoadingPattern
        {
            get { return m_AssemblySkipLoadingPattern; }
            set { m_AssemblySkipLoadingPattern = value; }
        }

        /// <summary>
        /// 设置搜索程序集的文件目录
        /// </summary>
        public string SearchDirectoryPath
        {
            get { return m_SearchDirectoryPath; }
            set { m_SearchDirectoryPath = value; }
        }

        /// <summary>
        /// 设置搜索程序集的文件名范围
        /// </summary>
        public string SearchPattern
        {
            get { return m_SearchPattern; }
            set { m_SearchPattern = value; }
        }


        List<Assembly> ITypeFinder.GetFilteredAssemblyList()
        {
            foreach (string dllPath in Directory.GetFiles(m_SearchDirectoryPath, m_SearchPattern))
            {
                try
                {
                    Assembly a = Assembly.LoadFrom(dllPath);
                    if (!Matches(a.FullName))
                    {
                        m_Assemblies.Add(a);
                    }
                }
                catch (BadImageFormatException ex)
                {
                    Trace.TraceError(ex.ToString());
                }
            }

            return m_Assemblies;
        }

        /// <summary>
        /// 匹配程序集名称
        /// </summary>
        /// <param name="assemblyFullName">程序集名称</param>
        /// <returns>是否匹配成功</returns>
        public virtual bool Matches(string assemblyFullName)
        {
            return Matches(assemblyFullName, this.AssemblySkipLoadingPattern);
        }

        /// <summary>
        /// 匹配程序集名称
        /// </summary>
        /// <param name="assemblyFullName">程序集名称</param>
        /// <param name="pattern">匹配的正则条件</param>
        /// <returns>是否匹配成功</returns>
        protected virtual bool Matches(string assemblyFullName, string pattern)
        {
            return Regex.IsMatch(assemblyFullName, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }
    }
}
