using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Smeedee.Client.Framework.SL;
#if SILVERLIGHT
using Smeedee.Client.Framework.SL.Resources.Graphic.Icons;
#endif
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Client.Framework.ViewModel.DockBarItems;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Config.SlideConfig;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.DSL.Specifications;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.Framework;
using TinyMVVM.Framework;
using Smeedee.Client.Framework.ViewModel.Dialogs;

namespace Smeedee.Client.Framework.Services.Impl
{
    public class ModuleLoader : IModuleLoader
    {
        [ImportMany(AllowRecomposition = true)]
        private IEnumerable<WidgetMetadata> availalbleWidgets;

        private readonly List<string> adminSlideTitles = new List<string>() { "Task Administration", "User Administration", "Holidays", "Add Widget", "Edit Slideshow" };
        private readonly IAsyncRepository<SlideConfiguration> slideConfigRepo;
        private readonly ILog logger;
        private Slideshow slideshowViewModel;
        private IEnumerable<SlideConfiguration> slideConfigs;
        private Widget welcomeWidget;
        private DockBar dockBarViewModel;
    	private IAsyncRepository<WidgetMetadata> widgetMetadataRepo;

    	public ModuleLoader(IAsyncRepository<SlideConfiguration> slideConfigRepo, 
			IAsyncRepository<WidgetMetadata> widgetMetadataRepo,
			ILog logger)
        {
    		Guard.Requires<ArgumentNullException>(slideConfigRepo != null);
			Guard.Requires<ArgumentNullException>(widgetMetadataRepo != null);
			Guard.Requires<ArgumentNullException>(logger != null);

            this.slideConfigRepo = slideConfigRepo;
            this.logger = logger;
			this.widgetMetadataRepo = widgetMetadataRepo;

			widgetMetadataRepo.GetCompleted += widgetMetadataRepo_GetCompleted;
        }

		void widgetMetadataRepo_GetCompleted(object sender, GetCompletedEventArgs<WidgetMetadata> e)
		{
			slideConfigRepo.GetCompleted -= slideConfigRepo_GetCompleted;
			slideConfigRepo.GetCompleted += slideConfigRepo_GetCompleted;
			availalbleWidgets = e.Result;

			if (slideConfigs == null)
				slideConfigRepo.BeginGet(All.ItemsOf<SlideConfiguration>());
			else
				CreateSlidesAndAdminWidgets();
		}

		void slideConfigRepo_GetCompleted(object sender, GetCompletedEventArgs<SlideConfiguration> e)
		{
			slideConfigs = e.Result;

			CreateSlidesAndAdminWidgets();
		}

    	private void CreateSlidesAndAdminWidgets()
    	{
    		if (slideshowViewModel != null)
    		{
    			welcomeWidget.ProgressbarService.ShowInView("Got slideshow configuration!");
    			CreateSlidesFromConfigs();
    		}

    		if (dockBarViewModel != null)
    			CreateAdminWidgets();
    	}

    	public void LoadAdminWidgets(DockBar dockBarViewModel)
        {
			Guard.Requires<ArgumentNullException>(dockBarViewModel != null);

            this.dockBarViewModel = dockBarViewModel;

            dockBarViewModel.Items.Add(new FullScreen());

			widgetMetadataRepo.BeginGet(All.ItemsOf<WidgetMetadata>());
        }

        public void LoadTraybarWidgets(Traybar traybarViewModel)
        {
        }

        public void LoadSlides(Slideshow slideshow)
        {
        	Guard.Requires<ArgumentNullException>(slideshow != null);

            slideshowViewModel = slideshow; 
            slideshowViewModel.Slides.Clear();
            welcomeWidget = new WelcomeWidget();
#if SILVERLIGHT
            welcomeWidget.View = new SL.WelcomeWidgetView();
#endif
            slideshowViewModel.Slides.Add(new Slide(){Widget = welcomeWidget});
            welcomeWidget.ProgressbarService.ShowInView("Downloading slideshow configuration...");

			widgetMetadataRepo.BeginGet(All.ItemsOf<WidgetMetadata>());
        }

