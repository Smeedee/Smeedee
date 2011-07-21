using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Smeedee.Widgets.GenericCharting.Controllers;

namespace Smeedee.Widgets.GenericCharting.ViewModels
{
    public partial class ChartSettingsViewModel
    {
        partial void OnInitialize()
        {
            Databases = new ObservableCollection<string>();
            Collections = new ObservableCollection<string>();
            SeriesConfig = new ObservableCollection<SeriesConfigViewModel>();

            XAxisTypes = new ObservableCollection<string>{ChartConfig.DATETIME, ChartConfig.CATEGORY, ChartConfig.LINEAR};
        }


    }

    public class SettingsChoices
    {
        public ObservableCollection<string> Actions
        {
            get { return new ObservableCollection<string> {ChartConfig.SHOW, ChartConfig.HIDE, ChartConfig.REFERENCE, ChartConfig.REMOVE}; }
        }

        public ObservableCollection<string> ChartTypes
        {
            get { return new ObservableCollection<string> { ChartConfig.LINE, ChartConfig.AREA, ChartConfig.COLUMNS }; }
        }
    }

}
