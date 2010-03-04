using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using APD.Client.Framework;
using APD.Client.Framework.Controllers;
using APD.Client.Widget.Admin.ViewModels;
using APD.DomainModel.Framework;
using APD.DomainModel.Holidays;


namespace APD.Client.Widget.Admin.Controllers
{
    public class HolidaysDbController
        : ControllerBase<HolidaysDbViewModel>
    {
        private IRepository<Holiday> holidaysRepository;
        private IInvokeBackgroundWorker<IEnumerable<Holiday>> asyncInvoker;

        public HolidaysDbController(INotifyWhenToRefresh refreshNotifier, IInvokeUI uiInvoker, IRepository<Holiday> holidaysRepository, HolidaysDbViewModel viewModel, IInvokeBackgroundWorker<IEnumerable<Holiday>> asyncInvoker)
            : base(refreshNotifier, uiInvoker, viewModel)
        {
            this.asyncInvoker = asyncInvoker;
            this.holidaysRepository = holidaysRepository;
            
            LoadData();
        }

        protected override void OnNotifiedToRefresh(object sender, RefreshEventArgs e)
        {
            LoadData();   
        }

        private void LoadData()
        {
            if( !ViewModel.IsLoading )
            {
                asyncInvoker.RunAsyncVoid(() =>
                {
                    ViewModel.IsLoading = true;
                    var holidaysFromDb = holidaysRepository.Get(new AllSpecification<Holiday>());

                    ObservableCollection<HolidayViewModel> holidayViewModels = new ObservableCollection<HolidayViewModel>();
                    foreach (var domainModelHoliday in holidaysFromDb)
                    {
                        holidayViewModels.Add(new HolidayViewModel(uiInvoker) { Date = domainModelHoliday.Date, Description = domainModelHoliday.Description });
                    }

                    ViewModel.Data = holidayViewModels;
                    ViewModel.IsLoading = false;
                });
            }
        }
    }
}
