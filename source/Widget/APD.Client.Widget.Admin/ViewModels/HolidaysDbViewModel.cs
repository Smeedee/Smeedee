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
        public HolidaysDbViewModel(IInvokeUI invoker, ITriggerCommand saveHolidaysUiCommandTrigger, ITriggerCommand reloadHolidaysUiCommandTrigger)
            : base(invoker)
        {
            SaveHolidaysUICommand = new UICommand( saveHolidaysUiCommandTrigger );
            ReloadHolidaysUICommand = new UICommand( reloadHolidaysUiCommandTrigger );

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

        public bool DataIsChanged { get; set; }
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
