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
        private IPersistDomainModels<Holiday> persister;

        public HolidaysDbController(
            INotifyWhenToRefresh refreshNotifier, 
            IInvokeUI uiInvoker, 
            IRepository<Holiday> holidaysRepository, 
            HolidaysDbViewModel viewModel, 
            IInvokeBackgroundWorker<IEnumerable<Holiday>> asyncInvoker,
            IPersistDomainModels<Holiday> persister,
            INotify<EventArgs> saveNotifier,
            INotify<EventArgs> createNewNotifier,
            INotify<EventArgs> deleteSelectedNotifier
            )
            : base(refreshNotifier, uiInvoker, viewModel)
        {
            this.persister = persister;
            this.asyncInvoker = asyncInvoker;
            this.holidaysRepository = holidaysRepository;

            saveNotifier.NewNotification += new EventHandler<EventArgs>(saveNotifier_NewNotification);
            createNewNotifier.NewNotification += new EventHandler<EventArgs>(createNewNotifier_NewNotification);
            deleteSelectedNotifier.NewNotification += new EventHandler<EventArgs>(deleteSelectedNotifier_NewNotification);

            LoadData();
        }

        void deleteSelectedNotifier_NewNotification(object sender, EventArgs e)
        {
            uiInvoker.Invoke(()=>
            {
                if( ViewModel.SelectedHolidayIndex > -1 && ViewModel.SelectedHolidayIndex < ViewModel.Data.Count )
                {
                    ViewModel.Data.RemoveAt(ViewModel.SelectedHolidayIndex);
                }
            });
        }

        void createNewNotifier_NewNotification(object sender, EventArgs e)
        {
            uiInvoker.Invoke(() =>
            {
                DateTime nextDateInHolidayList = DateTime.Today;
                
                if ( ViewModel.Data.Count > 0 )
                    nextDateInHolidayList =  ViewModel.Data.Max(h=> h.Date).AddDays(1);
                
                ViewModel.Data.Add(new HolidayViewModel(uiInvoker)
                {
                    Date = nextDateInHolidayList,
                    Description = "New Holiday (yay!)"
                });
            });
        }

        void saveNotifier_NewNotification(object sender, EventArgs e)
        {
            List<Holiday> domainModels = new List<Holiday>();
            foreach (HolidayViewModel holidayViewModel in ViewModel.Data)
            {
                domainModels.Add(
                    new Holiday() {Date = holidayViewModel.Date, Description = holidayViewModel.Description});
            }

            persister.Save(domainModels);
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

                    uiInvoker.Invoke(() =>
                    {
                        ExtractViewmodelsFromDomainModelsAndInsert(holidaysFromDb);
                    });

                    ViewModel.IsLoading = false;
                });
            }
        }

        private void ExtractViewmodelsFromDomainModelsAndInsert(IEnumerable<Holiday> holidaysFromDb) 
        {
            ViewModel.Data.Clear();
            foreach (var domainModelHoliday in holidaysFromDb)
            {
                ViewModel.Data.Add(new HolidayViewModel(uiInvoker)
                {Date = domainModelHoliday.Date, Description = domainModelHoliday.Description});
            }
        }
    }
}
