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
using System.Windows;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.ProjectInfo;

namespace Smeedee.Widget.BurndownChart.ViewModel
{
    public class BurndownChartViewModel : AbstractViewModel
    {
        public readonly float BurndownUpperWarningLimitInPercent = 1.2f;
        public readonly float BurndownLowerWarningLimitInPercent = 0.8f;
        private List<BurndownChartCoordinate> actualBurndown;
        private List<BurndownChartCoordinate> idealBurndown;
        
        public BurndownChartViewModel()
        {
            actualBurndown = new List<BurndownChartCoordinate>();
            InitializeIdealBurndown();
            InitializeUpperWarningLimit();
            InitializeLowerWarningLimit();

            ExistsProjectsInRepository = false;
            ExistsIterationInProject = false;
            ExistsTasksInIteration = false;
            ExistsAvailableServer = false;
            showErrorMessageInsteadOfChart = true;
//            errorMessage = "Burndown chart coordinates have not yet been set on the ViewModel";
            errorMessage = "Loading project...";
        }

        private void InitializeIdealBurndown()
        {
            idealBurndown = new List<BurndownChartCoordinate>
            {
                new BurndownChartCoordinate(1, DateTime.Today),
                new BurndownChartCoordinate(0, DateTime.Today.AddDays(1))
            };
        }

        private void InitializeUpperWarningLimit()
        {
            upperWarningLimit = new List<BurndownChartCoordinate>
            {
                new BurndownChartCoordinate(BurndownUpperWarningLimitInPercent, DateTime.Today),
                new BurndownChartCoordinate(0, DateTime.Today.AddDays(1))
            };
        }

        private void InitializeLowerWarningLimit()
        {
            lowerWarningLimit = new List<BurndownChartCoordinate>
            {
                new BurndownChartCoordinate(BurndownLowerWarningLimitInPercent, DateTime.Today),
                new BurndownChartCoordinate(0, DateTime.Today.AddDays(1))
            };
        }

        private bool existsProjectsInRepository;

        public bool ExistsProjectsInRepository
        {
            get { return existsProjectsInRepository; }
            set
            {
                if (!value)
                {
                    ErrorMessage = "No projects with the given name exists in the repository";
                }
                if (value != existsProjectsInRepository)
                {
                    existsProjectsInRepository = value;
                    TriggerPropertyChanged("ExistsProjectsInRepository");
                }
            }
        }

        private bool existsIterationInProject;

        public bool ExistsIterationInProject
        {
            get { return existsIterationInProject; }
            set
            {
                if (!value)
                {
                    ErrorMessage = "There are no iterations in the given project";
                }
                if (value != existsIterationInProject)
                {
                    existsIterationInProject = value;
                    TriggerPropertyChanged("ExistsIterationInProject");
                }
            }
        }

        private bool existsTasksInIteration;

        public bool ExistsTasksInIteration
        {
            get { return existsTasksInIteration; }
            set
            {
                if (!value)
                {
                    ErrorMessage = "There are no tasks for the current iteration for the given project";
                }
                if (value != existsTasksInIteration)
                {
                    existsTasksInIteration = value;
                    TriggerPropertyChanged("ExistsTasksInIteration");
                }
            }
        }

        private bool existsAvailableServer;

        public bool ExistsAvailableServer
        {
            get { return existsAvailableServer; }
            set
            {
                if (!value)
                {
                    ErrorMessage = "There are no available servers";
                }
                if (value != existsAvailableServer)
                {
                    existsAvailableServer = value;
                    TriggerPropertyChanged<BurndownChartViewModel>(t => t.ExistsAvailableServer);
                }
            }
        }

        private string errorMessage;

        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                if (value != errorMessage)
                {
                    errorMessage = value;
                    TriggerPropertyChanged("ErrorMessage");
                }
            }
        }

        private bool showErrorMessageInsteadOfChart;

        public bool ShowErrorMessageInsteadOfChart
        {
            get { return showErrorMessageInsteadOfChart; }
            set
            {
                if (value != showErrorMessageInsteadOfChart)
                {
                    showErrorMessageInsteadOfChart = value;
                    TriggerPropertyChanged("ShowErrorMessageInsteadOfChart");
                }
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
                    TriggerPropertyChanged("ProjectName");
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
                    TriggerPropertyChanged("IterationName");
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
                    TriggerPropertyChanged("ActualBurndown");
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
                    TriggerPropertyChanged("IdealBurndown");
                }
            }
        }

        private List<BurndownChartCoordinate> upperWarningLimit;

        public List<BurndownChartCoordinate> UpperWarningLimit
        {
            get { return upperWarningLimit; }
            set
            {
                if (value != upperWarningLimit)
                {
                    upperWarningLimit = value;
                    TriggerPropertyChanged("UpperWarningLimit");
                }
            }
        }

        private List<BurndownChartCoordinate> lowerWarningLimit;

        public List<BurndownChartCoordinate> LowerWarningLimit
        {
            get { return lowerWarningLimit; }
            set
            {
                if (value != lowerWarningLimit)
                {
                    lowerWarningLimit = value;
                    TriggerPropertyChanged("LowerWarningLimit");
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