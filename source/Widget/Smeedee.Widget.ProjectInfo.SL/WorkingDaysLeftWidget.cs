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
// The project webpage is located at http://www.smeedee.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.ComponentModel;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Config.SlideConfig;
using Smeedee.Widget.ProjectInfo.SL.Views;
using Smeedee.Widget.ProjectInfo.ViewModels;
using TinyMVVM.Framework;

namespace Smeedee.Widget.ProjectInfo.SL
{
    [WidgetInfo(Name = "Working days left", 
                Description = "A simple widget that shows the amount of actual working days left in a project iteration (or any other dead line). Can use data from Project management tools to set dead line date!",
                Author = "Smeedee team",
                Version = "1.0",
                Tags = new[] { CommonTags.ProjectManagement, CommonTags.Agile, CommonTags.Scrum })]
    public class WorkingDaysLeftWidget : Client.Framework.ViewModel.Widget
    {
        private WorkingDaysLeftController controller;
        private WorkingDaysLeftSettingsViewModel settingsViewModel;

        public WorkingDaysLeftWidget()
        {
            Title = "Working days left";

			var viewModel = GetInstance<WorkingDaysLeftViewModel>();
			settingsViewModel = GetInstance<WorkingDaysLeftSettingsViewModel>();
        	controller = NewController<WorkingDaysLeftController>();

			View = new WorkingDaysLeft { DataContext = viewModel };
			SettingsView = new WorkingDaysLeftSettingsView { DataContext = settingsViewModel };

            PropertyChanged += WorkingDaysLeftSlide_PropertyChanged;
            ConfigurationChanged += (o, e) => controller.SetConfigAndUpdate(Configuration);

            settingsViewModel.Save.ExecuteDelegate = () => SaveSettings.Execute();
            SaveSettings.BeforeExecute += (o,e) => controller.BeforeSave();
        }

        private void WorkingDaysLeftSlide_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsInSettingsMode"))
            {
                if (IsInSettingsMode)
                {
                    settingsViewModel.Reset();
                    controller.Stop();
                }
                else
                {
                    controller.Start();
                }
            }
        }

		public override void Configure(DependencyConfigSemantics config)
		{
			config.Bind<WorkingDaysLeftSettingsViewModel>().To<WorkingDaysLeftSettingsViewModel>().InSingletonScope();
			config.Bind<WorkingDaysLeftViewModel>().To<WorkingDaysLeftViewModel>().InSingletonScope();
		}

        protected override Configuration NewConfiguration()
        {
            return WorkingDaysLeftController.GetDefaultConfiguration();
        }
    }
}