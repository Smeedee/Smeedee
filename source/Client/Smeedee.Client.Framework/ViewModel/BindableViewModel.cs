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
// </copyright> 
// 
// <contactinfo>
// The project webpage is located at http://www.smeedee.org
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System;
using System.Collections.ObjectModel;
using TinyMVVM.Framework;

namespace Smeedee.Client.Framework.ViewModel
{
    public class BindableViewModel<T> : AbstractViewModel
    {
        public BindableViewModel()
        {
            Data = new ObservableCollection<T>();

            SetupDelegateCommands();
        }

        public DelegateCommand SaveSettings { get; set; }
        public DelegateCommand ReloadFromRepository { get; set; }

        private void SetupDelegateCommands()
        {
            SaveSettings = new DelegateCommand();
            ReloadFromRepository = new DelegateCommand();
        }

        private ObservableCollection<T> data;
        public ObservableCollection<T> Data 
        { 
            get { return data; } 
            set
            {
                if (data != value)
                {
                    data = value;
                    DuckTypedData = value;
                    TriggerPropertyChanged<BindableViewModel<T>>( vm => vm.Data);
                }
            }
        }

        private bool isUsingDate;
        public bool IsUsingDate
        {
            get { return isUsingDate; }
            set
            {
                if (value != isUsingDate)
                {
                    isUsingDate = value;

                    if(isUsingTimespan == isUsingDate) { IsUsingTimespan = !IsUsingTimespan;}
                    TriggerPropertyChanged<BindableViewModel<T>>(vm => vm.IsUsingDate);
                    TriggerPropertyChanged<BindableViewModel<T>>(vm => vm.ActualDateUsed);
                }
            }
        }

        private bool isUsingTimespan;
        public bool IsUsingTimespan
        {
            get { return isUsingTimespan; }
            set
            {
                if (value != isUsingTimespan)
                {
                    isUsingTimespan = value;

                    if (isUsingTimespan == isUsingDate) { IsUsingDate = !IsUsingDate; }

                    TriggerPropertyChanged<BindableViewModel<T>>(vm => vm.IsUsingTimespan);
                }
            }
        }

        private DateTime sinceDate;
        public DateTime SinceDate
        {
            get { return sinceDate; }
            set
            {
                if (value != sinceDate)
                {
                    sinceDate = value.Date;
                    TriggerPropertyChanged<BindableViewModel<T>>(vm => vm.SinceDate);
                    TriggerPropertyChanged<BindableViewModel<T>>(vm => vm.ActualDateUsed);
                }
            }
        }

        private int timeSpanInDays;
        public int TimeSpanInDays
        {
            get { return timeSpanInDays; }
            set
            {
                if (value != timeSpanInDays)
                {
                    const int minAllowedTimeSpan = 0;
                    if (value < minAllowedTimeSpan) { value = minAllowedTimeSpan; }

                    timeSpanInDays = value;
                    TriggerPropertyChanged<BindableViewModel<T>>(vm => vm.TimeSpanInDays);
                    TriggerPropertyChanged<BindableViewModel<T>>(vm => vm.ActualDateUsed);
                }
            }
        }

        private int maxNumOfCommiters;
        public int MaxNumOfCommiters
        {
            get{ return maxNumOfCommiters; }
            set
            {
                if(value != maxNumOfCommiters)
                {
                    maxNumOfCommiters = value;
                    TriggerPropertyChanged<BindableViewModel<T>>(vm => vm.MaxNumOfCommiters);
                }
            }
        }

        private bool acknowledgeOthers;
        public bool AcknowledgeOthers
        {
            get { return acknowledgeOthers; }
            set
            {
                if (value != acknowledgeOthers)
                {
                    acknowledgeOthers = value;
                    TriggerPropertyChanged<BindableViewModel<T>>(vm => vm.AcknowledgeOthers);
                    TriggerPropertyChanged<BindableViewModel<T>>(t => t.NumberOfCommitsAndRevision);
                }
            }
        }

        private long currentRevision;
        public long CurrentRevision
        {
            get { return currentRevision; }
            set
            {
                if (value != currentRevision)
                {
                    currentRevision = value;
                    revisionIsChanged = true;
                    TriggerPropertyChanged<BindableViewModel<T>>(t => t.NumberOfCommitsAndRevision);
                    TriggerPropertyChanged<BindableViewModel<T>>(t => t.CurrentRevision);
                }
            }
        }

        private bool revisionIsChanged;

        public bool RevisionIsChanged
        {
            get
            {
                if(revisionIsChanged)
                {
                    revisionIsChanged = false;
                    return true;
                }
                return false;
            }
            
        }

        private int numberOfCommitsShown;
        public int NumberOfCommitsShown
        {
            get { return numberOfCommitsShown; }
            set
            {
                if (value != numberOfCommitsShown)
                {
                    numberOfCommitsShown = value;
                    TriggerPropertyChanged<BindableViewModel<T>>(t => t.NumberOfCommitsShown);
                    TriggerPropertyChanged<BindableViewModel<T>>(t => t.NumberOfCommitsAndRevision);
                }
            }
        }

        public DateTime ActualDateUsed
        {
            get { return isUsingDate ? sinceDate : DateTime.Now.AddDays(-timeSpanInDays).Date; }
        }

        public string NumberOfCommitsAndRevision
        {
            get
            {
                return string.Format(
                    "Number of commits shown is {0} \nBased on revision {1}",
                    numberOfCommitsShown, currentRevision);
            }
        }

        public Object DuckTypedData { get; set; }
    }
}