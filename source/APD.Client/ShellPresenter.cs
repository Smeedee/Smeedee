#region File header

// <copyright>
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
// /copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using APD.Client.Framework;
using Microsoft.Practices.Composite.Regions;
using Microsoft.Practices.Unity;
using APD.Client.Services;


namespace APD.Client
{
    public class ShellPresenter
    {
        public IShell Shell { get; private set; }

        public IEnumerable<Slide> Slides { get; set; }
        private bool alternateAttentionText = true;
        private List<int> freezeQueue = new List<int>();

        public Slide AdminSlide;

        private int currentSlideIndex = -1;

        private int slideSwitchCountdown = 15;
        private Timer secondTicker;
        private IInvokeUI uiInvoker;
        private IRegionManager regionManager;
        private ICheckIfAdminUIShouldBeDisplayed adminUiDisplayChecker;

        public bool IsPaused { get; private set;}
        public bool IsFrozen { get; private set;}
        public bool IsBrokenBuild { get; private set; }
        public bool IsAdminMode { get; private set; }

        public ShellPresenter(IShell view, 
            IInvokeUI uiInvoker,
            IRegionManager regionManager,
            FreezeViewCommandNotifier freezeCmd,
            UnFreezeViewCommandNotifier unFreezeCmd,
            NextSlideCommandNotifier nextSlideCmd,
            PreviousSlideCommandNotifier prevSlideCmd,
            TogglePauseSlideShowCommandNotifier togglePauseCmd,
            ToggleAdminModeCommandNotifier toggleAdminCmd,
            ICheckIfAdminUIShouldBeDisplayed adminUiDisplayChecker)
        {
            Slides = new List<Slide>();
            Shell = view;

            Shell.SetTitle("Welcome!");

            this.uiInvoker = uiInvoker;
            this.regionManager = regionManager;
            SetupCommandNotifiers(freezeCmd,unFreezeCmd,nextSlideCmd,prevSlideCmd,togglePauseCmd, toggleAdminCmd);

            secondTicker = new Timer(second_Tick, null, 2000, 998);

            this.adminUiDisplayChecker = adminUiDisplayChecker;
        }

        private void SetupCommandNotifiers(
            FreezeViewCommandNotifier freezeCmd,
            UnFreezeViewCommandNotifier unFreezeCmd,
            NextSlideCommandNotifier nextSlideCmd,
            PreviousSlideCommandNotifier prevSlideCmd,
            TogglePauseSlideShowCommandNotifier togglePauseCmd,
            ToggleAdminModeCommandNotifier toggleAdminCmd
            )
        {
            var freezeViewCommandsListener = freezeCmd;
            var unfreezeViewCommandsListener = unFreezeCmd;
            var nextSlideCommandListener = nextSlideCmd;
            var previousSlideCommandListener = prevSlideCmd;
            var togglePauseSlideShowCommandNotifier = togglePauseCmd;
            var toggleAdminModeCommandNotifier = toggleAdminCmd;

            freezeViewCommandsListener.CommandPublished += (o, e) => FreezeSlide();
            unfreezeViewCommandsListener.CommandPublished += (o, e) => UnFreezeSlide();
            nextSlideCommandListener.CommandPublished += (o, e) => SwithToNextSlide();
            previousSlideCommandListener.CommandPublished += (o, e) => SwitchToPreviousSlide();
            togglePauseSlideShowCommandNotifier.CommandPublished += (o, e) => TogglePauseSlideshow();
            toggleAdminModeCommandNotifier.CommandPublished += (o, e) => ToggleAdminMode();
        }

