using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using APD.Client.Framework;
using APD.Client.Framework.ViewModels;


namespace APD.Client.Widget.CI.ViewModels
{
    public class ProjectBuildHistoryViewModel : AbstractViewModel
    {
        public ProjectBuildHistoryViewModel(IInvokeUI invoker)
            : base(invoker) {}

        private string projectName;
        public string ProjectName
        {
            get { return projectName; }
            set
            {
                if( value != projectName )
                {
                    projectName = value;
                    TriggerPropertyChanged<ProjectBuildHistoryViewModel>(vm => vm.ProjectName);
                }
            }
        }

        private Dictionary<DateTime, int> successfullBuildsByDate;
        public Dictionary<DateTime, int> SuccessfullBuildsByDate
        {
            get { return successfullBuildsByDate; }
            set
            {
                if( value != successfullBuildsByDate )
                {
                    successfullBuildsByDate = value;
                    TriggerPropertyChanged<ProjectBuildHistoryViewModel>(vm => vm.SuccessfullBuildsByDate);
                }
            }
        }

        private Dictionary<DateTime, int> failedBuildsByDate;
        public Dictionary<DateTime, int> FailedBuildsByDate
        {
            get { return failedBuildsByDate; }
            set
            {
                if( value != failedBuildsByDate )
                {
                    failedBuildsByDate = value;
                    TriggerPropertyChanged<ProjectBuildHistoryViewModel>(vm => vm.FailedBuildsByDate);
                }
            }
        }

    }
}
