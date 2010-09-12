using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Smeedee.Client.Framework.Tests;
using Smeedee.DomainModel.Holidays;
using Smeedee.Widget.ProjectInfo.ViewModels;
using Smeedee.Widget.ProjectInfoTests.Controllers.WorkingDaysLeftViewModelSpecs;
using TinyBDD.Specification.NUnit;
using TinyMVVM.Framework;
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework.Testing.Services;
using TinyMVVM.IoC;

namespace Smeedee.Client.Widget.ProjectInfoTests.ViewModel.WorkingDaysLeftSettingsViewModelSpecs
{
    [TestFixture]
    public class when_spawned
    {
        [Test]
        public void assure_viewmodel_is_initialized()
        {
			RemoveAllGlobalDependencies.ForAllViewModels();
			ConfigureGlobalDependencies.ForAllViewModels(config =>
				config.Bind<IUIInvoker>().To<UIInvokerForTesting>());

            var vm = new WorkingDaysLeftSettingsViewModel();
            vm.IsManuallyConfigured.ShouldBe(true);
            vm.SelectedEndDate.ShouldBe(DateTime.Now.Date);
            vm.NonWorkWeekDays.First(d => d.Day == DayOfWeek.Saturday).IsNotWorkingDay.ShouldBe(true);
            vm.NonWorkWeekDays.First(d => d.Day == DayOfWeek.Saturday).IsNotWorkingDay.ShouldBe(true);
            vm.AvailableServers.ShouldNotBeNull();
            vm.AvailableProjects.ShouldNotBeNull();
        }

    }
}
