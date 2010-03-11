using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using APD.Client.Widget.Admin.ViewModels;
using APD.Client.Framework.SL;
using APD.Client.Framework.ViewModels;
using System.Collections.ObjectModel;
using APD.Client.Widget.Admin.Controllers;
using APD.DomainModel.Users;
using APD.Client.Framework;

using Microsoft.Practices.Composite.Events;
using APD.Client.Framework.Controllers;
using APD.Client.Widget.Admin.SL.Views;
using APD.Client.Widget.Admin.SL.Repositories;
using APD.DomainModel.Framework;
using APD.DomainModel.Config;
using APD.Client.Framework.Factories;


namespace APD.Client.Widget.Admin.SL
{
    public partial class MainPage : UserControl
    {
        public class EditCommandWiring : Notifier<EventArgs>, ITriggerEvent<EventArgs>
        {

            #region ITriggerEvent<EventArgs> Members

            public void NewEvent(EventArgs args)
            {
                TriggerNotification(args);
            }

            #endregion
        }

        public MainPage() : this(new EventAggregator())
        {
        }

        public MainPage(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            var uiInvoker = new SilverlightUIInvoker(Dispatcher);
            UIInvokerFactory.UIInvoker = uiInvoker;

            var showModalDialogEventTrigger = new ShowModalDialogEventTrigger(eventAggregator);

            var saveCommandWire =  new CommandNotifierWiring<EventArgs>();
            var updateCommandWire = new CommandNotifierWiring<EventArgs>();
            var updateCommandWireAdapter = new CommandNotifierAdapter(updateCommandWire);
            var saveConfigCommand = new CommandNotifierWiring<EventArgs>();
            var refreshConfigCommand = new CommandNotifierWiring<EventArgs>();

            var editCommandWire = new EditCommandWiring();
            var closeModalDialogCommandWire = new CommandNotifierWiring<HideModalDialogEventArgs>();

            var saveHolidaysCommandTrigger = new CommandNotifierWiring<EventArgs>();
            var reloadHolidaysCommandTrigger = new CommandNotifierWiring<EventArgs>();
            var createNewHolidayTrigger = new CommandNotifierWiring<EventArgs>();
            var deleteSelectedHolidayTrigger = new CommandNotifierWiring<EventArgs>();

            var adminViewModel = new AdminViewModel(uiInvoker, saveCommandWire,
                                                    updateCommandWire, editCommandWire,
                                                    saveConfigCommand, refreshConfigCommand,
                                                    saveHolidaysCommandTrigger,
                                                    reloadHolidaysCommandTrigger,
                                                    createNewHolidayTrigger,
                                                    deleteSelectedHolidayTrigger);

            var userdbRepository = new UserdbMockRepository();
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
            DataContext = adminViewModel;

            var configurationRepository = new ConfigurationMockRepository();
            var saveNotifier = new CommandNotifierWiring<EventArgs>();
            var refreshNotifier = new CommandNotifierWiring<EventArgs>();
            var configController = new ConfigurationController(adminViewModel.Configuration,
                                                               configurationRepository,
                                                               new AsyncClient<IEnumerable<Configuration>>(),
                                                               configurationRepository,
                                                               saveNotifier,
                                                               refreshNotifier);

            var modalDialogViewModel = new ModalDialogViewModel(uiInvoker, closeModalDialogCommandWire); 
            var modalDialogController =
                new ModalDialogController(modalDialogViewModel, new ShowModalDialogEventNotifier(eventAggregator), closeModalDialogCommandWire);

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
    }
}
