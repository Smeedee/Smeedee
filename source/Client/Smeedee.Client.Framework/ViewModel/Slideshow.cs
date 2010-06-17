using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Text;
using Smeedee.Client.Framework.Factories;
using Smeedee.Client.Framework.Services;

namespace Smeedee.Client.Framework.ViewModel
{
    public partial class Slideshow : IPartImportsSatisfiedNotification
    {
        private bool IsPlaying;
        private int cursor;
        private IModuleLoader moduleLoader = new ModuleLoaderFactory().NewModuleLoader();
        private ITimer timer;
        private int SLIDE_CHANGE_INTERVAL = 15; //seconds
        private DateTimeOffset slideChangedTimestamp;

        public void OnInitialize()
        {
            ErrorInfo = new ErrorInfo();
            Slides = new ObservableCollection<Slide>();
            Slides.CollectionChanged += Slides_CollectionChanged;

            TryLoadSlides();
            SetSlideshowInfo();

            timer = GetInstance<ITimer>();
            timer.Elapsed += timer_Elapsed;

            if (Slides.Count > 0) CurrentSlide = Slides.First();
        }

        void Slides_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (HasSlides())
            {
                Next.TriggerCanExecuteChanged();
                Previous.TriggerCanExecuteChanged();
                Start.TriggerCanExecuteChanged();
                Pause.TriggerCanExecuteChanged();
                ChangeSlide();
            }
        }

        private void TryLoadSlides()
        {
            try
            {
                //TODO: Load one slide at a time, so one failing widget wont mess up for everyone
                moduleLoader.LoadSlides(this);
            }
            catch (Exception ex)
            {
                ErrorInfo.HasError = true;
                ErrorInfo.ErrorMessage = ex.Message;
            }
        }

        private void SetSlideshowInfo()
        {
            var cursorPrettyPrint = HasSlides() ? string.Format("{0}", cursor + 1) : "0";
            if (IsPlaying)
            {
                SlideshowInfo = string.Format("Slide {0}/{1} - Next slide in {2} seconds", cursorPrettyPrint, Slides.Count, 
                    (SLIDE_CHANGE_INTERVAL - SecondsSinceChangedLastSlide()).ToString("0"));
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
            if (SecondsSinceChangedLastSlide() >= SLIDE_CHANGE_INTERVAL)           
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
            return HasSlides();
        }

        private void NextSlide()
        {
            CurrentSlide.IsInSettingsMode = false;
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

        public void OnPrevious()
        {
            if (HasSlides())
                PreviousSlide();
        }

        public bool CanPrevious()
        {
            return HasSlides();
        }

        private void PreviousSlide()
        {
            CurrentSlide.IsInSettingsMode = false;
            if (cursor > 0) cursor--;
            else cursor = Slides.Count - 1;

            ChangeSlide();
        }

        public void OnStart()
        {
            if (IsPlaying)
            {
                IsPlaying = !IsPlaying;
                timer.Stop();
                SetSlideshowInfo();
            }
            else
            {
                IsPlaying = !IsPlaying;
                NewSlideChangedTimestamp();
                SetSlideshowInfo();
                timer.Start(1000);
            }
        }

        public bool CanStart()
        {
            return HasSlides();
        }

        private void NewSlideChangedTimestamp()
        {
            slideChangedTimestamp = new DateTimeOffset(DateTime.Now);
        }

        public void OnPause()
        {
            IsPlaying = !IsPlaying;
            timer.Stop();
        }

        public void OnImportsSatisfied()
        {
        }
    }
}
