using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smeedee.Client.Framework.ViewModel;

namespace Smeedee.Client.Framework.Services.Impl
{
	public class ProgressbarDummy : IProgressbar
	{
		public Widget WidgetToShowNotificationFor
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public void ShowInView(string messageToShow)
		{
		}

		public void HideInView()
		{
		}

		public void ShowInSettingsView(string messageToShow)
		{
		}

		public void HideInSettingsView()
		{
		}

		public void ShowInBothViews(string messageToShow)
		{
		}

		public void HideInBothViews()
		{
		}
	}
}
