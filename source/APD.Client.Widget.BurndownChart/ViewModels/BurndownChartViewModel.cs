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
using APD.Client.Framework;
using APD.Client.Framework.ViewModels;
using APD.DomainModel.ProjectInfo;


namespace APD.Client.Widget.BurndownChart.ViewModel
{
    public class BurndownChartViewModel : AbstractViewModel
    {
        private List<BurndownChartCoordinate> actualBurndown;
        private List<BurndownChartCoordinate> idealBurndown;
        public bool ProjectsInRepository;
        public bool IterationInProject;
        public bool TasksInIteration;

        public BurndownChartViewModel(IInvokeUI invoker)
            : base(invoker)
        {
            actualBurndown = new List<BurndownChartCoordinate>();
            idealBurndown = new List<BurndownChartCoordinate>();

            idealBurndown.Add(new BurndownChartCoordinate(1, DateTime.Today));
            idealBurndown.Add(new BurndownChartCoordinate(0, DateTime.Today.AddDays(1)));

            ProjectsInRepository = false;
            IterationInProject = false;
            TasksInIteration = false;
            errorMessage = "Burndown chart coordinates have not yet been set on the ViewModel";
        }

        
        private string errorMessage;  
        public string ErrorMessage   
        {
            get
            {
                if (!ProjectsInRepository)
                {
                    errorMessage = "No projects with the given name exists in the repository";
                    TriggerPropertyChanged<BurndownChartViewModel>(vm => vm.ErrorMessage);
                }  
                else if (!IterationInProject)
                {
                    errorMessage = "There are no iterations in the given project";
                    TriggerPropertyChanged<BurndownChartViewModel>(vm => vm.ErrorMessage);
                }
                else if (!TasksInIteration)
                {
                    errorMessage = "There are no tasks for the current iteration for the given project";
                    TriggerPropertyChanged<BurndownChartViewModel>(vm => vm.ErrorMessage);
                }
                return errorMessage;
            }
        }

        private string projectName;
        public string ProjectName
        {
            get { return projectName; }
            set
            {
                if (value != projectName)
                {
                    projectName = value;
                    TriggerPropertyChanged<BurndownChartViewModel>(vm => vm.ProjectName);
                }
            }
        }

        private string iterationName;
        public string IterationName
        {
            get { return iterationName; }
            set
            {
                if (value != iterationName)
                {
                    iterationName = value;
                    TriggerPropertyChanged<BurndownChartViewModel>(vm => vm.IterationName);
                }
            }
        }

        public List<BurndownChartCoordinate> ActualBurndown
        {
            get
            {
                return actualBurndown;
            }
            set
            {
                if (value != actualBurndown)
                {
                    actualBurndown = value;
                    TriggerPropertyChanged<BurndownChartViewModel>(vm => vm.ActualBurndown);
                }
            }
        }

        public List<BurndownChartCoordinate> IdealBurndown
        {
            get
            {
                return idealBurndown;
            }
            set
            {
                if (value != idealBurndown)
                {
                    idealBurndown = value;
                    TriggerPropertyChanged<BurndownChartViewModel>(vm => vm.IdealBurndown);
                }
            }
        }
    }


    public class BurndownChartCoordinate : IComparable<BurndownChartCoordinate>
    {
        public float RemainingWorkEffort { get; set; }
        public DateTime TimeStampForUpdate { get; set; }

        public BurndownChartCoordinate(float remainingWorkEffort, DateTime timeStampForUpdate)
        {
            RemainingWorkEffort = remainingWorkEffort;
            TimeStampForUpdate = timeStampForUpdate;
        }

        public BurndownChartCoordinate(WorkEffortHistoryItem workEffortHistoryItem)
            : this(workEffortHistoryItem.RemainingWorkEffort, workEffortHistoryItem.TimeStampForUpdate){}

        public int CompareTo(BurndownChartCoordinate other)
        {
            return TimeStampForUpdate.CompareTo(other.TimeStampForUpdate);
        }
    }
}