using System.Windows;
using System.Windows.Controls;

namespace Smeedee.Client.Framework.SL.Views.Dialogs
{
    public partial class WidgetSelectionPreviewView : UserControl
    {
        public WidgetSelectionPreviewView()
        {
            InitializeComponent();
            Loaded += (o, e) => Focus();
        }
    }
}
