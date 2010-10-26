using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smeedee.Client.Framework.Services;

namespace Smeedee.Client.Framework.Messages
{
	/// <summary>
	/// This message is published in th EventAggregator when
	/// a ModalDialog is closed.
	/// </summary>
	public class CloseModalDialogMessage : MessageBase
	{
		public CloseModalDialogMessage(object sender) : base(sender)
		{
			
		}
	}
}
