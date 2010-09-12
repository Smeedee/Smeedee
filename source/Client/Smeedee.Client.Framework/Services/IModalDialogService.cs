using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Client.Framework.ViewModel.Dialogs;

namespace Smeedee.Client.Framework.Services
{
	public interface IModalDialogService
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dialog"></param>
        /// <returns>
        /// DialogResult
        /// 
        /// true == yes/ok
        /// false == no/cancel
        /// </returns>
		void Show(Dialog dialog, Action<bool> dialogClosedCallback);
	}
}
