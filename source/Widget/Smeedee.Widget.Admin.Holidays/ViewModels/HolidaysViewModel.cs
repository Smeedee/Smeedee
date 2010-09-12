using System;
using System.Collections.Specialized;
using System.ComponentModel;
using Smeedee.Client.Framework.ViewModel;
using TinyMVVM.Framework;

namespace Smeedee.Widget.Admin.Holidays.ViewModels
{
    public class HolidaysViewModel : BindableViewModel<SingleHolidayViewModel>
    {
        public HolidaysViewModel()
        {
            SetupDelegateCommands();

            selectedHolidayIndex = -1;

            Data.CollectionChanged += Data_CollectionChanged;
        }

        private int selectedHolidayIndex;
        public int SelectedHolidayIndex
        {
            get { return selectedHolidayIndex; }
            set
            {
                if (value != selectedHolidayIndex)
                {
                    selectedHolidayIndex = value;
                    TriggerPropertyChanged<HolidaysViewModel>(vm => vm.SelectedHolidayIndex);
                }
            }
        }

        private bool dataIsChanged;
        public bool DataIsChanged
        {
            get { return dataIsChanged; }
            set
            {
                if (value != dataIsChanged)
                {
                    dataIsChanged = value;
                    TriggerPropertyChanged<HolidaysViewModel>(vm => vm.DataIsChanged);
                }
            }
        }

        void Data_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DataIsChanged = true;
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (SingleHolidayViewModel item in e.NewItems)
                {
                    item.PropertyChanged += item_PropertyChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (SingleHolidayViewModel item in e.OldItems)
                {
                    item.PropertyChanged -= item_PropertyChanged;
                }
            }
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            DataIsChanged = true;
        }

        public DelegateCommand CreateNewHoliday { get; set; }
        public DelegateCommand DeleteSelectedHoliday { get; set; }


        private void SetupDelegateCommands()
        {
            CreateNewHoliday = new DelegateCommand();
            DeleteSelectedHoliday = new DelegateCommand();
        }

    }

    public class SingleHolidayViewModel : AbstractViewModel
    {
        private DateTime date;
        public DateTime Date
        {
            get { return date; }
            set
            {
                if (value != date)
                {
                    date = value;
                    TriggerPropertyChanged<SingleHolidayViewModel>(vm => vm.Date);
                }
            }
        }

        private string description;
        public string Description
        {
            get { return description; }
            set
            {
                if (value != description)
                {
                    description = value;
                    TriggerPropertyChanged<SingleHolidayViewModel>(vm => vm.Description);
                }
            }
        }
    }
}
