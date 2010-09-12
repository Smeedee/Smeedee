using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smeedee.Client.Framework.Services;
using TinyMVVM.IoC;

namespace Smeedee.Client.Framework.ViewModel.Dialogs
{
    public partial class Dialog
    {
        partial void OnInitialize()
        {
            Progressbar = new Progressbar();
            Width = 480;
            Height = 300;

            DisplayOkButton = true;
            Ok.Text = "OK";

            DisplayCancelButton = true;
            Cancel.Text = "Cancel";
        }

        public virtual void OnOk()
        {
            OnCloseDialog(true);
        }

        public virtual void OnCancel()
        {
            OnCloseDialog(false);
        }

        protected void OnCloseDialog(bool dialogResult)
        {
            if (CloseDialog != null)
                CloseDialog(this, new CloseDialogEventArgs(){ DialogResult = dialogResult});
        }

        public event EventHandler<CloseDialogEventArgs> CloseDialog;
    }

    public class CloseDialogEventArgs : EventArgs
    {
        public bool DialogResult { get; set; }
    }
}
