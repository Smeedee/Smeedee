using System;
using Smeedee.Client.Framework.ViewModel;

namespace Smeedee.Widget.CI.ViewModels
{
    class ProductivityCalendarDayViewModel : AbstractViewModel
    {
        private DateTime date;
        public DateTime Date
        {
            get
            {
                return date;
            }
            set
            {
                if(value != date)
                {
                    date = value;
                    TriggerPropertyChanged<ProductivityCalendarDayViewModel>(t => t.Date);
                }
            }
        }

        private BuildStatus status;
        public BuildStatus Status
        {
            get
            {
                return status;
            }
            set
            {
                if(value != status)
                {
                    status = value;
                    TriggerPropertyChanged<ProductivityCalendarDayViewModel>(t => t.Status);
                }
            }
        }
    }
}