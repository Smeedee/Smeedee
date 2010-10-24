using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyMVVM.Framework.Conventions;
using Smeedee.Client.Framework.Services;
using System.ComponentModel;
using TinyMVVM.Framework;

namespace Smeedee.Client.Framework.ViewModel.DockBarItems
{
    public partial class AddWidgetDockBarItem
    {
        public AddWidgetDockBarItem(string itemName)
        {
            ItemName = itemName;
            OnInitialize();
            ApplyConvention(new BindCommandsDelegatesToMethods());
        }

        partial void OnInitialize()
        {
            Description = "Settings";
#if SILVERLIGHT
            Icon = new SettingsIcon(ItemName);
#endif
            this.PropertyChanged += AddWidgetDockBarItem_PropertyChanged;
        }

        void AddWidgetDockBarItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SlideShow")
            {
                Click = new DelegateCommand(() => SlideShow.OnAddSlide());
            }
        }
    }
}
