using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

using APD.Client.Framework;
using APD.Client.Framework.ViewModels;


namespace APD.Client.Widget.Admin.ViewModels
{
    public class HolidaysDbViewModel : BindableViewModel<HolidayViewModel>
    {

        private int selectedHolidayIndex;
        public int SelectedHolidayIndex
        {
            get { return selectedHolidayIndex; }
            set
            {
                if (value != selectedHolidayIndex)
                {
                    selectedHolidayIndex = value;
                    TriggerPropertyChanged<HolidaysDbViewModel>(vm => vm.SelectedHolidayIndex);
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
                    TriggerPropertyChanged<HolidaysDbViewModel>(vm => vm.DataIsChanged);
                }
            }
        }

        public HolidaysDbViewModel(IInvokeUI invoker, ITriggerCommand saveHolidaysUiCommandTrigger, ITriggerCommand reloadHolidaysUiCommandTrigger, ITriggerCommand createNewHolidayTrigger, ITriggerCommand deleteSelectedTrigger)
            : base(invoker)
        {
            SaveHolidaysUICommand = new UICommand( saveHolidaysUiCommandTrigger );
            ReloadHolidaysUICommand = new UICommand( reloadHolidaysUiCommandTrigger );
            CreateNewHolidayUICommand = new UICommand(createNewHolidayTrigger);
            DeleteSelectedHolidayUICommand = new UICommand(deleteSelectedTrigger);

            selectedHolidayIndex = -1;

            Data.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Data_CollectionChanged);
        }

        void Data_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            DataIsChanged = true;
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (HolidayViewModel item in e.NewItems)
                {
                    item.PropertyChanged += new PropertyChangedEventHandler(item_PropertyChanged);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (HolidayViewModel item in e.OldItems)
                {
                    item.PropertyChanged -= new PropertyChangedEventHandler(item_PropertyChanged);
                }
            }
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            DataIsChanged = true;
        }

        public ICommand SaveHolidaysUICommand { get; set; }
        public ICommand ReloadHolidaysUICommand { get; set; }
        public ICommand CreateNewHolidayUICommand { get; set; }
        public ICommand DeleteSelectedHolidayUICommand { get; set; }
        
    }

    public class HolidayViewModel : AbstractViewModel
    {
        private DateTime date;
        public DateTime Date
        {
            get { return date; }
            set
            {
                if( value != date )
                {
                    date = value;
                    TriggerPropertyChanged<HolidayViewModel>(vm => vm.Date);
                }
            }
        }

        private string description;
        public string Description
        {
            get { return description; }
            set
            {
                if( value != description )
                {
                    description = value;
                    TriggerPropertyChanged<HolidayViewModel>(vm => vm.Description);
                }
            }
        }

        public HolidayViewModel(IInvokeUI invoker)
            : base(invoker)
        {
            
        }
    }
}