        private void second_Tick(object state)
        {
            if (IsAdminMode)
            {
                return;
            }
            if (IsPaused || IsFrozen)
            {
                uiInvoker.Invoke(() =>
                {
                    string blinkingText = IsBrokenBuild ? "ATTENTION!" : "Paused";
                    Shell.SetInfoString( alternateAttentionText ? blinkingText : "");
                });

                alternateAttentionText = !alternateAttentionText;
                return;
            }

            slideSwitchCountdown--;
            if (slideSwitchCountdown <= 0)
            {
                SwithToNextSlide();
            }

            uiInvoker.Invoke(
                () => Shell.SetInfoString(
                    string.Format("Slide {0}/{1} - Next slide in {2}s", currentSlideIndex + 1,
                                                                        Slides.Count(), 
                                                                        slideSwitchCountdown)));
        }

        public void StartSlideRotation()
        {
            if (Slides.Count() <= 0)
            {
                return;
            }
            else if (adminUiDisplayChecker.DisplayAdminUI())
                ToggleAdminMode();
            else
                SwithToNextSlide();
        }

        public void SwithToNextSlide()
        {
            currentSlideIndex = NextSlideIndex();
            ShowCurrentSlide();
        }

        public void SwitchToPreviousSlide()
        {
            currentSlideIndex = PreviousSlideIndex();
            ShowCurrentSlide();
        }

        public void TogglePauseSlideshow()
        {
            var paused = IsPaused;
            var frozen = IsFrozen;

            if(paused)
                IsPaused = false;       

            if (frozen)
                IsFrozen = false;

            if (!paused && !frozen)
            {
                IsPaused = true;                
            }
        }

        //public void ResumeSlideshow()
        //{
        //    IsPaused = false;
        //}

        private int NextSlideIndex()
        {
            int nextSlideIndex = currentSlideIndex + 1;
            if (nextSlideIndex >= Slides.Count())
            {
                nextSlideIndex = 0;
            }
            return nextSlideIndex;
        }

        private int PreviousSlideIndex()
        {
            int previousSlideIndex = currentSlideIndex - 1;
            if (previousSlideIndex < 0)
            {
                previousSlideIndex = Slides.Count() - 1;
            }
            return previousSlideIndex;
        }

        public void FreezeSlide()
        {
            IsFrozen = true;
            IsBrokenBuild = true;
            if (!IsPaused && !IsAdminMode)
            {
                currentSlideIndex = IndexOfSlideWithTitle("Build Status");
                ShowCurrentSlide();
            }
        }

        private int IndexOfSlideWithTitle(String title)
        {
            var i = 0;
            foreach (var slide in Slides)
            {
                if (slide.Title.Equals(title))
                {
                    return i;
                }
                i++;
            }
            return 0;
        }

        public void UnFreezeSlide()
        {
            IsFrozen = false;
            IsBrokenBuild = false;
        }

        private void ShowCurrentSlide()
        {
            Slide currentSlide = Slides.ElementAt(currentSlideIndex);
            LayoutViewsInSlide(currentSlide);
            SetSlideSwitchCountdown(currentSlide.DisplayTime);
        }

        private void SetSlideSwitchCountdown(TimeSpan span)
        {
            slideSwitchCountdown = (int) Math.Ceiling(span.TotalSeconds);
        }

        private void LayoutViewsInSlide(Slide slide)
        {
            uiInvoker.Invoke(() =>
            {
                Shell.SetTitle(slide.Title);

                foreach (var regionMapping in slide.RegionMappings)
                {
                    IRegion region = regionManager.Regions[regionMapping.Key.ToString()];
                    RemoveCurrentView(region);
                    region.Add(regionMapping.Value.View);
                }
            });
        }

        private void RemoveCurrentView(IRegion region)
        {
            if (region.Views == null) return;
            if (region.Views.Count() >= 1)
            {
                var view = region.Views.First();
                region.Remove(view);
            }
        }

        public void ToggleAdminMode()
        {
            if (IsAdminMode)
            {
                IsAdminMode = false;
                ShowCurrentSlide();
            }
            else
            {
                IsAdminMode = true;
                showAdminSlide();
            }
        }

        private void showAdminSlide()
        {
            uiInvoker.Invoke(() =>
            {
                Shell.SetInfoString("Admin Mode");
            });
            LayoutViewsInSlide(AdminSlide);
        }
    }
}