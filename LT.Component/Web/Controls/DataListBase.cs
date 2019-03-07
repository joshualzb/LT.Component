using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LT.Component.Web.Controls
{
    public class BaseDataList : Repeater
    {
        public BaseDataList()
        {

        }

        /// <summary>
        /// 列表内容查询条件控件
        /// </summary>
        [Browsable(false), DefaultValue(null), Description("列表内容查询条件控件"), PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual SearchQuery SearchQuery
        {
            get { return _searchQuery; }
            set
            {
                if (_searchQuery != null)
                {
                    this.Controls.Remove(_searchQuery);
                }

                _searchQuery = value;
                if (_searchQuery != null)
                {
                    this.Controls.Add(_searchQuery);
                }
            }
        }
        private SearchQuery _searchQuery = new SearchQuery();

        /// <summary>
        /// 列表没有数据显示页控件
        /// </summary>
        [Browsable(false), DefaultValue(null), Description("列表没有数据显示页控件"), PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate NoDataTemplate
        {
            set { _noDataTemplate = value; }
            get { return _noDataTemplate; }
        }
        private ITemplate _noDataTemplate;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.DataBind();

            if (this.Items.Count == 0 && _noDataTemplate != null)
            {
                _noDataTemplate.InstantiateIn(this);
            }
        }
    }
}
