using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using APD.Client.Widget.Admin.SL;
using APD.Client.Widget.Admin.ViewModels.Configuration;
using APD.Client.Framework.Factories;

namespace APD.Client.Widget.Admin.Plugins
{
    public class CIConfigPlugin : ConfigPlugin
    {
        public override string Name
        {
            get { return "Continuous Integration"; }
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
