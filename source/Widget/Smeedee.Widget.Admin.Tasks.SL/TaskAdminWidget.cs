using System;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Widget.Admin.Tasks.Controllers;
using Smeedee.Widget.Admin.Tasks.SL.Views;
using Smeedee.Widget.Admin.Tasks.ViewModels;
using TinyMVVM.Framework;

namespace Smeedee.Widget.Admin.Tasks.SL
{
    [WidgetInfo(Name = "Task Administration")]
    public class TaskAdminWidget : Client.Framework.ViewModel.Widget
    {
        private TasksController controller;

        public TaskAdminWidget()
        {
            Title = "Task Administration";

            var viewModel = GetInstance<TasksViewModel>();

			controller = NewController<TasksController>();
            controller.Start();

            //View = new TaskFrontPage();
            View = new TasksView { DataContext = viewModel };

            viewModel.SaveChanges.AfterExecute += ToggleSettings;
        }

		public override void Configure(DependencyConfigSemantics config)
		{
			config.Bind<TasksViewModel>().To<TasksViewModel>().InSingletonScope();
		}
    }
}