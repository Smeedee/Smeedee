using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APD.Client.Widget.Admin.ViewModels.Configuration;
using System.Windows.Controls;
using APD.Client.Widget.Admin.SL;
using APD.Client.Framework.Factories;
using APD.Client.Widget.Admin.Controllers;
using APD.Client.Widget.Admin.SL.Services;
using APD.Client.Framework;
using APD.DomainModel.Framework.Services;

namespace APD.Client.Widget.Admin.Plugins
{
    public class VCSConfigPlugin : ConfigPlugin
    {
        public override string Name
        {
            get { return "Version Control System"; }
        }

        public override UserControl UserInterface
        {
            get
            {
                var viewModel = new ProviderConfigItemViewModel(UIInvokerFactory.Assemlble(), configuration);
                var backgroundWorker = new AsyncClient<bool>();
                var vcsCredentialsCheckerProxy =
                    ServiceLocator.Instance.GetInstance<ICheckIfCredentialsIsValid>();

                var authCheckerController = new CredentialsCheckerController(viewModel,
                    vcsCredentialsCheckerProxy,
                    backgroundWorker);
                var view = new ProviderConfigItemView();
                view.DataContext = viewModel;
                return view;
            }
        }
    }
}
