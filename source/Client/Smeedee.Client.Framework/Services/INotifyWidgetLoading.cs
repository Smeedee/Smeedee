using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Smeedee.Client.Framework.ViewModel;

namespace Smeedee.Client.Framework.Services
{
    public interface INotifyWidgetLoading
    {
        Widget WidgetToShowNotificationFor { get; set; }
        
        void ShowNotificationOnView(string messageToShow);
        void HideNotificationFromView();

        void ShowNotificationOnSettingsView(string messageToShow);
        void HideNotificationFromSettingsView();
    }
}
