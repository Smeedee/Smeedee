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
using Smeedee.Client.Framework.ViewModel;

namespace Smeedee.Widget.CI.ViewModels
{
    public class BuildViewModel : BasicViewModel
    {
        private Timer buildDurationUpdateTimer;
        private TimeSpan buildDurationUpdateTimerOffset;

        /// <summary>
        /// This constructor initially sets triggeredBy, triggerCause and status to unknown.
        /// </summary>
        public BuildViewModel()
        {
            triggeredBy = new Person() {Firstname = "Unknown user"};
            triggerCause = "Unknown";
            status = BuildStatus.Unknown;
        }

        private void NotifyBuildDurationUpdate(object state)
        {
            TriggerPropertyChanged<BuildViewModel>(vm => vm.BuildDuration);
        }


        private Person triggeredBy;
        public Person TriggeredBy
        {
            get { return triggeredBy; }
            set
            {
                if (value != triggeredBy)
                {
                    triggeredBy = value;
                    TriggerPropertyChanged<BuildViewModel>(vm => vm.TriggeredBy);
                }
            }
        }

        private string triggerCause;
        public string TriggerCause
        {
            get { return triggerCause; }
            set
            {
                if (value != triggerCause)
                {
                    triggerCause = value;
                    TriggerPropertyChanged<BuildViewModel>(vm => vm.TriggerCause);
                }
            }
        }

        private DateTime startTime;
        public DateTime StartTime
        {
            get { return startTime; }
            set
            {
                if (value != startTime)
                {
                    startTime = value;
                    TriggerPropertyChanged<BuildViewModel>(vm => vm.StartTime);
                }
            }
        }

        private DateTime finishedTime;
        public DateTime FinishedTime
        {
            get { return finishedTime; }
            set
            {
                if (value != finishedTime)
                {
                    finishedTime = value;
                    TriggerPropertyChanged<BuildViewModel>(vm => vm.FinishedTime);
                }
            }
        }

        private BuildStatus status;
        public BuildStatus Status
        {
            get { return status; }
            set
            {
                if (value != status)
                {
                    status = value;

                    if (status == BuildStatus.Building)
                    {
                        buildDurationUpdateTimer = new Timer(new TimerCallback(NotifyBuildDurationUpdate),
                                                             null, 800, 800);
                        buildDurationUpdateTimerOffset = DateTime.Now - StartTime;
                    }
                    else
                    {
                        buildDurationUpdateTimer = null;
                    }

                    TriggerPropertyChanged<BuildViewModel>(vm => vm.Status);
                }
            }
        }

        public TimeSpan BuildDuration
        {
            get
            {
                if (Status == BuildStatus.Building)
                    return (DateTime.Now - StartTime);
                else
                    if (FinishedTime != DateTime.MinValue && StartTime != DateTime.MinValue)
                    {
                        return ( FinishedTime - StartTime );
                    }
                    else
                    {
                        return TimeSpan.Zero;
                    }
            }
        }
    }
}