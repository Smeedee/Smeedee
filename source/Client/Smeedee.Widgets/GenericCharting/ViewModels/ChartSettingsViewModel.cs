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
            Collections = new ObservableCollection<string>();
            SeriesConfig = new ObservableCollection<SeriesConfigViewModel>();

            XAxisTypes = new ObservableCollection<string>{"DateTime", "Category", "Linear"};
        }


    }

    public class SettingsChoices
    {
        public ObservableCollection<string> Actions
        {
            get { return new ObservableCollection<string> {"Show", "Hide", "Reference", "Remove"}; }
        }

        public ObservableCollection<string> ChartTypes
        {
            get { return new ObservableCollection<string> { "Line", "Area", "Columns" }; }
        }
    }

}
