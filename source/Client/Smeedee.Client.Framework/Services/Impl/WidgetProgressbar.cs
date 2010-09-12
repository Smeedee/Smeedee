using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Framework;
using TinyMVVM.Framework.Services;

namespace Smeedee.Client.Framework.Services.Impl
{
	public class WidgetProgressbar : IProgressbar
	{
		private Progressbar settingsProgressbar;
		private Progressbar viewProgressbar;
		private IUIInvoker uiInvoker;

		public WidgetProgressbar(IUIInvoker uiInvoker, Progressbar viewProgressbar, Progressbar settingsProgressbar)
		{
			Guard.Requires<ArgumentException>(uiInvoker != null);
			Guard.Requires<ArgumentException>(viewProgressbar != null);
			Guard.Requires<ArgumentException>(settingsProgressbar != null);

			this.uiInvoker = uiInvoker;
			this.viewProgressbar = viewProgressbar;
			this.settingsProgressbar = settingsProgressbar;
		}

		public Widget WidgetToShowNotificationFor
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public void ShowInView(string messageToShow)
		{
			uiInvoker.Invoke(() =>
			{
				viewProgressbar.IsVisible = true;
				viewProgressbar.Message = messageToShow;
			});
		}

		public void HideInView()
		{
			uiInvoker.Invoke(() =>
			{
				viewProgressbar.IsVisible = false;
				viewProgressbar.Message = string.Empty;
			});
		}

		public void ShowInSettingsView(string messageToShow)
		{
			uiInvoker.Invoke(() =>
			{
				settingsProgressbar.IsVisible = true;
				settingsProgressbar.Message = messageToShow;
			});
		}

		public void HideInSettingsView()
		{
			uiInvoker.Invoke(() =>
			{
				settingsProgressbar.IsVisible = false;
				settingsProgressbar.Message = string.Empty;
			});
		}

        public void ShowInBothViews(string messageToShow)
        {
            uiInvoker.Invoke(() =>
            {
                viewProgressbar.Message = messageToShow;
                viewProgressbar.IsVisible = true;
                settingsProgressbar.Message = messageToShow;
                settingsProgressbar.IsVisible = true;
            });
        }

        public void HideInBothViews()
        {
            uiInvoker.Invoke(() =>
            {
                viewProgressbar.IsVisible = false;
                viewProgressbar.Message = string.Empty;
                settingsProgressbar.IsVisible = false;
                settingsProgressbar.Message = string.Empty;
            });
        }
	}
}
