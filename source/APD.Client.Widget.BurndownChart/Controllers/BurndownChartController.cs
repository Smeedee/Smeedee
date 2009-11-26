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
using APD.Client.Framework;
using APD.Client.Framework.Controllers;
using APD.Client.Widget.BurndownChart.ViewModel;
using APD.DomainModel.Framework;
using APD.DomainModel.ProjectInfo;


namespace APD.Client.Widget.BurndownChart.Controllers
{
    public class BurndownChartController : ControllerBase<BurndownChartViewModel>
    {
        public Project CurrentProject { get; private set; }
        private readonly IRepository<ProjectInfoServer> repository;
        private readonly IInvokeBackgroundWorker<IEnumerable<ProjectInfoServer>> asyncClient;

        public BurndownChartController(IRepository<ProjectInfoServer> repository,
                                       INotifyWhenToRefresh refreshNotifier, IInvokeUI uiInvoker,
                                       IInvokeBackgroundWorker<IEnumerable<ProjectInfoServer>> backgroundWorkerInvoker)
            : base(refreshNotifier, uiInvoker)
        {
            this.repository = repository;
            asyncClient = backgroundWorkerInvoker;

            LoadData();
        }

        private void LoadData()
        {
            ViewModel.IsLoading = true;
            asyncClient.RunAsyncVoid(() =>
            {
                var servers = TryGetProjectInfoServers();

                if (servers == null || servers.Count() == 0)
                {
                    ViewModel.ProjectsInRepository = false;
                }
                else if (servers.Count() > 0)
                {
                    CurrentProject = servers.First().Projects.First();
                    SetProjectStatus();
                }
                ViewModel.IsLoading = false;
            });
        }

        protected override void OnNotifiedToRefresh(object sender, RefreshEventArgs e)
        {
            LoadData();
        }

        private static bool IterationHasTasks(Iteration iteration)
        {
            return iteration.Tasks.Count() > 0;
        }

        private IEnumerable<ProjectInfoServer> TryGetProjectInfoServers()
        {
            IEnumerable<ProjectInfoServer> servers = null;
            try
            {
                servers = repository.Get(new AllSpecification<ProjectInfoServer>());
                ViewModel.HasConnectionProblems = false;
            }
            catch (Exception)
            {
                ViewModel.HasConnectionProblems = true;
            }

            return servers;
        }

        private void SetProjectStatus()
        {
            if (CurrentProject.CurrentIteration == null)
            {
                SetCurrentProjectHasNoIteration();
            }
            else if (!IterationHasTasks(CurrentProject.CurrentIteration))
            {
                SetCurrentProjectIterationHasNoTasks();
            }
            else
            {
                SetCurrentProjectAndIterationName();
                SetCurrentProjectIterationBurndownCoordinates();
            }
        }

        private void SetCurrentProjectHasNoIteration()
        {
            ViewModel.ProjectsInRepository = true;
            ViewModel.IterationInProject = false;
        }

        private void SetCurrentProjectIterationHasNoTasks()
        {
            ViewModel.ProjectsInRepository = true;
            ViewModel.IterationInProject = true;
            ViewModel.TasksInIteration = false;
        }

        private void SetCurrentProjectAndIterationName()
        {
            ViewModel.ProjectName = CurrentProject.Name;
            ViewModel.IterationName = CurrentProject.CurrentIteration.Name;
        }

        private void SetCurrentProjectIterationBurndownCoordinates()
        {
            ViewModel.ProjectsInRepository = true;
            ViewModel.IterationInProject = true;
            ViewModel.TasksInIteration = true;

            SetIdealRegressionOnViewModel();
            SetAcutalRegressionOnViewModel();
        }

        private void SetIdealRegressionOnViewModel()
        {
            var idealRegression = new List<BurndownChartCoordinate>();
            idealRegression.Add(GetIdealStartCoordinate());
            idealRegression.Add(GetIdealEndCoordinate());

            ViewModel.IdealBurndown = idealRegression;
        }

        private BurndownChartCoordinate GetIdealEndCoordinate()
        {
            return new BurndownChartCoordinate(0, CurrentProject.CurrentIteration.EndDate);
        }

        private BurndownChartCoordinate GetIdealStartCoordinate()
        {
            var startEstimatedWorkEffort = CurrentProject.CurrentIteration.GetWorkEffortEstimateForTasks();

            return new BurndownChartCoordinate(startEstimatedWorkEffort, CurrentProject.CurrentIteration.StartDate);
        }

