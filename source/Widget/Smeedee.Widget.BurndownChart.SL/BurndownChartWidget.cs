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
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.ComponentModel;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Config.SlideConfig;
using Smeedee.Widget.BurndownChart.Controllers;
using Smeedee.Widget.BurndownChart.SL.Views;
using Smeedee.Widget.BurndownChart.ViewModel;
using Smeedee.Widget.BurndownChart.ViewModels;
using TinyMVVM.Framework;

namespace Smeedee.Widget.BurndownChart.SL
{
	[WidgetInfo(
        Name = "Burndown Chart", 
        Description = "Uses data from a project management tool to show a burndown chart for the current iteration.",
        Author = "Smeedee team",
        Version = "1.0",
        Tags = new[] { CommonTags.ProjectManagement, CommonTags.TeamCommunication, CommonTags.Scrum })
    ]
	public class BurndownChartWidget : Client.Framework.ViewModel.Widget
	{
		private BurndownChartController controller;

		public BurndownChartWidget()
		{
			Title = "Burndown Chart";

			var viewModel = GetInstance<BurndownChartViewModel>();
			var settingsViewModel = GetInstance<BurndownChartSettingsViewModel>();
			controller = NewController<BurndownChartController>();

			View = new BurndownChartView { DataContext = viewModel };
			SettingsView = new BurndownChartSettingsView { DataContext = settingsViewModel };
			PropertyChanged += controller.ToggleRefreshInSettingsMode;

		    ConfigurationChanged += (o, e) => controller.ConfigurationChanged(Configuration);

		    SaveSettings.BeforeExecute += (s, e) => controller.BeforeSaving();
            SaveSettings.AfterExecute += controller.AfterSave;

		    settingsViewModel.Save.ExecuteDelegate += () => SaveSettings.Execute();
		}

	    public override void Configure(DependencyConfigSemantics config)
		{
			config.Bind<BurndownChartViewModel>().To<BurndownChartViewModel>().InSingletonScope();
			config.Bind<BurndownChartSettingsViewModel>().To<BurndownChartSettingsViewModel>().InSingletonScope();
		}

        protected override Configuration NewConfiguration()
        {
            return BurndownChartController.GetDefaultConfiguration();
        }
	}
}