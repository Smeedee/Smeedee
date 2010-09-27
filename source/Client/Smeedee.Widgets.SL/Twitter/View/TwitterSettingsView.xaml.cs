using System.Windows.Controls;
using System.Windows.Input;
using Smeedee.Widgets.SL.Twitter.ViewModel;

namespace Smeedee.Widgets.SL.Twitter.View
{
    public partial class TwitterSettingsView : UserControl
    {

        public TwitterSettingsView()
        {
            InitializeComponent();

            KeyDown += TwitterSettingsView_KeyDown;
            expander.Expanded += (o, e) => expander.Header = "Hide";
            expander.Collapsed += (o, e) => expander.Header = "More";
            refreshIntervalTimeUpDown.MouseEnter += (o, e) => refreshIntervalTimeUpDown.Focus();
        }

        void TwitterSettingsView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var dataContext = (TwitterSettingsViewModel) DataContext;
                dataContext.Search.Execute(null);
            }
        }
    }
}
