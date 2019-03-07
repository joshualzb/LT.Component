using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LT.Component.Web.Controls
{

    /// <summary>
    /// RepeaterBase 通用类
    /// </summary>
    public class RepeaterBase : Control, INamingContainer
    {
        private ITemplate _itemTemplate;
        private object _dataSource;

        [PersistenceMode(PersistenceMode.InnerProperty), Browsable(false), Description("Repeater_ItemTemplate")]
        public virtual ITemplate ItemTemplate
        {
            get { return _itemTemplate; }
            set { _itemTemplate = value; }
        }

        [Description("BaseDataBoundControl_DataSource"), Bindable(true), DefaultValue((string)null), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual object DataSource
        {
            get { return _dataSource; }
            set { _dataSource = value; }
        }

        protected virtual RepeaterItem CreateItem(int itemIndex, ListItemType itemType)
        {
            return new RepeaterItem(itemIndex, itemType);
        }

        public override void DataBind()
        {
            this.OnDataBinding(EventArgs.Empty);
            base.DataBind();
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
            this.Controls.Clear();
            ClearChildViewState();
            ChildControlsCreated = true;
        }

        protected virtual void InitializeItem(RepeaterItem item)
        {
            _itemTemplate.InstantiateIn(item);
        }
    }
}
