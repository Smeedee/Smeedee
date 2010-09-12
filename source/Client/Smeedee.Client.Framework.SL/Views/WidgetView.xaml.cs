using System.Windows.Controls;

namespace Smeedee.Client.Framework.SL.Views
{
    public partial class WidgetView : UserControl
    {
        public WidgetView()
        {
            InitializeComponent();
            ExitSettingsButton.Click += (o, e) => Focus();
        }
    }
}