        private void SetAcutalRegressionOnViewModel()
        {
            var coordinates = new List<BurndownChartCoordinate>();
            var allDatesInHistory = GetAllDatesWithHistory(CurrentProject.CurrentIteration);

            foreach (DateTime day in allDatesInHistory)
            {
                float hoursRemaining = GetHoursRemainingForDay(CurrentProject.CurrentIteration, day);
                coordinates.Add(new BurndownChartCoordinate(hoursRemaining, day));
            }

            MovePointsOneStepToTheRight(coordinates);
            coordinates.Add(GetStartCoordiante());
            coordinates.Sort();
            AssureLastCoordinateIsNow(coordinates);
            
            ViewModel.ActualBurndown = coordinates;
        }

        private static IEnumerable<DateTime> GetAllDatesWithHistory(Iteration iteration)
        {
            var distinctDates = (from task in iteration.Tasks
                                 from workEffortItem in task.WorkEffortHistory
                                 select workEffortItem.TimeStampForUpdate.Date).Distinct();
            
            return distinctDates;
        }

        private static float GetHoursRemainingForDay(Iteration iteration, DateTime day)
        {
            float hoursRemaining = 0;
            foreach (var task in iteration.Tasks)
            {
                if (IsDayInHistory(task, day))
                {
                    hoursRemaining += GetLastRemainingHoursForDay(task, day);
                }
                else if (IsDayBeforeHistory(task, day))
                {
                    hoursRemaining += task.WorkEffortEstimate;
                }
                else // day is a history gap or after history
                {
                    var previousDay = GetPreviousDayInHistoryForDay(task, day);
                    hoursRemaining += GetLastRemainingHoursForDay(task, previousDay);
                }
            }
            return hoursRemaining;
        }

        private static bool IsDayInHistory(Task task, DateTime day)
        {
            return task.WorkEffortHistory
                .Any(x => x.TimeStampForUpdate.Date.Equals(day.Date));
        }

        private static bool IsDayBeforeHistory(Task task, DateTime day)
        {
            return task.WorkEffortHistory.All(w => w.TimeStampForUpdate.Date >= day.Date);
        }

        private static DateTime GetPreviousDayInHistoryForDay(Task task, DateTime day)
        {
            return ( from workEffort in task.WorkEffortHistory
                     orderby workEffort.TimeStampForUpdate
                     where workEffort.TimeStampForUpdate.Date < day.Date
                     select workEffort.TimeStampForUpdate.Date ).Last();
        }

        private static float GetLastRemainingHoursForDay(Task task, DateTime day)
        {
            return ( from workEffort in task.WorkEffortHistory
                     orderby workEffort.TimeStampForUpdate
                     where workEffort.TimeStampForUpdate.Date.Equals(day.Date)
                     select workEffort.RemainingWorkEffort ).Last();
        }

        private static void AssureLastCoordinateIsNow(ICollection<BurndownChartCoordinate> coordinates)
        {
            if (coordinates.Count == 0) return;
            
            var lastCoord = coordinates.Last();
            if (lastCoord.TimeStampForUpdate.Date == DateTime.Now.Date)
            {
                coordinates.Last().TimeStampForUpdate = DateTime.Now;
            }
            else if (lastCoord.TimeStampForUpdate.Date < DateTime.Now.Date)
            {
                var newLastCoord = new BurndownChartCoordinate(lastCoord.RemainingWorkEffort, DateTime.Now);
                coordinates.Add(newLastCoord);
            }
        }

        private BurndownChartCoordinate GetStartCoordiante()
        {
            var iteration = CurrentProject.CurrentIteration;
            var startCoordinate = new BurndownChartCoordinate(iteration.GetWorkEffortEstimateForTasks(), iteration.StartDate);
            
            return startCoordinate;
        }

        private void MovePointsOneStepToTheRight(IEnumerable<BurndownChartCoordinate> actualCoordinates)
        {
            var iteration = CurrentProject.CurrentIteration;
            ViewModel.IdealBurndown.Last().TimeStampForUpdate = iteration.EndDate.AddDays(1);

            foreach (BurndownChartCoordinate coordinate in actualCoordinates)
            {
                coordinate.TimeStampForUpdate = coordinate.TimeStampForUpdate.AddDays(1);
            }
        }
    }
}