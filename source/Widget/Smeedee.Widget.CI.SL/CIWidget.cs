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
// The project webpage is located at http://www.smeedee.org
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System.ComponentModel;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config.SlideConfig;
using Smeedee.Widget.CI.Controllers;
using Smeedee.Widget.CI.SL.Views;
using Smeedee.Widget.CI.ViewModels;
using TinyMVVM.Framework;

namespace Smeedee.Widget.CI.SL
{
    [WidgetInfo(Name = "Build status",
                Description = "This widget shows all the projects on any configured build server and their current status (broken, building or green). It also displays name and picture of the person who triggered the current build.",
                Author = "Smeedee team",
                Version = "1.0",
                Tags = new[] { CommonTags.TeamCommunication, CommonTags.ContinuousIntegration, CommonTags.Agile })
    ]
    public class CIWidget : Client.Framework.ViewModel.Widget
    {
        private readonly CIController controller;

        public CIWidget()
        {
            Title = "Build Status";

            var viewModel = GetInstance<CIViewModel>();
			var settingsViewModel = GetInstance<CISettingsViewModel>();

        	controller = NewController<CIController>();

			View = new CIView{ DataContext = viewModel };
			SettingsView = new CISettingsView{ DataContext = settingsViewModel };

            PropertyChanged += controller.ToggleRefreshInSettingsMode;
            controller.ViewModel.PropertyChanged += ViewModelPropertyChanged;
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
			config.Bind<CIViewModel>().To<CIViewModel>().InSingletonScope();
			config.Bind<CISettingsViewModel>().To<CISettingsViewModel>().InSingletonScope();
		}
    }
}