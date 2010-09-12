using System;
using System.ComponentModel;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Config.SlideConfig;
using Smeedee.Widget.TeamMembers.Controllers;
using Smeedee.Widget.TeamMembers.SL.Views;
using Smeedee.Widget.TeamMembers.ViewModels;
using TinyMVVM.Framework;

namespace Smeedee.Widget.TeamMembers.SL
{
    [WidgetInfo(Name = "Team members",
            Description = "Shows a picture and name of all the members on the team",
            Author = "Smeedee team",
            Version = "1.0",
            Tags = new[] { CommonTags.ProjectManagement, CommonTags.TeamBuilding, CommonTags.TeamCommunication })]
    public class TeamMembersWidget : Client.Framework.ViewModel.Widget
    {
        private TeamMembersViewModel viewModel;
        private TeamMembersSettingsViewModel settingsViewModel;
        private TeamMembersController controller;
        
        public TeamMembersWidget()
        {
            Title = "Team Members";
            
        	controller = NewController<TeamMembersController>();
            viewModel = GetInstance<TeamMembersViewModel>();
            settingsViewModel = GetInstance<TeamMembersSettingsViewModel>();

            PropertyChanged += TeamMembersWidget_PropertyChanged;

            View = new TeamMembersView() { DataContext = viewModel };
            SettingsView = new TeamMembersSettingsView() { DataContext = settingsViewModel };

            settingsViewModel.PropertyChanged += ViewModelPropertyChanged;
        }

        private void TeamMembersWidget_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsInSettingsMode"))
            {
                ToggleRefreshInSettingsMode();
            }
        }

        private void ToggleRefreshInSettingsMode()
        {
            if (IsInSettingsMode)
            {
                controller.StopRefreshTimer();
                settingsViewModel.Refresh.Execute();
            }
            else
            {
                controller.StartRefreshTimer();
            }
        }

        private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var viewModel = sender as TeamMembersSettingsViewModel;
            var isDoneSaving = (viewModel != null && e.PropertyName.Equals("IsSaving") && !viewModel.IsSaving);

            if (isDoneSaving && IsInSettingsMode)
                OnSettings();
        }

        public override void Configure(DependencyConfigSemantics config)
        {
            config.Bind<TeamMembersViewModel>().To<TeamMembersViewModel>().InSingletonScope();
            config.Bind<TeamMembersSettingsViewModel>().To<TeamMembersSettingsViewModel>().InSingletonScope();
        }
       
        protected override Configuration NewConfiguration()
        {
            return TeamMembersController.DefaultConfig;
        }
         
    } 
}
