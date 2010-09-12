using System.ComponentModel.Composition;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Widget.Browser.Views;

namespace Smeedee.Widget.Browser
{
    [Export(typeof(Client.Framework.ViewModel.Widget))]
    public class BrowserSlide : Client.Framework.ViewModel.Widget
    {
        public BrowserSlide()
        {
            Title = "Web browser slide:)";
            View = new BrowserView()
            {
                DataContext = this
            };

            SettingsView = new BrowserSettingsView()
            {
                DataContext = this
            };
        }
    }
}
