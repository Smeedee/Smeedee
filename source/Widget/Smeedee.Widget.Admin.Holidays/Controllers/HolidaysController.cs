using System;
using System.Collections.Generic;
using System.Linq;
using Smeedee.Client.Framework.Controller;
using Smeedee.Client.Framework.Services;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Holidays;
using Smeedee.Framework;
using Smeedee.Widget.Admin.Holidays.ViewModels;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widget.Admin.Holidays.Controllers
{
    public class HolidaysController: ControllerBase<HolidaysViewModel>
    {
        private readonly IAsyncRepository<Holiday> holidaysRepository;
        private readonly IPersistDomainModelsAsync<Holiday> holidaysPersister;
        private string DEFAULT_HOLIDAY_DESCRIPTION = "New Holiday, enter your description here";

        public HolidaysController(
            IProgressbar progressbar,
            ITimer timer,
            IUIInvoker uiInvoker,
            IAsyncRepository<Holiday> holidaysRepository,
            HolidaysViewModel viewModel,
            IPersistDomainModelsAsync<Holiday> holidaysPersister)
            : base(viewModel, timer, uiInvoker, progressbar)
        {
            Guard.ThrowExceptionIfNull(holidaysPersister, "holidaysPersister");
            Guard.ThrowExceptionIfNull(holidaysRepository, "holidaysRepository");
            
            this.holidaysPersister = holidaysPersister;
            this.holidaysRepository = holidaysRepository;

            holidaysRepository.GetCompleted += OnGetCompleted;
            holidaysPersister.SaveCompleted += OnSaveCompleted;

            ViewModel.SaveSettings.AfterExecute += OnSaveHolidayCmd;
            ViewModel.ReloadFromRepository.AfterExecute += OnReloadCmd;
            ViewModel.DeleteSelectedHoliday.AfterExecute += OnDeleteSelectedHolidayCmd;
            ViewModel.CreateNewHoliday.AfterExecute += OnCreateNewHolidayCmd;

            Start();

            BeginLoadData();
        }

        private void OnSaveCompleted(object sender, SaveCompletedEventArgs eventArgs)
        {
            SetIsNotSavingData();
        }

        private void OnGetCompleted(object sender, GetCompletedEventArgs<Holiday> eventArgs)
        {
            var holidaysFromDb = eventArgs.Result;
            
            var viewModelHolidays = HolidaysFromDomainModelToViewModel(holidaysFromDb);

            UpdateHolidaysInViewModel(viewModelHolidays);

            SetIsNotLoadingData();
        }

        private void OnSaveHolidayCmd(object sender, EventArgs eventArgs)
        {
            var domainModels = new List<Holiday>();

            foreach (var holiday in ViewModel.Data)
            {
                domainModels.Add(new Holiday { Date = holiday.Date, Description = holiday.Description });
            }

            SetIsSavingData();
            holidaysPersister.Save(domainModels);
        }

        private void OnReloadCmd(object sender, EventArgs eventArgs)
        {
            BeginLoadData();
        }

        private void OnDeleteSelectedHolidayCmd(object sender, EventArgs eventArgs)
        {
            uiInvoker.Invoke(() =>
            {
                if (SelectedHolidayIndexIsValid())
                    ViewModel.Data.RemoveAt(ViewModel.SelectedHolidayIndex);
            });
        }

        private bool SelectedHolidayIndexIsValid()
        {
            return ViewModel.SelectedHolidayIndex >= 0 && ViewModel.SelectedHolidayIndex < ViewModel.Data.Count;
        }

        private void OnCreateNewHolidayCmd(object sender, EventArgs eventArgs)
        {
            var nextDate = (ViewModel.Data.Count > 0) ? ViewModel.Data.Max(h => h.Date).AddDays(1) : DateTime.Today.Date;

            var holidayToAdd = new SingleHolidayViewModel {Date = nextDate, Description = DEFAULT_HOLIDAY_DESCRIPTION};

            uiInvoker.Invoke(() => ViewModel.Data.Add(holidayToAdd));
        }

        private void BeginLoadData()
        {
            if (!ViewModel.IsLoading)
            {
                    SetIsLoadingData();
                    
                    holidaysRepository.BeginGet(new AllSpecification<Holiday>());
            }
        }

        private static IEnumerable<SingleHolidayViewModel> HolidaysFromDomainModelToViewModel(IEnumerable<Holiday> domainModelList)
        {
            return domainModelList.Select(holiday => 
                new SingleHolidayViewModel {Date = holiday.Date, Description = holiday.Description}).ToList();
        }

        private void UpdateHolidaysInViewModel(IEnumerable<SingleHolidayViewModel> viewModelHolidays)
        {
            uiInvoker.Invoke(() =>
            {
                ViewModel.Data.Clear();

                if(ContainsHolidays(viewModelHolidays))
                {
                    foreach (var holiday in viewModelHolidays)
                        ViewModel.Data.Add(holiday);
                }
            });
        }

        private static bool ContainsHolidays(IEnumerable<SingleHolidayViewModel> viewModelHolidays)
        {
            return viewModelHolidays != null && viewModelHolidays.Count() != 0;
        }

        protected override void OnNotifiedToRefresh(object sender, EventArgs e) {}

    }
}
