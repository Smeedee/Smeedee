using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;
using Smeedee.Client.Framework.Tests;
using Smeedee.DomainModel.Holidays;
using Smeedee.Widget.ProjectInfo.ViewModels;
using Smeedee.Widget.ProjectInfoTests.Controllers.WorkingDaysLeftViewModelSpecs;
using TinyBDD.Specification.NUnit;
using TinyMVVM.Framework;

namespace Smeedee.Widget.ProjectInfoTests.WorkingDaysLeftViewModelSpecs
{
    [TestFixture]
    public class Shared
    {
        protected WorkingDaysLeftViewModel _viewModel;
    }

    [TestFixture]
    class When_setting_end_date : Shared
    {
        [SetUp]
        public void Setup()
        {
            var noHolidays = new List<Holiday>();
            _viewModel = new WorkingDaysLeftViewModel();
            _viewModel.Holidays = noHolidays;
        }


        [Test]
        public void Assure_days_remaining_is_calculated_correctly_when_asked()
        {
            _viewModel.EndDate = DateTime.Now.AddDays(9);
            var nonWorkingDays = new List<DayOfWeek>();
            _viewModel.UpdateDaysRemaining(nonWorkingDays);
            _viewModel.DaysRemaining.ShouldBe("10");
        }

        [Test]
        public void Assure_propertyChanged_is_fired_for_textFields()
        {
            _viewModel.PropertyChangeRecorder.Start();
            _viewModel.EndDate = DateTime.Now.AddDays(10);
            _viewModel.UpdateDaysRemaining(new List<DayOfWeek>());

            var changed = _viewModel.PropertyChangeRecorder.Data;
            changed.Any(p => p.PropertyName == "EndDate").ShouldBeTrue();
            changed.Any(p => p.PropertyName == "DaysRemainingTextSmall").ShouldBeTrue();
            changed.Any(p => p.PropertyName == "DaysRemainingTextLarge").ShouldBeTrue();
            changed.Any(p => p.PropertyName == "DaysRemaining").ShouldBeTrue();
        }

        [Test]
        public void Assure_single_day_left_uses_singluar_text()
        {
            _viewModel.HasInformationToShow = true;
            _viewModel.EndDate = DateTime.Now.Date.AddMinutes(2);
            _viewModel.UpdateDaysRemaining(new List<DayOfWeek>());
            _viewModel.DaysRemaining.ShouldBe("1");
            _viewModel.DaysRemainingTextLarge.ShouldBe(MessageStrings.WORKING_DAYS_LEFT_SINGULAR_STRING);
        }

        [Test]
        public void Assure_endDate_before_today_shows_overtime()
        {
            _viewModel.HasInformationToShow = true;
            _viewModel.EndDate = DateTime.Now.Date.AddDays(-10);
            _viewModel.UpdateDaysRemaining(new List<DayOfWeek>());
            _viewModel.DaysRemainingTextLarge.ShouldBe(MessageStrings.DAYS_ON_OVERTIME_STRING);
            _viewModel.DaysRemainingTextSmall.ShouldBe(MessageStrings.DAYS_ON_OVERTIME_STRING);
        }

        [Test]
        public void Assure_viewmodel_shows_correct_overtime_day_count()
        {
            _viewModel.EndDate = DateTime.Now.Date.AddDays(10).AddMinutes(2);
            _viewModel.UpdateDaysRemaining(new List<DayOfWeek>());
            _viewModel.DaysRemaining.ShouldBe("11");
        }

        [Test]
        public void Assure_non_working_days_are_taken_into_account()
        {
            _viewModel.Holidays = new List<Holiday> { new Holiday() { Date = DateTime.Now.AddDays(1) } };
            var nonWorkWeekDays = new List<DayOfWeek>
                                      {
                                          DayOfWeek.Monday,
                                          DayOfWeek.Tuesday,
                                          DayOfWeek.Wednesday,
                                          DayOfWeek.Thursday,
                                          DayOfWeek.Friday,
                                          DayOfWeek.Saturday,
                                          DayOfWeek.Sunday
                                      };
            _viewModel.EndDate = DateTime.Now.Date.AddDays(40);
            _viewModel.UpdateDaysRemaining(nonWorkWeekDays);
            _viewModel.DaysRemaining.ShouldBe("0");
        }
    }


    [TestFixture]
    public class when_spawned : Shared
    {
        [SetUp]
        public void Setup()
        {
            _viewModel = new WorkingDaysLeftViewModel();
        }

        [Test]
        public void Assure_holidayList_is_initialized()
        {
            _viewModel.Holidays.ShouldNotBeNull();
        }
    }
}

