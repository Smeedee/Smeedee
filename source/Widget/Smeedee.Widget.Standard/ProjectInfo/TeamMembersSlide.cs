using System;
using System.ComponentModel.Composition;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Client.Web.Services;
using Smeedee.Widget.Standard.ProjectInfo.Views;

namespace Smeedee.Widget.Standard.ProjectInfo
{
    public class TeamMembersSlide : Slide
    {
        public TeamMembersSlide()
        {
            Title = "Team Members";

            var userdbContext = new UserdbContext();
            userdbContext.Load(userdbContext.GetUserQuery());

            var view = new TeamMembersView()
            {
                DataContext = userdbContext
            };

            var settingsView = new UserdbSettingsView()
            {
                DataContext = userdbContext
            };
            settingsView.userDomainDataSource.SubmittedChanges += (o, e) =>
            {
                view.userDomainDataSource.Load();
            };

            SettingsView = settingsView;
            View = view;
        }
    }
}
