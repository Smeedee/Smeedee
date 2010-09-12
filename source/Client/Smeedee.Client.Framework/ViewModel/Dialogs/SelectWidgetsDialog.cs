using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Smeedee.Client.Framework.Controller;
using Smeedee.DomainModel.Config.SlideConfig;
using Smeedee.DomainModel.Framework;
using TinyMVVM.IoC;

#if SILVERLIGHT
using Smeedee.Client.Framework.SL.Views.Dialogs;
#endif

namespace Smeedee.Client.Framework.ViewModel.Dialogs
{
    public partial class SelectWidgetsDialog
    {
        //Public to enable testing
        public SelectWidgetsDialogController controller;

        partial void OnInitialize()
        {
            Title = "Select Widgets";
            Width = 600;
            Height = 600;
            NewSlides = new List<Slide>();

            AvailableWidgets = new ObservableCollection<WidgetMetadata>();
            Progressbar = new Progressbar();

#if SILVERLIGHT
            View = new SelectWidgetsDialogView();
#endif

            var widgetMetaDataRepo = this.GetDependency<IAsyncRepository<WidgetMetadata>>();
            var slideConfigPersister = this.GetDependency<IPersistDomainModelsAsync<SlideConfiguration>>();
            controller = new SelectWidgetsDialogController(this, widgetMetaDataRepo, slideConfigPersister);


            PropertyChanged +=
                (o, e) => { if (e.PropertyName == "SearchTerm") TriggerPropertyChanged("FilteredWidgets"); }; 
        }

        public IList<WidgetMetadata> FilteredWidgets
        {
            get
            {
                if (string.IsNullOrEmpty(SearchTerm))
                    return AvailableWidgets;
                else
                    return AvailableWidgets.Where(MatchesSearchTerm).ToList();
            }
        }

        private bool MatchesSearchTerm(WidgetMetadata item)
        {
            bool matchesName = item.Name != null && item.Name.ToLower().Contains(SearchTerm.ToLower());
            bool matchesTag = item.Tags != null &&item.Tags.Any(s => s != null && s.ToLower().Contains(SearchTerm.ToLower()));

            return matchesName || matchesTag;
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
            foreach (var widgetMetadata in FilteredWidgets)
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

        public override void OnOk()
        {
            controller.CreateSlidesFromSelectedWidgets();
        }

        public void DoneAddingSlides()
        {
            Progressbar.IsVisible = false;
            OnCloseDialog(true);
        }
    }
}
