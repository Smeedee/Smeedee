using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Smeedee.Client.Framework.Messages;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.SL.Views.Dialogs;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Client.Framework.ViewModel.Dialogs;

namespace Smeedee.Client.Framework.SL.Services.Impl
{
	public class SLModalDialogService : IModalDialogService
	{
		private IEventAggregator eventAggregator;

		public SLModalDialogService(IEventAggregator eventAggregator)
		{
			this.eventAggregator = eventAggregator;
		}

		public void Show(Dialog dialog, Action<bool> dialogClosedCallback)
		{
			eventAggregator.PublishMessage(new OpenModalDialogMessage(this));
		    var modalDialog = new ModalDialog();
		    modalDialog.DataContext = dialog;
			modalDialog.Show();
            modalDialog.Closed += (o, e) =>
            {
                var dialogResult = false;
                if (modalDialog.DialogResult.HasValue)
                    dialogResult = (bool) modalDialog.DialogResult;

                dialogClosedCallback.Invoke(dialogResult);
				eventAggregator.PublishMessage(new CloseModalDialogMessage(this));
            };
		}
	}
}
