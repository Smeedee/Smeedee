using System;
using Smeedee.Client.Framework.ViewModel;
using TinyMVVM.Framework;

namespace Smeedee.Widgets.SourceControl.ViewModels
{
    public class CommitStatisticsSettingsViewModel : AbstractViewModel
    {

        private const int Max_Timespan_In_Days = 365*5;
        private readonly DateTime defaultSinceDate = DateTime.Now.AddDays(-50);

        private bool isUsingDate;
        public bool IsUsingDate
        {
            get { return isUsingDate; }
            set
            {
                if (value != isUsingDate)
                {
                    isUsingDate = value;

                    if (value)
                    {
                        isUsingTimespan = false;
                    }

                    TriggerPropertyChanged<CommitStatisticsSettingsViewModel>(vm => vm.IsUsingDate);
                    TriggerPropertyChanged<CommitStatisticsSettingsViewModel>(vm => vm.ActualDateUsed);
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

                    if (value)
                    {
                        isUsingDate = false;
                    }

                    TriggerPropertyChanged<CommitStatisticsSettingsViewModel>(vm => vm.IsUsingTimespan);
                    TriggerPropertyChanged<CommitStatisticsSettingsViewModel>(vm => vm.ActualDateUsed);
                }
            }
        }

        private int commitTimespanDays;
        public int CommitTimespanDays
        {
            get { return commitTimespanDays; }
            set
            {
                if (value != commitTimespanDays)
                {
                    commitTimespanDays = value;
                    TriggerPropertyChanged<CommitStatisticsSettingsViewModel>(t => t.CommitTimespanDays);
                    TriggerPropertyChanged<CommitStatisticsSettingsViewModel>(vm => vm.ActualDateUsed);
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
                    if (DateTime.Now.Subtract(sinceDate).Days > Max_Timespan_In_Days)
                    {
                        sinceDate = defaultSinceDate;
                    }
                    else
                    {
                        sinceDate = value.Date;
                    }
                    TriggerPropertyChanged<CommitStatisticsSettingsViewModel>(vm => vm.SinceDate);
                    TriggerPropertyChanged<CommitStatisticsSettingsViewModel>(vm => vm.ActualDateUsed);
                }
            }
        }

        public DateTime ActualDateUsed
        {
            get
            {
                if (isUsingDate)
                {
                    return sinceDate;
                }
                return DateTime.Now.AddDays(-commitTimespanDays);
            }
        }

        public DelegateCommand SaveSettings { get; set; }
        public DelegateCommand ReloadSettings { get; set; }

        public bool IsSaving;
        


        public CommitStatisticsSettingsViewModel()
        {
            SaveSettings = new DelegateCommand();
            ReloadSettings = new DelegateCommand();
            SinceDate = defaultSinceDate;
        }
    }
}
