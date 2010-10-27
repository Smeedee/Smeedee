using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Text;
using Smeedee.Client.Framework.Controller;
using Smeedee.Client.Framework.Factories;
using Smeedee.Client.Framework.Services;
#if SILVERLIGHT
using Smeedee.Client.Framework.SL.ViewModel.Repositories;
#endif
using Smeedee.Client.Framework.ViewModel.Dialogs;
using Smeedee.DomainModel.Config.SlideConfig;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using TinyMVVM.Framework;
using TinyMVVM.IoC;

namespace Smeedee.Client.Framework.ViewModel
{
    public partial class Slideshow
    {
        private int cursor;
        private IModuleLoader moduleLoader;
        private ITimer timer;
        private DateTimeOffset slideChangedTimestamp;
        private ILog log;
        private IModalDialogService modalDialogService;
        private bool wasRunningBeforeEnteringSettingsView = true;
        public SelectWidgetsDialog SelectWidgetsDialog { get; protected set;}


        partial void OnInitialize()
        {
            SelectWidgetsDialog = new SelectWidgetsDialog();
        	moduleLoader = this.GetDependency<IModuleLoader>();
            log = this.GetDependency<ILog>();
            
            ErrorInfo = new ErrorInfo();
            Slides = new ObservableCollection<Slide>();
            Slides.CollectionChanged += Slides_CollectionChanged;

            TryLoadSlides();
            SetSlideshowInfo();

            timer = this.GetDependency<ITimer>();
            timer.Elapsed += timer_Elapsed;

        	modalDialogService = this.GetDependency<IModalDialogService>();

            if (Slides.Count > 0)
            {
                cursor = 0;
                ChangeSlide();
            }
        }

		partial void OnSetCurrentSlide(ref Slide value)
		{
			foreach (var slide in Slides)
			{
				slide.IsDisplayed = false;
				if (slide.Widget != null)
					slide.Widget.IsDisplayed = false;
			}

			value.IsDisplayed = true;
			if (value.Widget != null)
				value.Widget.IsDisplayed = true;
		}

        void Slides_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (!HasSlides()) 
                return;

            Next.TriggerCanExecuteChanged();
            Previous.TriggerCanExecuteChanged();
            Start.TriggerCanExecuteChanged();
            Pause.TriggerCanExecuteChanged();
            GoToLastSlide();

            if (e.NewItems == null)
                return;

            foreach (var newSlide in e.NewItems)
            {
                var slide = newSlide as Slide;
                if (slide == null || slide.Widget == null) continue;
                slide.Widget.PropertyChanged += PauseOnSettingsView;
            }
        }

        private void PauseOnSettingsView(object sender, PropertyChangedEventArgs e)
        {
            var widget = sender as Widget;
            if (e.PropertyName != "IsInSettingsMode" || widget == null) 
                return;

            if (widget.IsInSettingsMode)
            {
                wasRunningBeforeEnteringSettingsView = IsRunning;
                Pause.Execute();
            } 
            else
            {
                if (wasRunningBeforeEnteringSettingsView)
                    Start.Execute();
            }
        }

        private void TryLoadSlides()
        {
        	try
        	{
				moduleLoader.LoadSlides(this);
        	}
        	catch (Exception ex)
        	{
				ErrorInfo.HasError = true;
				ErrorInfo.ErrorMessage = ex.Message;
				TryWriteToErrorLog(ex);
        	}
        }

    	private void TryWriteToErrorLog(Exception ex)
        {
            if (log != null)
            {
                log.WriteEntry(new ErrorLogEntry("Slideshow", "Failed to load slide: " + ex.ToString()));    
            }
        }

        private void SetSlideshowInfo()
        {
            var cursorPrettyPrint = HasSlides() ? string.Format("{0}", cursor + 1) : "0";
            if (IsRunning)
            {
                SlideshowInfo = string.Format("Slide {0}/{1} - Next slide in {2} seconds", cursorPrettyPrint, Slides.Count, 
                    (CurrentSlide.SecondsOnScreen - SecondsSinceChangedLastSlide()).ToString("0"));
                TimeLeftOfSlideInPercent = 1.0 - (SecondsSinceChangedLastSlide()/CurrentSlide.SecondsOnScreen);
            }
            else
            {
                SlideshowInfo = string.Format("Slide {0}/{1} - Paused", cursorPrettyPrint, Slides.Count);
            }
        }

        private double SecondsSinceChangedLastSlide()
        {
            return (DateTimeOffset.Now - slideChangedTimestamp).TotalSeconds;
        }

