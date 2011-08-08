using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Smeedee.Client.Framework.ViewModel;

namespace Smeedee.Widget.Admin.Tasks.ViewModels
{
    public class ConfigurationEntryViewModel : BasicViewModel
    {
        public event ChangeConfig ConfigChanged;

        public ConfigurationEntryViewModel()
        {
            PropertyChanged += ConfigurationEntryViewModel_PropertyChanged;
        }

        private void ConfigurationEntryViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnChange();
        }

        private void OnChange()
        {
            if (ConfigChanged != null)
                ConfigChanged(this, new EventArgs());
        }

        //State
        public int OrderIndex { get; set; }

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (value != _Name)
                {
                    _Name = value;
                    TriggerPropertyChanged<ConfigurationEntryViewModel>(vm => vm.Name);
                }
            }
        }
        private string _Name;

        public Type Type
        {
            get
            {
                return _Type;
            }
            set
            {
                if (value != _Type)
                {
                    _Type = value;
                    TriggerPropertyChanged("Type");
                }
            }
        }
        private Type _Type;


        public object Value
        {
            get
            {
                return _Value;
            }
            set
            {
                if (value != _Value)
                {
                    _Value = value;
                    TriggerPropertyChanged("Value");
                }
            }
        }
        private object _Value;



        public string HelpText
        {
            get
            {
                return _HelpText;
            }
            set
            {
                if (value != _HelpText)
                {
                    _HelpText = value;
                    TriggerPropertyChanged("HelpText");
                }
            }
        }
        private string _HelpText;
    }

    public delegate void ChangeConfig(object sender, EventArgs args);
}
