using System;
using System.Collections.Generic;
using System.Linq;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Client.Framework.ViewModel.Dialogs;
using Smeedee.DomainModel.Config.SlideConfig;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.DSL.Specifications;
using Smeedee.Framework;

namespace Smeedee.Client.Framework.Controller
{
    public class SelectWidgetsDialogController
    {
        private List<string> adminWidgets = new List<string> { "Holidays", "Task Administration", "User Administration", "Smeedee for Mobile Devices" };
        private SelectWidgetsDialog viewModel;
        private IAsyncRepository<WidgetMetadata> repository;
        private readonly IPersistDomainModelsAsync<SlideConfiguration> slideConfigPersister;
        private int numberOfSlidesToSave;
        private int numberOfSlidesSaved;

        public SelectWidgetsDialogController(
            SelectWidgetsDialog viewModel, 
            IAsyncRepository<WidgetMetadata> repository, 
            IPersistDomainModelsAsync<SlideConfiguration> slideConfigPersister)
        {
            Guard.Requires<ArgumentNullException>(viewModel != null);
            Guard.Requires<ArgumentNullException>(repository != null);
            Guard.Requires<ArgumentNullException>(slideConfigPersister != null);
            
            this.viewModel = viewModel;
            this.repository = repository;
            this.slideConfigPersister = slideConfigPersister;

            this.slideConfigPersister.SaveCompleted += slideConfigPersister_SaveCompleted;

            repository.GetCompleted += repository_GetCompleted;

            BeginGetWidgetMetadata();
        }

        void slideConfigPersister_SaveCompleted(object sender, SaveCompletedEventArgs e)
        {
            numberOfSlidesSaved++;
            if( numberOfSlidesSaved >= numberOfSlidesToSave )
                viewModel.DoneAddingSlides();
        }

        public void CreateSlidesFromSelectedWidgets()
        {
            viewModel.NewSlides=new List<Slide>();
            numberOfSlidesToSave = viewModel.SelectedWidgets.Count();

            if(numberOfSlidesToSave == 0)
            {
                viewModel.DoneAddingSlides();
                return;
            }

            numberOfSlidesSaved = 0;
            viewModel.Progressbar.IsVisible = true;
            viewModel.Progressbar.Message = "Creating slides...";
            foreach (var widgetMetadata in viewModel.SelectedWidgets)
            {
                Widget newWidget = Activator.CreateInstance(widgetMetadata.Type) as Widget;

                var newSlideConfiguration = new SlideConfiguration()
                {
                    Title = widgetMetadata.UserSelectedTitle,
                    Duration = widgetMetadata.SecondsOnScreen,
                    WidgetConfigurationId = newWidget.Configuration.Id,
                    WidgetType = widgetMetadata.Type.FullName,
                    WidgetXapName = widgetMetadata.XAPName
                };

                var slideTitle = widgetMetadata.UserSelectedTitle ?? widgetMetadata.Name;
                var newSlide = new Slide { Title = slideTitle, Widget = newWidget, SecondsOnScreen = widgetMetadata.SecondsOnScreen };

                slideConfigPersister.Save(newSlideConfiguration);
                viewModel.NewSlides.Add(newSlide);
            }
        }


        private void BeginGetWidgetMetadata()
        {
            viewModel.Progressbar.IsVisible = true;
            repository.BeginGet(All.ItemsOf<WidgetMetadata>());
        }

        void repository_GetCompleted(object sender, GetCompletedEventArgs<WidgetMetadata> e)
        {
            foreach (var widgetMetadata in e.Result)
            {
                if (!adminWidgets.Contains(widgetMetadata.Name))
                {
                    viewModel.AvailableWidgets.Add(widgetMetadata);
                }
            }

            viewModel.Progressbar.IsVisible = false;
        }
    }
}
