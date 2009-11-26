using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using APD.Client.Framework;


namespace APD.Client.Widget.Admin.ViewModels.Configuration
{
    public class GenericConfigItemViewModel : ConfigurationItemViewModel
    {
        public ObservableCollection<SettingViewModel> Data { get; set; }

        public GenericConfigItemViewModel(IInvokeUI uiInvoker) :
            base(uiInvoker)
        {
            Data = new ObservableCollection<SettingViewModel>();
        }
    }
}
