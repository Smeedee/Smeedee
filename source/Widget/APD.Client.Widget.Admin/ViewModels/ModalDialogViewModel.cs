using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.Client.Framework.ViewModels;
using APD.Client.Framework;
using APD.Client.Widget.Admin.Views;
using System.Windows.Input;


namespace APD.Client.Widget.Admin.ViewModels
{
    public class ModalDialogViewModel : AbstractViewModel
    {
        private bool visible;
        public bool Visible
        {
            get { return visible; }
            set
            {
                if( value != visible )
                {
                    visible = value;
                    TriggerPropertyChanged<ModalDialogViewModel>(vm => vm.Visible);
                }
            }
        }

        public IModalDialog ModalDialog { get; set; }
        public AbstractViewModel ViewModel { get; set; }

        public ICommand CloseUICommand { get; set; }

        public ModalDialogViewModel(IInvokeUI uiInvoker, ITriggerCommand closeCommandNotifier) : base(uiInvoker)
        {
            CloseUICommand = new UICommand(closeCommandNotifier);
        }
    }
}
