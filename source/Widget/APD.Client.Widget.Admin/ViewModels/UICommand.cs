using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

using APD.Client.Framework.Controllers;
using System.Threading;


namespace APD.Client.Widget.Admin.ViewModels
{
    public class UICommand : ICommand
    {
        private ITriggerCommand commandTrigger;
        private ITriggerEvent<EventArgs> eventTrigger;

        public UICommand(ITriggerCommand commandTrigger)
        {
            this.commandTrigger = commandTrigger;
        }

        public UICommand(ITriggerEvent<EventArgs> eventTrigger)
        {
            this.eventTrigger = eventTrigger;
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (commandTrigger != null)
                commandTrigger.Trigger();

            if (eventTrigger != null)
                eventTrigger.NewEvent(EventArgs.Empty);
        }

        #endregion
    }
}
