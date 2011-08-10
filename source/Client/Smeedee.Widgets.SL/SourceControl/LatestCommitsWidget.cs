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

using System.ComponentModel;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config.SlideConfig;
using Smeedee.Widgets.SL.SourceControl.Views;
using Smeedee.Widgets.SourceControl.Controllers;
using Smeedee.Widgets.SourceControl.ViewModels;
using TinyMVVM.Framework;

namespace Smeedee.Widgets.SL.SourceControl
{
    [WidgetInfo(Name = "Latest commits",
                Description = "A simple widget that shows the latest commits in any configured source control system.",
                Author = "Smeedee team",
                Version = "1.1",
                Tags = new[] { CommonTags.SourceControl, CommonTags.TeamCommunication, CommonTags.VCS, CommonTags.Agile })]
    public class LatestCommitsWidget : Client.Framework.ViewModel.Widget
    {
        public LatestCommitsWidget()
        {
            Title = "Latest commits";

        	var viewModel = GetInstance<LatestCommitsViewModel>();
            viewModel.PropertyChanged += ViewModelPropertyChanged;

			var controller = NewController<LatestCommitsController>();
			PropertyChanged += controller.ToggleRefreshInSettingsMode;

			View = new LatestCommitsView() { DataContext = viewModel };
			SettingsView = new LatestCommitsSettingsView() { DataContext = viewModel };
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
			config.Bind<LatestCommitsViewModel>().To<LatestCommitsViewModel>().InSingletonScope();
		}
    }
}