        private void CreateSlidesFromConfigs()
        {
			var loadedSlides = new List<Slide>();
			int numberOfReadySlides = 0;
			int numberOfSlideConfigs = slideConfigs.Count();
			foreach (var slideConfiguration in slideConfigs)
			{
				var config = slideConfiguration;
				welcomeWidget.ProgressbarService.ShowInView(string.Format("Loading slide {0} of {1}",
								numberOfReadySlides,
								numberOfSlideConfigs));

				if (availalbleWidgets.Any(w => w.Type.FullName == config.WidgetType))
				{
					try
					{
						var newSlide = CreateSlideFromConfig(config);
						loadedSlides.Add(newSlide);
						numberOfReadySlides++;
					}
					catch (Exception e)
					{
						logger.WriteEntry(new ErrorLogEntry("(ModuleLoader)", e.ToString()));
					}
				}
				else
				{
					logger.WriteEntry(new ErrorLogEntry("(ModuleLoader)", "Tried to create an instance of an Widget that doesn't exist. Make sure Widget is deployed: " + slideConfiguration.WidgetType));
				}
			}
			welcomeWidget.ProgressbarService.ShowInView("Done!");

			if (loadedSlides.Count == 0 && numberOfSlideConfigs == 0)
			{
#if SILVERLIGHT
                welcomeWidget.ProgressbarService.HideInView();
                ((WelcomeWidgetView) welcomeWidget.View).ShowFirstTimeHelp();
#endif
			}
			else
			{
				slideshowViewModel.Slides.Clear();
				foreach (var loadedSlide in loadedSlides)
				{
					slideshowViewModel.Slides.Add(loadedSlide);
				}
			}
        }

        private Slide CreateSlideFromConfig(SlideConfiguration slideConfiguration)
        {
            var widgetType = GetType(slideConfiguration.WidgetType);
            var widget = Activator.CreateInstance(widgetType) as Widget;
            widget.SetConfigurationId(slideConfiguration.WidgetConfigurationId);
            
            var newSlide = new Slide()
            {
                Title = slideConfiguration.Title,
                SecondsOnScreen = slideConfiguration.Duration,
                Widget = widget
            };

            return newSlide;
        }

        private Type GetType(string typeName)
        {
            var widgetType = availalbleWidgets
                .Where(w => w.Type.FullName.Equals(typeName))
                .Select(w => w.Type)
                .SingleOrDefault();

            if( widgetType == null)
                throw new ArgumentException(string.Format("The type '{0}' could not be found. It will not be added to the slideshow!", typeName));

            return widgetType;
        }

        private void CreateAdminWidgets()
        {
            //NB: It's only possible to have one instance pr. Widget, so you need
            //to check if the admin widget has already been added
            if (this.dockBarViewModel == null) return;
            foreach (string title in adminSlideTitles)
            {
                if (dockBarViewModel.Items.Any(i => i.Description == title)) continue;

                var adminWidget = availalbleWidgets.FirstOrDefault(w => w.Name == title);

                if (title.Contains("Add Widget"))
                {
                    //var SelectWidgetsDialog = new SelectWidgetsDialog();
                    var dockBarItem = new AddWidgetDockBarItem(){ItemName = title};
                    

                    //dockBarItem.Click

                    //dockBarViewModel.Items.Add(new WidgetDockBarItem(title)
                    
                    //{                        
                    //    Click = new DelegateCommand(() => dialogService.Show(SelectWidgetsDialog, dialogResult =>
                    //    {
                    //        if (dialogResult == true)
                    //        {
                    //           // RemoveWelcomeWidgetIfPresent();

                    //            foreach (var newSlide in SelectWidgetsDialog.NewSlides)
                    //            {
                    //                slideshowViewModel.Slides.Add(newSlide);
                    //            }
                    //        }
                    //    })),
                    //    Icon = GetIcon(title)
                    //});
                }
                if (adminWidget != null)
                {
                    dockBarViewModel.Items.Add(new WidgetDockBarItem(title)
                    {
                        Description = adminWidget.Name,
                        Widget = Activator.CreateInstance(adminWidget.Type) as Widget,
                        //TODO: Refactor this hack. Now resolving the Icon based on the Title of the Widget
                        //We want to handle AdminDockbar plugins in a better way. A suggestion is to
                        //provide an API for the Widget devs so that they can add items programatically
                        //or use MEF and attributes. 
                        Icon = GetIcon(title)
                    });
                }
            }
        }

        private FrameworkElement GetIcon(string title)
        {
#if SILVERLIGHT
            if (title == "Holidays") return new HolidaysIcon();
            if (title == "Task Administration") return new TaskAdministrationIcon();
            if (title == "User Administration") return new UserAdministrationIcon();
            if (title == "Add Widget") return new AddWidgetIcon();
            if (title == "Edit Slideshow") return new EditSlideShowIcon();
            return new SettingsIcon(title);
#endif

            return null;
        }
    }
}
