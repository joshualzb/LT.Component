using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LT.Component.Web.Controls
{
    /// <summary>
    /// TextBox
    /// </summary>
    public class AddressDropdown : System.Web.UI.Control, IPostBackDataHandler
    {
        private string m_Province;
        private string m_City;
        private string m_District;
        private string m_Exact;

        /// <summary>
        /// 构造函数
        /// </summary>
        public AddressDropdown()
        {
        }

        bool IPostBackDataHandler.LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
        {
            String presentValue = AddressPCD + "-" + AddressExact;
            String postedValue = postCollection[postDataKey];
            m_Province = postCollection[postDataKey + "ProvinceId"];
            m_City = postCollection[postDataKey + "CityId"];
            m_District = postCollection[postDataKey + "DistrictId"];
            m_Exact = postCollection[postDataKey + "Exact"];
            String currentValue = string.Format("{0}-{1}-{2}-{3}", m_Province, m_City, m_District, m_Exact);
            if (presentValue == null || !presentValue.Equals(currentValue))
            {
                AddressPCD = string.Format("{0}-{1}-{2}", m_Province, m_City, m_District);
                AddressExact = m_Exact;
                return true;
            }
            return false;
        }

        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {
            OnTextChanged(EventArgs.Empty);
        }


        public event EventHandler TextChanged;

        public virtual void RaisePostDataChangedEvent()
        {
            OnTextChanged(EventArgs.Empty);
        }
        protected virtual void OnTextChanged(EventArgs e)
        {
            if (TextChanged != null)
            {
                TextChanged(this, e);
            }
        }


        /// <summary>
        /// 宽
        /// </summary>
        public Unit Width
        {
            get;
            set;
        }

        /// <summary>
        /// 地址-省市区（使用中横线分隔）
        /// </summary>
        public string AddressPCD
        {
            get { return (String)ViewState["AddressPCD"]; }
            set { ViewState["AddressPCD"] = value; }
        }

        /// <summary>
        /// 具体门牌地址（不含省市区）
        /// </summary>
        public string AddressExact
        {
            get { return (String)ViewState["AddressExact"]; }
            set { ViewState["AddressExact"] = value; }
        }

        /// <summary>
        /// 输出控件到页面
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {

            //输出页面
            string Editor = this.EditorForm();
            writer.Write(Editor);
            base.Render(writer);
        }

        /// <summary>
        /// 输出页面的HTML标签
        /// </summary>
        /// <returns></returns>
        private string EditorForm()
        {
            var text = @"<select id=""" + this.UniqueID + @"ProvinceId"" name=""" + this.UniqueID + @"ProvinceId"" ><option value=""0"">==省份==</option></select>
            <select id=""" + this.UniqueID + @"CityId"" name=""" + this.UniqueID + @"CityId""><option value=""0"">==城市==</option></select>
            <select id=""" + this.UniqueID + @"DistrictId"" name=""" + this.UniqueID + @"DistrictId""><option value=""0"">==区==</option></select>
            <input name=""" + this.UniqueID + @"Exact"" id=""" + this.UniqueID + @"Exact"" style=""width:" + Width + @""" type=""text"" value=""" + AddressExact + @""" />
            <input type=""hidden"" name=""" + this.UniqueID + @""" id=""" + this.UniqueID + @"""  value=""" + AddressPCD + @""" />
             <script type=""text/javascript"">
                $(document).ready(function(){
                    $('form').bind('reset',function(){
                        AddressDropdown('" + this.UniqueID + @"');
                    });
                    AddressDropdown('" + this.UniqueID + @"');
                })
             </script>";
            return text;
        }
    }
}
