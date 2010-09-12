using System;
using System.Windows;
using System.Windows.Controls;
using Smeedee.Client.Framework.ViewModel.Dialogs;

namespace Smeedee.Client.Framework.SL.Views.Dialogs
{
	public partial class ModalDialog : ChildWindow
	{
		public ModalDialog()
		{
			InitializeComponent();

            this.Loaded += ModalDialog_Loaded;
		}

        protected override void OnClosed(System.EventArgs e)
        {
            DisconnectView();

            base.OnClosed(e);
        }

        /// <summary>
        /// View must be disconnected before the dialog is closed. 
        /// Else the View on the Dialog will closed by the Silverlight
        /// runtime. Then it can't be displayed anywhere else.
        /// </summary>
	    private void DisconnectView()
	    {
	        DataContext = new Dialog();
	    }

	    void ModalDialog_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.CloseDialog += ViewModel_CloseDialog;
        }

        private Dialog ViewModel
        {
            get { return DataContext as Dialog; }
        }

        void ViewModel_CloseDialog(object sender, CloseDialogEventArgs e)
        {
            this.DialogResult = e.DialogResult;
        }
	}
}

