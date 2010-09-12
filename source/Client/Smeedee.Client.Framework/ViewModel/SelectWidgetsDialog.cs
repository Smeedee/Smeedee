using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
#if SILVERLIGHT
using Smeedee.Client.Framework.SL.Views.Dialogs;
#endif

namespace Smeedee.Client.Framework.ViewModel
{
    public partial class SelectWidgetsDialog
    {
        public void OnInitialize()
        {
            Title = "Select Widgets";

            AvailableWidgets = new ObservableCollection<WidgetMetadata>();
            Progressbar = new Progressbar();

#if SILVERLIGHT
            View = new SelectWidgetsDialogView();
#endif

            PropertyChanged +=
                (o, e) => { if (e.PropertyName == "SearchTerm") TriggerPropertyChanged("FilteredWidgets"); }; 

        }

        public IEnumerable<WidgetMetadata> FilteredWidgets
        {
            get
            {
                if (string.IsNullOrEmpty(SearchTerm))
                    return AvailableWidgets;
                else
                    return AvailableWidgets.Where(w => w.Name.ToLower().Contains(SearchTerm.ToLower()));
            }
        }

        public IEnumerable<WidgetMetadata> SelectedWidgets
        {
            get
            {
                return from widgetMetadata in AvailableWidgets
                       where widgetMetadata.IsSelected
                       select widgetMetadata;
            }
        }

        public void OnSelectAll()
        {
            foreach (var widgetMetadata in AvailableWidgets)
            {
                widgetMetadata.IsSelected = true;
            }
        }

        public void OnDeselectAll()
        {
            foreach (var widgetMetadata in AvailableWidgets)
            {
                widgetMetadata.IsSelected = false;
            }
        }

        public void OnSearch()
        {
            TriggerPropertyChanged("FilteredWidgets");
        }
    }
}
