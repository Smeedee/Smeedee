using System.Windows.Controls;
using APD.Client.Widget.Admin.SL;
using APD.Client.Widget.Admin.ViewModels.Configuration;
using APD.Client.Framework.Factories;

namespace APD.Client.Widget.Admin.Plugins
{
    public class PIConfigPlugin : ConfigPlugin
    {
        public override string Name
        {
            get { return "Project Management"; }
        }

        public override UserControl UserInterface
        {
            get
            {
                var viewModel = new ProviderConfigItemViewModel(UIInvokerFactory.Assemlble(), configuration);
                var view = new ProviderConfigItemView();
                view.DataContext = viewModel;

                return view;
            }
        }
    }
}
