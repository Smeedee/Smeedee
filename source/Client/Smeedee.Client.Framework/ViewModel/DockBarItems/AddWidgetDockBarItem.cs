using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Smeedee.Client.Framework.Services;
#if SILVERLIGHT
using Smeedee.Client.Framework.SL.Resources.Graphic.Icons;
#endif
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Client.Framework.ViewModel.Dialogs;
using TinyMVVM.Framework;
using TinyMVVM.Framework.Conventions;
using TinyMVVM.IoC;

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
