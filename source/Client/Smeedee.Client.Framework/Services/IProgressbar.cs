using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Smeedee.Client.Framework.ViewModel;

namespace Smeedee.Client.Framework.Services
{
    public interface IProgressbar
    {
        Widget WidgetToShowNotificationFor { get; set; }
        
        void ShowInView(string messageToShow);
        void HideInView();

        void ShowInSettingsView(string messageToShow);
        void HideInSettingsView();

        void ShowInBothViews(string messageToShow);
        void HideInBothViews();
    }
}
