using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Smeedee.Widget.Admin.Tasks.ViewModels
{
    partial class ConfigurationEntryViewModel
    {
        public event ChangeConfig ConfigChanged;

        partial void OnInitialize()
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
    }

    public delegate void ChangeConfig(object sender, EventArgs args);
}
