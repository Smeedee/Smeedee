using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Smeedee.Client.Framework.Services;
using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.Users;
using Smeedee.Widget.CI.ViewModels;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widget.CI.Controllers
{
    class ProductivityCalendarController : CIControllerBase<ProductivityCalendarViewModel>
    {
        public ProductivityCalendarController(ProductivityCalendarViewModel viewModel, 
			IRepository<CIServer> ciProjectRepository, 
			IRepository<User> userRepository, 
			IInvokeBackgroundWorker<IEnumerable<CIProject>> asyncClient, 
			ITimer timer, 
			IUIInvoker uiInvoke, 
			ILog logger,
            IProgressbar progressbar)
            : base(viewModel, ciProjectRepository, userRepository, asyncClient, timer, uiInvoke, logger, progressbar)
        {
            //LoadBasicDateIntoViewModel();
            viewModel.Update.ExecuteDelegate = () => LoadBasicDateIntoViewModel();
        }

        protected override void LoadDataIntoViewModel(IEnumerable<CIProject> projects)
        {
            var projectList = projects.ToList();
            foreach (var ciProject in projectList)
            {
                LoadDataForProjectIntoViewModel(ciProject);
            }
        }

        private void LoadDataForProjectIntoViewModel(CIProject ciProject)
        {
        }

        private void LoadBasicDateIntoViewModel()
        {
            DateTime today = DateTime.Now;
            var lastDayOfMonth = GetLastDayOfMonth(today);

            DateTime dateTimeToAdd;
            var currentMonth = today.Month;
            dateTimeToAdd = lastDayOfMonth;

            List<ProductivityCalendarDayViewModel> basicData = new List<ProductivityCalendarDayViewModel>();

            while(dateTimeToAdd.Month == currentMonth)
            {
                ProductivityCalendarDayViewModel viewModelToAdd = new ProductivityCalendarDayViewModel();
                viewModelToAdd.Date = dateTimeToAdd;
                basicData.Add(viewModelToAdd);
                dateTimeToAdd = dateTimeToAdd.AddDays(-1.0);
            }

            ViewModel.Data = new ObservableCollection<ProductivityCalendarDayViewModel>(basicData); 
        }

        private DateTime GetLastDayOfMonth(DateTime today)
        {
            DateTime lastDayOfMonth = DateTime.Today;
            var currentMonth = today.Month;
            
            while(lastDayOfMonth.Month == currentMonth)
            {
                lastDayOfMonth = lastDayOfMonth.AddDays(1.0);
            }
            return lastDayOfMonth.AddDays(-1.0);
        }
    }
}
