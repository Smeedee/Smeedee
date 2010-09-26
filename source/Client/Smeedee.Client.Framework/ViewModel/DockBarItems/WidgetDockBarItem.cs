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
    public partial class WidgetDockBarItem
    {
        private IModalDialogService dialogService;

        public WidgetDockBarItem(string itemName)
        {
            ItemName = itemName;
            OnInitialize();
            ApplyConvention(new BindCommandsDelegatesToMethods());
        }

        partial void OnInitialize()
        {
            dialogService = this.GetDependency<IModalDialogService>();

            Description = "Settings";
#if SILVERLIGHT
            Icon = new SettingsIcon(ItemName);
#endif
            this.PropertyChanged += WidgetDockBarItem_PropertyChanged;
        }

        void WidgetDockBarItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Widget")
            {
                Click = new DelegateCommand(() =>
                {
                    dialogService.Show(new WidgetDialog{ Widget = Widget , Progressbar = Widget.ViewProgressbar}, (result) => { });
                });
            }
        }
    }
}
