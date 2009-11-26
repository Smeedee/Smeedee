#region File header

// <copyright>
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
// </copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using APD.Client.Framework.SL;
using APD.Client.Framework.ViewModels;
using APD.Client.Widget.Admin.SL.Repositories;
using APD.DomainModel.Config;
using APD.DomainModel.Users;

using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Unity;
using APD.Client.Framework;
using APD.Client.Widget.Admin.ViewModels;
using APD.Client.Widget.Admin.SL.Views;
using APD.Client.Widget.Admin.Controllers;

namespace APD.Client.Widget.Admin.SL
{
    public class AdminModule : IVisibleModule, IModule
    {
        private IUnityContainer iocContainer;

        public AdminModule(IUnityContainer iocContainer)
        {
            this.iocContainer = iocContainer;
        }


        protected void ConfigureIocContainer()
        {
            var eventAggregator = iocContainer.Resolve<IEventAggregator>();
            var showModalDialogEventTrigger = new ShowModalDialogEventTrigger(eventAggregator);

            var saveCommandWire = new CommandNotifierWiring<EventArgs>();
            var updateCommandWire = new CommandNotifierWiring<EventArgs>();
            var updateCommandWireAdapter = new CommandNotifierAdapter(updateCommandWire);
            var editCommandWire = new MainPage.EditCommandWiring();
            var saveConfigCommand = new CommandNotifierWiring<EventArgs>();
            var refreshConfigCommand = new CommandNotifierWiring<EventArgs>();

            var uiInvoker = iocContainer.Resolve<IInvokeUI>();
          
            var adminViewModel = new AdminViewModel(uiInvoker, saveCommandWire, updateCommandWire, editCommandWire,
                saveConfigCommand, refreshConfigCommand);

            var userdbRepository = new UserdbWebserviceRepository();
            var controller = new UserdbController(adminViewModel.Userdb,
                                                  userdbRepository,
                                                  new AsyncClient<IEnumerable<Userdb>>(),
                                                  updateCommandWireAdapter,
                                                  userdbRepository,
                                                  saveCommandWire,
                                                  editCommandWire,
                                                  showModalDialogEventTrigger,
                                                  uiInvoker);

            adminViewModel.Userdb.DuckTypedData = new ObservableCollectionWrapper<UserViewModel, EditableUserViewModel>(adminViewModel.Userdb.Data);
            iocContainer.RegisterInstance(typeof (UserdbController), controller);

            var configurationRepository = new ConfigurationWebServiceRepository();
            var configController = new ConfigurationController(adminViewModel.Configuration,
                                                               configurationRepository,
                                                               new AsyncClient<IEnumerable<Configuration>>(),
                                                               configurationRepository,
                                                               saveConfigCommand,
                                                               refreshConfigCommand);
            iocContainer.RegisterInstance(typeof(ConfigurationController), configController);

            var adminView = new AdminView();
            adminView.DataContext = adminViewModel;
            iocContainer.RegisterInstance(typeof (AdminView), adminView);

            var closeModalDialogCommandWire = new CommandNotifierWiring<HideModalDialogEventArgs>();
            var modalDialogViewModel = new ModalDialogViewModel(uiInvoker, closeModalDialogCommandWire);
            var modalDialogController =
                new ModalDialogController(modalDialogViewModel, new ShowModalDialogEventNotifier(eventAggregator), closeModalDialogCommandWire);
            iocContainer.RegisterInstance(typeof (ModalDialogController), modalDialogController);

            var modalDialogView = new ModalDialogWindowContainerView();
            modalDialogView.DataContext = modalDialogViewModel;
            modalDialogViewModel.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == "Visible")
                {
                    if (modalDialogViewModel.Visible)
                    {
                        modalDialogView.Show();
                    }
                    else
                    {
                        modalDialogView.DialogResult = true;
                    }
                }

            };

        }

        #region IVisibleModule Members

        public UserControl View
        {
            get { return iocContainer.Resolve<AdminView>(); }
        }

        #endregion

        #region IModule Members

        public void Initialize()
        {
            ConfigureIocContainer();
        }

        #endregion
    }
}