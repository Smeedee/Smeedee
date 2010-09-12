using System.ComponentModel;
using System.Windows;

namespace Smeedee.Client.Framework.ViewModel.Dialogs
{
    public partial class WidgetDialog
    {
        partial void OnInitialize()
        {
            Width = 800;
            Height = 600;
            DisplayCancelButton = false;
        }

        public override FrameworkElement View
        {
            get
            {
                if (Widget != null)
                    return Widget.View;
                else
                    return null;
            }
            set
            {
                if (Widget != null)
                    Widget.View = value;
            }
        }
    }
}
