using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.Client.Framework.Commands;
using APD.Client.Framework.Controllers;
using APD.Client.Widget.Admin.ViewModels;
using APD.Client.Widget.Admin.Views;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using APD.Client.Framework.ViewModels;

namespace APD.Client.Widget.Admin.Controllers
{
    public class ShowModalDialogEventArgs : EventArgs
    {
        public IModalDialog UserInterface { get; set; }
        public AbstractViewModel ViewModel { get; set; }
    }

    public class HideModalDialogEventArgs : EventArgs
    {
        
    }

    public class ShowModalDialogEventTrigger : ITriggerEvent<ShowModalDialogEventArgs>
    {
        private IEventAggregator eventAggregator;

        public ShowModalDialogEventTrigger(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        #region ITriggerEvent<ShowModalDialogEventArgs> Members

        public void NewEvent(ShowModalDialogEventArgs args)
        {
            eventAggregator.GetEvent<CompositePresentationEvent<ShowModalDialogEventArgs>>().
                Publish(args);
        }

        #endregion
    }

    public class ShowModalDialogEventNotifier : Notifier<ShowModalDialogEventArgs>
    {
        private IEventAggregator eventAggregator;

        public ShowModalDialogEventNotifier(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

            this.eventAggregator.GetEvent<CompositePresentationEvent<ShowModalDialogEventArgs>>().Subscribe(New_ShowModalDialogEvent);
        }

        public void New_ShowModalDialogEvent(ShowModalDialogEventArgs e)
        {
            TriggerNotification(e);
        }
    }

    public class HideModalDialogEventNotifier : Notifier<HideModalDialogEventArgs>
    {
        private IEventAggregator eventAggregator;

        public HideModalDialogEventNotifier(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

            this.eventAggregator.GetEvent<CompositePresentationEvent<HideModalDialogEventArgs>>().Subscribe(New_HideModalDialogEvent);
        }

        public void New_HideModalDialogEvent(HideModalDialogEventArgs e)
        {
            TriggerNotification(e);
        }
    }

    public class ModalDialogController
    {
        private INotify<ShowModalDialogEventArgs> showModalDialogEventNotifier;
        private INotify<HideModalDialogEventArgs> hideModalDialogEventNotifier;
        private ModalDialogViewModel modalDialogViewModel;

        public ModalDialogController(ModalDialogViewModel modalDialogViewModel, INotify<ShowModalDialogEventArgs> showModalDialogEventNotifier,
            INotify<HideModalDialogEventArgs> hideModalDialogEventNotifier)
        {
            ValidateThatParamIsNotNullObj(modalDialogViewModel, "modalDialogViewModel");
            ValidateThatParamIsNotNullObj(showModalDialogEventNotifier, "showModalDialogCommandNotifier");
            ValidateThatParamIsNotNullObj(hideModalDialogEventNotifier, "hideModalDialogEventNotifier");

            this.modalDialogViewModel = modalDialogViewModel;

            this.showModalDialogEventNotifier = showModalDialogEventNotifier;
            this.showModalDialogEventNotifier.NewNotification += new EventHandler<ShowModalDialogEventArgs>(showModalDialogCommandNotifier_NewNotification);
            this.hideModalDialogEventNotifier = hideModalDialogEventNotifier;
            this.hideModalDialogEventNotifier.NewNotification += new EventHandler<HideModalDialogEventArgs>(hideModalDialogEventNotifier_NewNotification);
        }

        private void ValidateThatParamIsNotNullObj(Object param, string paramName)
        {
            if (param == null)
                throw new ArgumentException("Value can not be null", paramName);
        }

        void hideModalDialogEventNotifier_NewNotification(object sender, HideModalDialogEventArgs e)
        {
            modalDialogViewModel.Visible = false;
            modalDialogViewModel.ModalDialog = null;
        }

        void showModalDialogCommandNotifier_NewNotification(object sender, ShowModalDialogEventArgs e)
        {
            modalDialogViewModel.Visible = true;
            modalDialogViewModel.ModalDialog = e.UserInterface;
            modalDialogViewModel.ViewModel = e.ViewModel;
        }
    }
}
