using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace LT.Component.Utility
{
	/// <summary>
    /// TransformHtml 通用类
	/// </summary>
    public class TransformHtml : IDisposable
	{
		private MatchCollection m_mc = null;
		private StringBuilder m_inText = new StringBuilder();
		private StringBuilder m_outHtml = new StringBuilder();
		private ArrayList m_matchNames = new ArrayList();
		private ArrayList m_matchMethods = new ArrayList();
		private ArrayList m_teplText = new ArrayList();
		private ArrayList m_teplValue = new ArrayList();
		private int m_matches = 0;
		private int m_teplIndex = 0;
		private string m_pattern = null;

		/// <summary>
        /// 转换单个SqlDataRader，即单个数据集
		/// </summary>
		/// <param name="idr"></param>
        /// <param name="text">模板文本内容</param>
		/// <returns></returns>
		public string ByDataReader(IDataReader idr, string text)
		{
			GetTemplate(text);

			m_teplIndex = 0;
			SetTemplate(idr);
			idr.Close();

			return m_outHtml.ToString();
		}

		/// <summary>
        /// 转换多个SqlDataRader 即多个数据集
		/// </summary>
		/// <param name="idr"></param>
        /// <param name="text">模板文本内容</param>
		/// <returns></returns>
		public string ByDataReaders(IDataReader idr, string text)
		{
			GetTemplate(text);

			m_teplIndex = 0;
			SetTemplate(idr);

			while(idr.NextResult())
			{
				m_teplIndex++;
				SetTemplate(idr);
			}
			idr.Close();

			return m_outHtml.ToString();
		}

		/// <summary>
        /// 转换单个DataTable
		/// </summary>
		/// <param name="dt"></param>
        /// <param name="text">模板文本内容</param>
		/// <returns></returns>
        public string ByDataTable(DataTable dt, string text)
        {
            GetTemplate(text);

            m_teplIndex = 0;
            SetTemplate(dt);

            return m_outHtml.ToString();
        }

		/// <summary>
        /// 转换多个DataTable
		/// </summary>
		/// <param name="dts"></param>
        /// <param name="text">模板文本内容</param>
		/// <returns></returns>
		public string ByDataTables(DataTable[] dts, string text)
		{
			GetTemplate(text);

			m_teplIndex = 0;
			for(int n = 0; n < dts.Length; n++)
			{
				SetTemplate(dts[n]);
				m_teplIndex++;
			}

			return m_outHtml.ToString();
		}
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idr">两个DataReader的数据集</param>
        /// <param name="keyName"></param>
        /// <param name="text">模板文本内容</param>
        /// <returns></returns>
		public string ByRelateDataReaders(IDataReader idr, string keyName, string text)
		{
			DataTable parent = new DataTable();
			DataTable child = new DataTable();

			// first result table
			SetDataTable(idr, ref parent);

			// set next result
			idr.NextResult();

			// second result table
			SetDataTable(idr, ref child);

			// close datareader
			idr.Close();

			return ByRelateDataTables(parent, child, keyName, text);
		}

        /// <summary>
        /// 通父子关系的内存表替换成HTML
        /// </summary>
        /// <param name="fatherTable"></param>
        /// <param name="childTable"></param>
        /// <param name="keyName">两个Table所使用关联的字段</param>
        /// <param name="text">模板文本内容</param>
        /// <returns></returns>
		public string ByRelateDataTables(DataTable fatherTable, DataTable childTable, string keyName, string text)
		{
			int i = 0;
			int j = 0;
            int drs_length = 0;
            int drc_count = 0;

			DataRow[] drs = null;
			DataRowCollection pDrc = fatherTable.Rows;
			DataTable fatherRow = fatherTable.Clone();
			DataTable childRow = childTable.Clone();
			StringBuilder html = new StringBuilder();
			string query1 = string.Concat("[", keyName ,"]='");
			string query2 = null;

            drc_count = pDrc.Count;

            for (i = 0; i < drc_count; i++)
			{
				GetTemplate(text);

				m_teplIndex = 0;
				fatherRow.ImportRow(pDrc[i]);
				SetTemplate(fatherRow);
				fatherRow.Rows.Clear();

				//Child DataTable
				query2 = string.Concat(query1, pDrc[i][keyName], "'");
                drs = childTable.Select(query2);
                drs_length = drs.Length;
                for (j = 0; j < drs_length; j++)
				{
                    childRow.ImportRow(drs[j]);
				}

				m_teplIndex = 1;
				SetTemplate(childRow);
				childRow.Rows.Clear();

				html.Append(m_outHtml);			
			}

			return html.ToString();
		}

        /// <summary>
        /// 以每一行记录作为一个<#List>
        /// 即第几条记录谳用第几个循环体
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="text">模板文本内容</param>
        /// <returns></returns>
		public string ByListRecordDataTable(DataTable dt, string text)
		{
			GetTemplate(text);

			int i = 0;
			int j = 0;
            int drc_count = 0;
            int tpt_count = 0;
			string n_value = null;
			string m_value = null;
			string c_value = null;
			string template = null;

			DataRowCollection drc = dt.Rows;
            drc_count = drc.Count;
            tpt_count = m_teplText.Count;

            for (i = 0; i < tpt_count; i++)
            {
                template = Convert.ToString(m_teplValue[i]);

                GetMatches(template);
                m_matches = m_matchNames.Count;
                m_inText.Remove(0, m_inText.Length);

                if (i < drc_count)
                {
                    for (j = 0; j < m_matches; j++)
                    {
                        n_value = m_matchNames[j].ToString();
                        m_value = m_matchMethods[j].ToString();
                        c_value = Convert.ToString(drc[i][n_value]);
                        SetProperty(n_value, m_value, c_value, ref template);
                    }
                }
                else
                {
                    template = "";
                }

                m_inText.Append(template);
                m_outHtml.Replace(Convert.ToString(m_teplText[i]), m_inText.ToString());
            }

			return m_outHtml.ToString();
		}

        /// <summary>
        /// 每个循环体<#List>当作一个完整的数据列表，把所有行记录处理完毕
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="text">模板文本内容</param>
        /// <returns></returns>
        public string ByListRowDataTable(DataTable dt, string text)
        {
            GetTemplate(text);

            int i = 0;
            int j = 0;
            int m = 0;
            int drc_count = 0;
            int tpt_count = 0;
            string n_value = null;
            string m_value = null;
            string c_value = null;
            string org_template = null;
            string use_template = null;

            DataRowCollection drc = dt.Rows;
            DataRow dr = null;
            drc_count = drc.Count;
            tpt_count = m_teplText.Count;

            for (i = 0; i < tpt_count; i++)
            {
                org_template = Convert.ToString(m_teplValue[i]);

                GetMatches(org_template);
                m_matches = m_matchNames.Count;
                m_inText.Remove(0, m_inText.Length);

                for (m = 0; m < drc_count; m++)
                {
                    dr = drc[m];
                    use_template = org_template;

                    for (j = 0; j < m_matches; j++)
                    {
                        n_value = m_matchNames[j].ToString();
                        m_value = m_matchMethods[j].ToString();
                        c_value = Convert.ToString(dr[n_value]);
                        SetProperty(n_value, m_value, c_value, ref use_template);
                    }

                    m_inText.Append(use_template);
                }

                m_outHtml.Replace(Convert.ToString(m_teplText[i]), m_inText.ToString());
            }

            return m_outHtml.ToString();
        }

        /// <summary>
        /// 通过数组转换HTML
        /// </summary>
        /// <param name="strs"></param>
        /// <param name="text">模板文本内容</param>
        /// <returns></returns>
        public string ByStringArray(string[] strs, string text)
        {
            GetTemplate(text);

            int i = 0;
            string n_value = null;
            string m_value = null;
            string c_value = null;
            string template = Convert.ToString(m_teplValue[0]);

            GetMatches(template);
            m_matches = m_matchNames.Count;
            m_inText.Remove(0, m_inText.Length);

            for (i = 0; i < strs.Length; i++)
            {
                n_value = m_matchNames[i].ToString();
                m_value = m_matchMethods[i].ToString();
                c_value = strs[i];
                SetProperty(n_value, m_value, c_value, ref template);
            }

            m_inText.Append(template);

            string t = Convert.ToString(m_teplText[0]);
            string s = m_inText.ToString();
            m_outHtml.Replace(t, s);

            return m_outHtml.ToString();
        }

        /// <summary>
        /// 从文本中解释出替换元素
        /// </summary>
        /// <param name="text"></param>
        private void GetTemplate(string text)
		{
			m_pattern = @"(<#List>([\S\s]*?)</#List>)";
			m_teplText.Clear();	    // 保存整个List值
			m_teplValue.Clear();	// 保存List中的模板

            m_mc = Regex.Matches(text, m_pattern, RegexOptions.Compiled | RegexOptions.Multiline);
			foreach(Match m in m_mc)
			{
				m_teplText.Add(m.Result("$1"));
				m_teplValue.Add(m.Result("$2"));
			}

			if(m_mc.Count == 0)
			{
                m_teplText.Add(text);
                m_teplValue.Add(text);
			}

			//输出数据
			m_mc = null;
			m_outHtml.Remove(0, m_outHtml.Length);
            m_outHtml.Append(text);
		}

        /// <summary>
        /// 使用UTF-8获取文本内容
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
		public string GetTextFile(string file)
		{
			StreamReader stream = new StreamReader(file, Encoding.UTF8);
			string text = stream.ReadToEnd();
			stream.Close();
			return text;
		}

		private void GetMatches(string text)
		{
			m_matchNames.Clear();
			m_matchMethods.Clear();

			int n_index = -1;
			int m_index = -1;

			object n_obj = null;
			object m_obj = null;

			MatchCollection  _mc = Regex.Matches(text, @"\{([^\$]*)\$([a-zA-Z0-9_]+)\}");
			for(int i=0; i<_mc.Count; i++)
			{
				n_obj = _mc[i].Result("$2");
				m_obj = _mc[i].Result("$1");

				n_index = m_matchNames.IndexOf(n_obj);
				m_index = m_matchMethods.IndexOf(m_obj);

				if(n_index != m_index || (n_index == -1 && m_index == -1))
				{
					m_matchNames.Add(n_obj);
					m_matchMethods.Add(m_obj);
				}
			}
		}

		private string GetLength(string text)
		{
			if(text == "")
			{
				return "0";
			}
			else
			{
				byte[] arr = Encoding.Default.GetBytes(text);
				return arr.Length.ToString();
			}
		}

		private void SetTemplate(IDataReader idr)
		{
			int i = 0;
			int rowIndex = 0;
			string temp = null;
			string n_value = null;
			string m_value = null;
			string c_value = null;
			string template = Convert.ToString(m_teplValue[m_teplIndex]);

			GetMatches(template);
			m_matches = m_matchNames.Count;
			m_inText.Remove(0, m_inText.Length);

			if(m_matchNames.IndexOf("rowIndex") > -1)
			{
				while(idr.Read())
				{
					rowIndex++;
					temp = template;
					for(i=0; i<m_matches; i++)
					{
						n_value = m_matchNames[i].ToString();
						m_value = m_matchMethods[i].ToString();

						if(n_value == "rowIndex")
						{
							c_value =rowIndex.ToString();
						}
						else
						{
							c_value = Convert.ToString(idr[n_value]);
						}

						SetProperty(n_value, m_value, c_value, ref temp);
					}
				
					m_inText.Append(temp);
				}
			}
			else
			{
				while(idr.Read())
				{
					rowIndex++;
					temp = template;
					for(i=0; i<m_matches; i++)
					{
						n_value = m_matchNames[i].ToString();
						m_value = m_matchMethods[i].ToString();
						c_value = Convert.ToString(idr[n_value]);

						SetProperty(n_value, m_value, c_value, ref temp);
					}
				
					m_inText.Append(temp);
				}
			}

			string t = Convert.ToString(m_teplText[m_teplIndex]);
			string s = m_inText.ToString();
			m_outHtml.Replace(t, s);
		}

		private void SetTemplate(DataTable dt)
		{
			int i = 0;
			string temp = null;
			string n_value = null;
			string m_value = null;
			string c_value = null;
			string template = Convert.ToString(m_teplValue[m_teplIndex]);

			GetMatches(template);
			m_matches = m_matchNames.Count;
			m_inText.Remove(0, m_inText.Length);

			DataRowCollection drc = dt.Rows;
			for(int j=0; j<drc.Count; j++)
			{
				temp = template;
				for(i=0; i<m_matches; i++)
				{
					n_value = m_matchNames[i].ToString();
					m_value = m_matchMethods[i].ToString();
                    c_value = Convert.ToString(drc[j][n_value]);
					SetProperty(n_value, m_value, c_value, ref temp);
				}
				
				m_inText.Append(temp);
			}

			string t = Convert.ToString(m_teplText[m_teplIndex]);
			string s = m_inText.ToString();
			m_outHtml.Replace(t, s);
		}

		private void SetProperty(string n_value, string m_value, string c_value, ref string temp)
		{
			if(m_value == "")
			{
				m_pattern = string.Concat(@"\{\$", n_value, @"\}");
			}
			else if(m_value.IndexOf("DateFormat") > -1)
			{
				string p = Regex.Replace(m_value, @"DateFormat\(", "");
				p = Regex.Replace(p, @"\)", "");

				if(c_value != null && c_value != "")
                {
                    DateTime dt = Convert.ToDateTime(c_value);
                    c_value = dt.ToString(p);
				}

				m_value = m_value.Replace("(", "\\(");
				m_value = m_value.Replace(")", "\\)");
				m_pattern = string.Concat(@"\{", m_value, @"\$", n_value, @"\}");
			}
            else if (m_value.IndexOf("if") > -1)
            {
                string p = Regex.Replace(m_value, @"if\(", "");
                p = Regex.Replace(p, @"\)", "");
                int p1 = p.IndexOf("?");
                int p2 = p.IndexOf(":");

                string v = p.Substring(0, p1);
                if (p2 == -1)
                {
                    if (c_value.Equals(v))
                    {
                        c_value = p.Substring(p1 + 1);
                    }
                }
                else
                {
                    string v1 = p.Substring(p1 + 1, (p2 - p1 - 1));
                    string v2 = p.Substring(p2 + 1);
                    if (c_value.Equals(v))
                    {
                        c_value = v1;
                    }
                    else
                    {
                        c_value = v2;
                    }
                }

                m_value = Replace(m_value);
                m_pattern = string.Concat(@"\{", m_value, @"\$", n_value, @"\}");
            }
            else if (m_value.IndexOf("NumFormat") > -1)
            {
                string p = Regex.Replace(m_value, @"NumFormat\(", "");
                p = Regex.Replace(p, @"\)", "");

                if (c_value != null && c_value != "")
                {
                    decimal dec = Convert.ToDecimal(c_value);
                    c_value = dec.ToString(p);
                    if (p == "c")
                    {
                        c_value = c_value.Replace("￥", "");
                    }
                }

                m_value = m_value.Replace("(", "\\(");
                m_value = m_value.Replace(")", "\\)");
                m_pattern = string.Concat(@"\{", m_value, @"\$", n_value, @"\}");
            }
			else if(m_value.IndexOf("Substring") > -1)
			{
				string p = Regex.Replace(m_value, @"Substring\(", "");
				p = Regex.Replace(p, @"\)", "");
				string[] s = p.Split(',');
                int pos = Convert.ToInt32(s[0]);

                if (c_value.Length < pos)
                {
                    pos = c_value.Length - 1;
                }

                c_value = Regex.Replace(c_value, @"<[\s\S][^>]*>", "");
                c_value = Regex.Replace(c_value, @"\s", "");
				if(s.Length == 1)
				{
                    c_value = c_value.Substring(pos);
				}
				else if(s.Length == 2)
				{
                    int len = int.Parse(s[1]);
                    if (len > c_value.Length)
                    {
                        c_value = c_value.Substring(pos);
                    }
                    else
                    {
                        c_value = c_value.Substring(pos, len);
                    }
				}

				m_value = m_value.Replace("(", "\\(");
				m_value = m_value.Replace(")", "\\)");
				m_pattern = string.Concat(@"\{", m_value, @"\$", n_value, @"\}");
			}
            else if (m_value == "InnerText()")
            {
                c_value = Regex.Replace(c_value, @"<[\s\S][^>]*>", "");
                m_pattern = string.Concat(@"\{InnerText\(\)\$", n_value, @"\}");
            }
			else
			{
				m_pattern = string.Concat(@"\{", m_value, @"\$", n_value, @"\}");
			}

			temp = Regex.Replace(temp, m_pattern, c_value, RegexOptions.Compiled);
		}

		private string Replace(string str)
		{
			str = str.Replace("(", "\\(");
			str = str.Replace(")", "\\)");
			str = str.Replace("?", "\\?");
			str = str.Replace("[", "\\[");
			str = str.Replace("]", "\\]");
			return str;
		}

		private void SetDataTable(IDataReader idr, ref DataTable dt)
		{
			int i = 0;
			object[] obj = null;
			int m = idr.FieldCount;
			for(i=0; i<m; i++)
			{
				dt.Columns.Add(idr.GetName(i), idr.GetFieldType(i));
			}
			while(idr.Read())
			{
				obj = new object[m];
				for(i=0; i<m; i++)
				{
					obj[i] = idr.GetValue(i);
				}
				dt.Rows.Add(obj);
			}
		}

		#region IDisposable 成员
		public void Dispose()
		{
			m_mc = null;
			m_inText = null;
			m_outHtml = null;
			m_matchNames = null;
			m_matchMethods = null;
			m_teplText = null;
			m_teplValue = null;
            GC.SuppressFinalize(this);
            GC.Collect();
		}
		#endregion
	}
}
