using Smeedee.Client.Framework.Services;

namespace Smeedee.Client.Framework.Messages
{
	/// <summary>
	/// This message is published in the EventAggregator 
	/// when a ModalDialog is displayed
	/// </summary>
	public class OpenModalDialogMessage : MessageBase
	{
		public OpenModalDialogMessage(object sender) : base(sender)
		{
			
		}
	}
}
