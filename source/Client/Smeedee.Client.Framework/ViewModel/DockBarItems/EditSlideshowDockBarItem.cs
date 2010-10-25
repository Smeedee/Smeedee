using System.ComponentModel;
#if SILVERLIGHT
using Smeedee.Client.Framework.SL.Resources.Graphic.Icons;
#endif
using TinyMVVM.Framework;
using TinyMVVM.Framework.Conventions;

namespace Smeedee.Client.Framework.ViewModel.DockBarItems
{
    public partial class EditSlideshowDockBarItem
    {
        public EditSlideshowDockBarItem(string itemName)
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
            this.PropertyChanged += EditSlideshowDockBarItem_PropertyChanged;
        }

        void EditSlideshowDockBarItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SlideShow")
            {
                Click = new DelegateCommand(() => SlideShow.OnEdit());
            }
        }
    }
}
