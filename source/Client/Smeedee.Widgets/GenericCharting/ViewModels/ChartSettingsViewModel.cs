using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Smeedee.Widgets.GenericCharting.ViewModels
{
    public partial class ChartSettingsViewModel
    {
        partial void OnInitialize()
        {
            Databases = new ObservableCollection<string>();
        }
    }

}
