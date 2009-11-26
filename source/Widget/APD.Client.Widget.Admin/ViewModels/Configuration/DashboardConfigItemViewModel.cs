using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APD.Client.Framework;
using System.Collections.ObjectModel;

namespace APD.Client.Widget.Admin.ViewModels.Configuration
{
    public class DashboardConfigItemViewModel : ConfigurationItemViewModel
    {
        public ObservableCollection<string> SlideWidgets { get; set; }
        public ObservableCollection<string> SelectedSlideWidgets { get; set; }
        public ObservableCollection<string> TrayWidgets { get; set; }
        public ObservableCollection<string> SelectedTrayWidgets { get; set; }

        public DashboardConfigItemViewModel(IInvokeUI uiInvoker) :
            base(uiInvoker)
        {
            SlideWidgets = new ObservableCollection<string>();
            SelectedSlideWidgets = new ObservableCollection<string>();
            TrayWidgets = new ObservableCollection<string>();
            SelectedTrayWidgets = new ObservableCollection<string>();
        }
    }
}
