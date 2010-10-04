using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Client.Framework.ViewModel.DockBarItems;

namespace Smeedee.Client.SL
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
            if(Application.Current.IsRunningOutOfBrowser)
            {
                AddHandler(KeyDownEvent, new KeyEventHandler(ExitFullScreenWithEscape), true);            
            }
        }

        private void ExitFullScreenWithEscape(object sender, KeyEventArgs keyEvent)
        {
            if (keyEvent.Key == Key.Escape && Application.Current.Host.Content.IsFullScreen)
            {
                var dockBar = adminDockView.DataContext as DockBar;
                if (dockBar != null)
                {
                    var dockBarItems = dockBar.Items;
                    foreach (var dockBarItem in dockBarItems.OfType<FullScreen>())
                    {
                        dockBarItem.Click.Execute();
                    }
                }
                
            }
        }
    }
}