        void timer_Elapsed(object sender, EventArgs e)
        {
            if( CurrentSlide == null )
                return;

            if (SecondsSinceChangedLastSlide() >= CurrentSlide.SecondsOnScreen)           
                NextSlide();
            else
                SetSlideshowInfo();
        }

        public void OnNext()
        {
            if (HasSlides())
                NextSlide();
        }

        public bool CanNext()
        {
            return HasSlides() && CurrentSlide != null && !CurrentSlide.IsInSettingsMode;
        }

        private void NextSlide()
        {
            if(CurrentSlideInSettingsMode() )
                return;
            
            if (cursor < Slides.Count - 1) cursor++;
            else cursor = 0;

            ChangeSlide();
        }

        private bool HasSlides()
        {
            return Slides.Count > 0;
        }

        private void ChangeSlide()
        {
            CurrentSlide = Slides[cursor];
            NewSlideChangedTimestamp();
            SetSlideshowInfo();
        }

        private void GoToLastSlide()
        {
            cursor = Slides.Count - 1;
            CurrentSlide = Slides[cursor];
            NewSlideChangedTimestamp();
            SetSlideshowInfo();
        }

        public void OnPrevious()
        {
            if (HasSlides())
                PreviousSlide();
        }

        public bool CanPrevious()
        {
            return HasSlides() && CurrentSlide != null && !CurrentSlide.IsInSettingsMode;
        }

        private void PreviousSlide()
        {
            if (CurrentSlideInSettingsMode())
                return;

            if (cursor > 0) cursor--;
            else cursor = Slides.Count - 1;

            ChangeSlide();
        }

        public void OnStart()
        {
            if (CurrentSlideInSettingsMode() || IsRunning)
            {
                return;
            }

            IsRunning = true;
            NewSlideChangedTimestamp();
            SetSlideshowInfo();
            timer.Start(100);
        }

        public bool CanStart()
        {
            return HasSlides();
        }

        private void NewSlideChangedTimestamp()
        {
            slideChangedTimestamp = new DateTimeOffset(DateTime.Now);
            TimeLeftOfSlideInPercent = 1.0;
        }

        public void OnPause()
        {
            if(!IsRunning )
                return;

            IsRunning = false;
            timer.Stop();
            SetSlideshowInfo();
        }

        public void OnAddSlide()
		{
            if (CurrentSlideInSettingsMode())
            {
                return;
            }
		    
            SelectWidgetsDialog = new SelectWidgetsDialog();
            modalDialogService.Show(SelectWidgetsDialog, dialogResult =>
		    {
                if (dialogResult == true)
                {
                    RemoveWelcomeWidgetIfPresent();

                    foreach (var newSlide in SelectWidgetsDialog.NewSlides)
                    {
                        Slides.Add(newSlide);
                        
                    }
                }
            });

		}

        private void RemoveWelcomeWidgetIfPresent()
        {
            int welcomeWidgetIndex = -1;
            int indexCounter = 0;

            foreach (var slide in Slides)
            {
                if (slide.Widget != null && slide.Widget.GetType() == typeof(WelcomeWidget))
                    welcomeWidgetIndex = indexCounter;
                indexCounter++;
            }

            if(welcomeWidgetIndex != -1)
            {
                Slides.RemoveAt(welcomeWidgetIndex);
            }
        }

        private bool CurrentSlideInSettingsMode()
        {
            return CurrentSlide != null && CurrentSlide.Widget.IsInSettingsMode;
        }

        public void OnEdit()
		{
            if (CurrentSlideInSettingsMode())
            {
                return;
            }
		    var dialogViewModel = new EditSlideshowDialog()
		    {
                Slideshow = this
		    };

			modalDialogService.Show(dialogViewModel, dialogResult =>
			{
                if (dialogResult)
	            {
		            var slideConfigPersister = this.GetDependency<IPersistDomainModelsAsync<SlideConfiguration>>();
                    var newSlideConfigurations = new List<SlideConfiguration>();
                    foreach (var slide in Slides)
                    {
                        var slideConfig = new SlideConfiguration()
                        {
                            Title = slide.Title,
                            Duration = slide.SecondsOnScreen,
                            WidgetConfigurationId = slide.Widget.Configuration.Id,
                            WidgetType = slide.Widget.GetType().FullName,

                        };

                        newSlideConfigurations.Add(slideConfig);
                    }
                    slideConfigPersister.Save(newSlideConfigurations);
			    }
            }); 
		}
    }
}
