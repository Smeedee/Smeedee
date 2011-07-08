#region File header

// <copyright>
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
// /copyright> 
// 
// <contactinfo>
// The project webpage is located at http://smeedee.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.ComponentModel;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config.SlideConfig;
using Smeedee.Widget.SourceControl.Controllers;
using Smeedee.Widget.SourceControl.SL.Views;
using Smeedee.Widget.SourceControl.ViewModels;
using TinyMVVM.Framework;

namespace Smeedee.Widget.SourceControl.SL
{
    [WidgetInfo(Name = "Commit statistics",
                Description = "Shows a graph of the number of commits to the source control in a given time span.",
                Author = "Smeedee team",
                Version = "1.0",
                Tags = new[] { CommonTags.ProjectManagement, CommonTags.SourceControl, CommonTags.VCS, CommonTags.Agile })]
    public class CommitStatisticsWidget : Client.Framework.ViewModel.Widget
    {
        public CommitStatisticsWidget()
        {
            Title = "Commit Statistics";

			var settingsViewModel = GetInstance<CommitStatisticsSettingsViewModel>();
            settingsViewModel.SaveSettings.AfterExecute += (s, e) => SaveSettings.Execute();

            var viewModel = GetInstance<BindableViewModel<CommitStatisticsForDate>>();
            viewModel.PropertyChanged += ViewModelPropertyChanged;

            var controller = NewController<CommitStatisticsController>();
			PropertyChanged += controller.ToggleRefreshInSettingsMode;

			View = new CommitStatisticsView() { DataContext = viewModel };
			SettingsView = new CommitStatisticsSettingsView { DataContext = settingsViewModel };
        }
        

        private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var viewModel = sender as AbstractViewModel;
            var isDoneSaving = (viewModel != null && e.PropertyName.Equals("IsSaving") && !viewModel.IsSaving);

            if (isDoneSaving && IsInSettingsMode)
                OnSettings();
        }

        public override void Configure(DependencyConfigSemantics config)
		{
			config.Bind<CommitStatisticsSettingsViewModel>().To<CommitStatisticsSettingsViewModel>().InSingletonScope();
			config.Bind<BindableViewModel<CommitStatisticsForDate>>().To<BindableViewModel<CommitStatisticsForDate>>().InSingletonScope();
		}
    }
}