using System;
using System.ComponentModel;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config.SlideConfig;
using Smeedee.Widget.Admin.Users.Controllers;
using Smeedee.Widget.Admin.Users.SL.Views;
using Smeedee.Widget.Admin.Users.ViewModels;
using TinyMVVM.Framework;

namespace Smeedee.Widget.Admin.Users.SL
{
    [WidgetInfo(Name = "User Administration",
            Description = "Administrates users and user databases",
            Author = "Smeedee team",
            Version = "1.0",
            Tags = new[] { CommonTags.ProjectManagement, CommonTags.TeamBuilding, CommonTags.TeamCommunication })]
    public class UsersWidget : Client.Framework.ViewModel.Widget
    {
        private UsersViewModel viewModel;

        public UsersWidget()
        {
            Title = "User Administration";
            
        	var controller = NewController<UsersController>();
            viewModel = GetInstance<UsersViewModel>();
            View = new UsersView() {DataContext = viewModel};
            viewModel.PropertyChanged += ViewModelPropertyChanged;
            this.PropertyChanged += UsersWidgetPropertyChanged;
        }

        private void UsersWidgetPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsInSettingsMode") && IsInSettingsMode)
            {
                viewModel.Refresh.Execute();
            }
        }

        private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var viewModel = sender as UsersViewModel;
            var isDoneSaving = (viewModel != null && e.PropertyName.Equals("IsSaving") && !viewModel.IsSaving);

            if (isDoneSaving && IsInSettingsMode)
                OnSettings();
        }

        public override void Configure(DependencyConfigSemantics config)
        {
            config.Bind<UsersViewModel>().To<UsersViewModel>().InSingletonScope();
        }
    } 
}
