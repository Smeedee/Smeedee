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

namespace Smeedee.Client.Framework.Services.Impl
{
    public class ModuleLoader : IModuleLoader, IPartImportsSatisfiedNotification
    {
        [ImportMany(AllowRecomposition = true)]
        public IEnumerable<Lazy<Widget, IWidgetMetadata>> availalbleWidgets =
            new List<Lazy<Widget, IWidgetMetadata>>();

        private readonly List<string> adminSlideTitles = new List<string>(){ "Task Administration", "User Administration", "Holidays" };
        private readonly IAsyncRepository<SlideConfiguration> slideConfigRepo;
        private readonly ILog log;
        private Slideshow slideshowViewModel;
        private bool isMefFinishedLoadingTypes;
        private IEnumerable<SlideConfiguration> slideConfigs;
        private Widget welcomeWidget;
        private DockBar dockBarViewModel;

        public ModuleLoader(IAsyncRepository<SlideConfiguration> slideConfigRepo, ILog log)
        {
            this.slideConfigRepo = slideConfigRepo;
            this.log = log;

#if SILVERLIGHT
            CompositionInitializer.SatisfyImports(this);
#endif
        }

        public void LoadAdminWidgets(DockBar dockBarViewModel)
        {
            this.dockBarViewModel = dockBarViewModel;

            dockBarViewModel.Items.Add(new FullScreen());
        }

        public void LoadTraybarWidgets(Traybar traybarViewModel)
        {
            
        }

        public void LoadSlides(Slideshow slideshow)
        {
           
            slideshowViewModel = slideshow; 
            slideshowViewModel.Slides.Clear();
            welcomeWidget = new WelcomeWidget();
#if SILVERLIGHT
            welcomeWidget.View = new SL.WelcomeWidgetView();
#endif
            slideshowViewModel.Slides.Add(new Slide(){Widget = welcomeWidget});
            welcomeWidget.ProgressbarService.ShowInView("Downloading slideshow configuration...");
            slideConfigRepo.GetCompleted += (o, e) =>
            {
                welcomeWidget.ProgressbarService.ShowInView("Got slideshow configuration!");
                slideConfigs = e.Result;
                CreateSlidesFromConfigs();
            };
            slideConfigRepo.BeginGet(All.ItemsOf<SlideConfiguration>());
            
        }

        private void CreateSlidesFromConfigs()
        {
            if( isMefFinishedLoadingTypes == false )
            {
                welcomeWidget.ProgressbarService.ShowInView("Waiting for widgets to download...");
                return;
            }

                var slides = new List<Slide>();
                removeAdminSlideConfigs();
                int numberOfReadySlides = 0;
                int numberOfSlideConfigs = slideConfigs.Count();
                foreach (var slideConfiguration in slideConfigs)
                {
                    try
                    {
                        welcomeWidget.ProgressbarService.ShowInView(string.Format("Loading slide {0} of {1}",
                                                          numberOfReadySlides,
                                                          numberOfSlideConfigs));

                        var newSlide = CreateSlideFromConfig(slideConfiguration);
                        slides.Add(newSlide);
                        numberOfReadySlides++;
                    }
                    catch (Exception e)
                    {
                        log.WriteEntry(new ErrorLogEntry("(ModuleLoader)", e.ToString()));
                    }
                }
                welcomeWidget.ProgressbarService.ShowInView("Done!");

                if (slides.Count == 0 && numberOfSlideConfigs == 0)
                {
#if SILVERLIGHT
                    welcomeWidget.ProgressbarService.HideInView();
                    ((WelcomeWidgetView) welcomeWidget.View).ShowFirstTimeHelp();
#endif
                }
                else
                {
                    slideshowViewModel.Slides.Clear();
                    foreach (var loadedSlide in slides)
                    {
                        slideshowViewModel.Slides.Add(loadedSlide);
                    }
                }

        }

        private void removeAdminSlideConfigs()
        {
            slideConfigs = slideConfigs.Where(slideConfiguration => !adminSlideTitles.Contains(slideConfiguration.Title));
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
                .Where(w => w.Value.GetType().FullName.Equals(typeName))
                .Select(w => w.Value.GetType())
                .SingleOrDefault();

            if( widgetType == null)
                throw new ArgumentException(string.Format("The type '{0}' could not be found. It will not be added to the slideshow!", typeName));

            return widgetType;
            

            /*
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in loadedAssemblies)
            {
                var foundType = assembly.GetType(typeName);
                if (foundType != null)
                    return foundType;
            }

            return null; */

        }

        public void OnImportsSatisfied()
        {
            isMefFinishedLoadingTypes = true;
            if( slideConfigs != null )
                CreateSlidesFromConfigs();

            AddAdminWidgetsToDockBar();
        }

        private void AddAdminWidgetsToDockBar()
        {
            //NB: It's only possible to have one instance pr. Widget, so you need
            //to check if the admin widget has already been added
            if (this.dockBarViewModel == null) return;
            foreach (string title in adminSlideTitles)
            {
                if (dockBarViewModel.Items.Any(i => i.Description == title)) continue;

                var adminWidget = availalbleWidgets.FirstOrDefault(w => w.Metadata.Name == title);

                if (adminWidget != null)
                {
                    dockBarViewModel.Items.Add(new WidgetDockBarItem(title)
                    {
                        Description = adminWidget.Metadata.Name,
                        Widget = Activator.CreateInstance(adminWidget.Value.GetType()) as Widget,
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
            return new SettingsIcon(title);
#endif

            return null;
        }
    }
}
