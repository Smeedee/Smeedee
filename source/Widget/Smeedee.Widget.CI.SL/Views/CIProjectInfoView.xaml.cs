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
// The project webpage is located at http://www.smeedee.org
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using Smeedee.Widget.CI.ViewModels;

namespace Smeedee.Widget.CI.SL.Views
{
    public partial class CIProjectInfoView : UserControl
    {
        private ProjectInfoViewModel myProjectInfoViewModel = null;

        private bool hasBeenBroken;
        private bool firstTimeLoad;

        private bool tryPlayBuildFixed;
        private bool tryPlayBuildFailed;
        private Timer soundTimer;

        public CIProjectInfoView()
        {
            InitializeComponent();
            firstTimeLoad = true;
            hasBeenBroken = false;
            tryPlayBuildFixed = false;
            tryPlayBuildFailed = false;

            soundTimer = new Timer(new TimerCallback(soundTimer_callback), null, 500, 500);
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            myProjectInfoViewModel = DataContext as ProjectInfoViewModel;
            if (myProjectInfoViewModel != null)
            {
                myProjectInfoViewModel.LatestBuild.PropertyChanged +=
                    LatestBuildViewModel_PropertyChanged;
                if (firstTimeLoad)
                {
                    BuildStatusUpdate();
                }
            }
            firstTimeLoad = false;
        }

        private void LatestBuildViewModel_PropertyChanged(object sender,
                                                          System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Status")
            {
                BuildStatusUpdate();
            }
        }

        private void BuildStatusUpdate()
        {

            if (myProjectInfoViewModel == null)
            {
                return;
            }

            var buildStatus = myProjectInfoViewModel.LatestBuild.Status;

            switch (buildStatus)
            {
                case BuildStatus.Failed:
                    //TODO: A test must be written to validate this change
                    //The line is commented out to enable the sound to be played every time a build has failed
                    //and not only the first failed build after a successful one.
 //                   if (!hasBeenBroken)
 //                   {
                        tryPlayBuildFailed = true;
                        hasBeenBroken = true;
  //                  }
                    break;

                case BuildStatus.Successful:
                    if (hasBeenBroken)
                    {
                        tryPlayBuildFixed = true;
                        hasBeenBroken = false;
                    }
                    break;
            }
        }

        private void PlayBuildStatusSoundIfEnabled(MediaElement soundElement)
        {
            soundElement.Volume = 1.0;
            soundElement.Position = new TimeSpan(0);
            soundElement.Play();
        }

        /// <summary>
        /// note: we use a timer to play sounds in order to let the MediaElements buffer 
        /// the sound before it's played. It's ugly, it's a workaround, but it WORKS.
        /// </summary>
        /// <param name="state"></param>
        private void soundTimer_callback(object state)
        {
            if (myProjectInfoViewModel == null)
            {
                return;
            }
            Dispatcher.BeginInvoke(() =>
            {
                if (tryPlayBuildFixed && (BuildFixedSound.CurrentState != MediaElementState.Opening))
                {
                    PlayBuildStatusSoundIfEnabled(BuildFixedSound);
                    tryPlayBuildFixed = false;
                }

                if (tryPlayBuildFailed && (BuildFailedSound.CurrentState != MediaElementState.Opening))
                {
                    PlayBuildStatusSoundIfEnabled(BuildFailedSound);
                    tryPlayBuildFailed = false;
                }
            });
        }
    }
}