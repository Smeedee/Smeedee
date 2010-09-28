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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Smeedee.Client.Framework.Controller;
using Smeedee.Client.Framework.Services;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.ProjectInfo;
using Smeedee.Framework;
using Smeedee.Widget.BurndownChart.ViewModel;
using Smeedee.Widget.BurndownChart.ViewModels;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widget.BurndownChart.Controllers
{
    public class BurndownChartController : ControllerBase<BurndownChartViewModel>
    {

        public const string INCLUDE_WORK_ITEMS_BEFORE_ITERATION_STARTDATE = "IncludeWorkItemsBeforeIterationStartdate";
        public const string SELECTED_PROJECT_NAME = "SelectedProjectName";
        public const string DEFAULT_PROJECT_NAME = "Default Project";
        public const bool DEFAULT_INCLUDE_WORK_ITEMS_PRE_ITERATION_VALUE = false;

        private const string SETTINGS_ENTRY_NAME = "BurndownChartController";
        
        public readonly BurndownChartSettingsViewModel settingsViewModel;

        private readonly IRepository<ProjectInfoServer> repository;
        private readonly IInvokeBackgroundWorker<IEnumerable<ProjectInfoServer>> asyncClient;
        private readonly ILog logger;
        private readonly TimeSpan oneDay = new TimeSpan(1, 0, 0, 0);

        public Project CurrentProject { get; private set; }

        protected bool HasValidConfig { get { return currentConfig != null && currentConfig.IsConfigured; } }

        private List<DateTime> allDatesWithWorkItems;
        private Configuration currentConfig;
        
        public BurndownChartController(
            BurndownChartViewModel viewModel,
            BurndownChartSettingsViewModel settingsViewModel,
            IRepository<ProjectInfoServer> repository,
            Configuration configuration,
            ITimer refreshNotifier,
            IUIInvoker uiInvoker,
            IInvokeBackgroundWorker<IEnumerable<ProjectInfoServer>> asyncClient,
            ILog logger,
            IProgressbar loadingNotifier)
            : base(viewModel, refreshNotifier, uiInvoker, loadingNotifier)
        {
            Guard.Requires<ArgumentException>(settingsViewModel != null, "settingsViewModel");
            Guard.Requires<ArgumentException>(repository != null, "projectInfoServerRepository");
            Guard.Requires<ArgumentException>(configuration != null, "configuration");
            Guard.Requires<ArgumentException>(asyncClient != null, "asyncClient");
            Guard.Requires<ArgumentException>(logger != null, "logger");

            this.settingsViewModel = settingsViewModel;
            this.repository = repository;
            currentConfig = configuration;
            this.asyncClient = asyncClient;
            this.logger = logger;

            settingsViewModel.ReloadSettings.AfterExecute += (s, e) => LoadConfigAndData();

            LoadConfigAndData();
            REFRESH_INTERVAL = 30 * 60 * 1000;
            Start();
        }

        public void LoadConfigAndData()
        {
            try
            {
                SetIsLoadingConfig();
                if (HasValidConfig)
                {
                    UpdateSettingsViewModelFromConfiguration(currentConfig);
                }
                SetIsNotLoadingConfig();
                UpdateViewModel();
            }
            catch (Exception exception)
            {
                LogErrorMsg(exception);
                SetIsNotLoadingConfig();
            }
        }

        private void UpdateSettingsViewModelFromConfiguration(Configuration configuration)
        {
            uiInvoker.Invoke(() =>
            {
                settingsViewModel.SelectedProjectName = currentConfig.GetSetting(SELECTED_PROJECT_NAME).Value;
                settingsViewModel.IncludeWorkItemsBeforeIterationStartdate =
                    bool.Parse(configuration.GetSetting(INCLUDE_WORK_ITEMS_BEFORE_ITERATION_STARTDATE).Value);
                settingsViewModel.HasChanges = false;                
            });
        }
        
        private void UpdateViewModel()
        {
            if (settingsViewModel.HasChanges)
            {
                ReloadViewModel();
            }
            else
            {
                TryToLoadData();
            }
        }

        private void ReloadViewModel()
        {
            ClearViewModel();
            TryToLoadData();
        }

        private void ClearViewModel()
        {
            uiInvoker.Invoke(() =>
            {
                ViewModel.ActualBurndown.Clear();
                ViewModel.IdealBurndown.Clear();
                ViewModel.UpperWarningLimit.Clear();
                ViewModel.LowerWarningLimit.Clear();
                ViewModel.ProjectName = "";
                ViewModel.IterationName = "";
            });
        }

        private void TryToLoadData()
        {
            if (!ViewModel.IsLoading)
            {
                try
                {
                    LoadData();
                }
                catch (Exception e)
                {
                    uiInvoker.Invoke(() =>
                    {
                        ViewModel.HasConnectionProblems = true;
                    });
                    LogErrorMsg(e);
                }
            }
        }

        private void LoadData()
        {
            SetIsLoadingData();
            asyncClient.RunAsyncVoid(() =>
            {
                var servers = TryGetProjectInfoServers();

                if (NoServerExists(servers))
                {
                    SetNoServerExists();
                }
                else if (CurrentServerHasNoProject(servers))
                {
                    SetCurrentServerHasNoProject();
                }
                else
                {
                    PopulateAvailableProjects(servers);
                    if (settingsViewModel.SelectedProjectName == null || settingsViewModel.SelectedProjectName.Equals(DEFAULT_PROJECT_NAME))
                    {
                        SetSelectedProjectToFirstAvailableAndSave();
                    }

                    CurrentProject = GetSelectedProjectFromRepository(servers);

                    SetProjectStatus();
                }
                SetIsNotLoadingData();
            });
        }

        private static bool NoServerExists(IEnumerable<ProjectInfoServer> servers)
        {
            return servers == null || servers.Count() == 0;
        }

        private void SetNoServerExists()
        {
            uiInvoker.Invoke(() =>
            {
                ViewModel.ExistsAvailableServer = false;
                ViewModel.ShowErrorMessageInsteadOfChart = true;
            });
        }

        private static bool CurrentServerHasNoProject(IEnumerable<ProjectInfoServer> servers)
        {
            return servers.First().Projects.Count() == 0;
        }

        private void SetCurrentServerHasNoProject()
        {
            uiInvoker.Invoke(() =>
            {
                ViewModel.ExistsProjectsInRepository = false;
                ViewModel.ShowErrorMessageInsteadOfChart = true;
            });
        }

        private Project GetSelectedProjectFromRepository(IEnumerable<ProjectInfoServer> inputServer)
        {
            var servers = inputServer ?? TryGetProjectInfoServers();

            var matchingProject = new Project("");

            if (!NoServerExists(servers))
            {
                var listOfProjects = servers.First().Projects.ToList();
                foreach (var project in
                    listOfProjects.Where(project => project.Name.Equals(settingsViewModel.SelectedProjectName)))
                {
                    matchingProject = project;
                    matchingProject.Iterations = project.Iterations;
                }
            }
            return matchingProject;
        }

        private IEnumerable<ProjectInfoServer> TryGetProjectInfoServers()
        {
            IEnumerable<ProjectInfoServer> servers = null;

            try
            {
                servers = repository.Get(new AllSpecification<ProjectInfoServer>());
                uiInvoker.Invoke(() =>
                {
                    ViewModel.HasConnectionProblems = false;
                });
            }
            catch (Exception)
            {
                uiInvoker.Invoke(() =>
                {
                    ViewModel.HasConnectionProblems = true;
                });
            }

            return servers;
        }

        private void PopulateAvailableProjects(IEnumerable<ProjectInfoServer> servers)
        {
            var newProjects = servers.First().Projects.Select(r => r.Name).ToList();
            var oldProjects = new List<string>(settingsViewModel.AvailableProjects);

            uiInvoker.Invoke(() =>
            {
                foreach (var oldProject in oldProjects.Where(oldProject => !newProjects.Contains(oldProject)))
                {
                    settingsViewModel.AvailableProjects.Remove(oldProject);
                }

                foreach (var newProject in newProjects.Where(newProject => !oldProjects.Contains(newProject)))
                {
                    settingsViewModel.AvailableProjects.Add(newProject);
                }
            });
        }

        private void SetSelectedProjectToFirstAvailableAndSave()
        {
            uiInvoker.Invoke(() =>
            {
                settingsViewModel.SelectedProjectName = settingsViewModel.AvailableProjects.First();
            });
            currentConfig.ChangeSetting(SELECTED_PROJECT_NAME, settingsViewModel.SelectedProjectName);
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
                SetProjectIterationAndTaskExistsPropertiesOnViewModel();
                SetCurrentProjectIterationBurndownCoordinates();
            }
        }

        private void SetCurrentProjectHasNoIteration()
        {
            uiInvoker.Invoke(() =>
            {
                ViewModel.ExistsIterationInProject = false;
                ViewModel.ShowErrorMessageInsteadOfChart = true;
            });
        }

        private static bool IterationHasTasks(Iteration iteration)
        {
            return iteration.Tasks.Count() > 0;
        }

        private void SetCurrentProjectIterationHasNoTasks()
        {
            uiInvoker.Invoke(() =>
            {
                ViewModel.ExistsTasksInIteration = false;
                ViewModel.ShowErrorMessageInsteadOfChart = true;
            });
        }

        private void SetCurrentProjectAndIterationName()
        {
            uiInvoker.Invoke(() =>
            {
                ViewModel.ShowErrorMessageInsteadOfChart = false;
                ViewModel.ProjectName = CurrentProject.Name;
                ViewModel.IterationName = CurrentProject.CurrentIteration.Name;
            });
        }

        private void SetProjectIterationAndTaskExistsPropertiesOnViewModel()
        {
            uiInvoker.Invoke(() =>
            {
                ViewModel.ExistsProjectsInRepository = true;
                ViewModel.ExistsIterationInProject = true;
                ViewModel.ExistsTasksInIteration = true;
            });
        }

        private void SetCurrentProjectIterationBurndownCoordinates()
        {
            PopulateAndSortAllDatesWithWorkItems();

            SetBurndownCoordinatesOnViewModel();
        }

        private void PopulateAndSortAllDatesWithWorkItems()
        {
            allDatesWithWorkItems = GetAllDatesWithWorkItems(CurrentProject.CurrentIteration).ToList();
            allDatesWithWorkItems.Sort();
        }

        private static IEnumerable<DateTime> GetAllDatesWithWorkItems(Iteration iteration)
        {
            var distinctDates = (from task in iteration.Tasks
                                 from workEffortItem in task.WorkEffortHistory
                                 select workEffortItem.TimeStampForUpdate.Date).Distinct();

            return distinctDates;
        }

        private void SetBurndownCoordinatesOnViewModel()
        {
            SetIdealBurndownOnViewModel();
            SetUpperWarningLimitOnViewModel();
            SetLowerWarningLimitOnViewModel();
            SetAcutalBurndownOnViewModel();
        }

        private void SetIdealBurndownOnViewModel()
        {
            var idealBurndown = new List<BurndownChartCoordinate>
            {
                GetStartCoordinate(),
                GetIdealEndCoordinate()
            };
            uiInvoker.Invoke(() =>
            {
                ViewModel.IdealBurndown = idealBurndown;
            });
        }

        private BurndownChartCoordinate GetStartCoordinate()
        {
            var iteration = CurrentProject.CurrentIteration;
            if (settingsViewModel.IncludeWorkItemsBeforeIterationStartdate && allDatesWithWorkItems.Count != 0)
            {
                return new BurndownChartCoordinate(iteration.GetWorkEffortEstimateForTasks(), GetCorrectStartDateOnIncludeWorkItemsBeforeIteration(iteration));
            }
            var workEffort = GetWorkEffortFromStartDate(iteration);
            return new BurndownChartCoordinate(workEffort, iteration.StartDate);
        }

        private DateTime GetCorrectStartDateOnIncludeWorkItemsBeforeIteration(Iteration iteration)
        {
            var workItemDate = allDatesWithWorkItems.First().Date;
            var iterationDate = iteration.StartDate.Date;
            return workItemDate < iterationDate ? workItemDate : iterationDate;
        }

        private float GetWorkEffortFromStartDate(Iteration iteration)
        {
            return GetHoursRemainingForDay(iteration, iteration.StartDate.Subtract(oneDay));
        }

        private BurndownChartCoordinate GetIdealEndCoordinate()
        {
            return new BurndownChartCoordinate(0, CurrentProject.CurrentIteration.EndDate);
        }

        private void SetUpperWarningLimitOnViewModel()
        {
            var tempListOfCoordinates = new List<BurndownChartCoordinate>();

            var tempCoordinate = GetStartCoordinate();

            tempListOfCoordinates.Add(
                new BurndownChartCoordinate(
                    CalculateUpperRemainingWorkEffort(tempCoordinate),
                    tempCoordinate.TimeStampForUpdate)
            );

            tempCoordinate = GetIdealEndCoordinate();

            tempListOfCoordinates.Add(
                new BurndownChartCoordinate(
                    CalculateUpperRemainingWorkEffort(tempCoordinate),
                    tempCoordinate.TimeStampForUpdate)
            );

            uiInvoker.Invoke(() => { ViewModel.UpperWarningLimit = tempListOfCoordinates; });
        }

        private float CalculateUpperRemainingWorkEffort(BurndownChartCoordinate tempCoordinate)
        {
            return tempCoordinate.RemainingWorkEffort *
                    ViewModel.BurndownUpperWarningLimitInPercent;
        }

        private void SetLowerWarningLimitOnViewModel()
        {
            var tempListOfCoordinates = new List<BurndownChartCoordinate>();

            var tempCoordinate = GetStartCoordinate();

            tempListOfCoordinates.Add(
                new BurndownChartCoordinate(
                    CalculateLowerRemainingWorkEffort(tempCoordinate),
                    tempCoordinate.TimeStampForUpdate));

            tempCoordinate = GetIdealEndCoordinate();

            tempListOfCoordinates.Add(
                new BurndownChartCoordinate(
                    CalculateLowerRemainingWorkEffort(tempCoordinate),
                    tempCoordinate.TimeStampForUpdate
                )
             );
            uiInvoker.Invoke(() => { ViewModel.LowerWarningLimit = tempListOfCoordinates; });
        }

        private float CalculateLowerRemainingWorkEffort(BurndownChartCoordinate tempCoordinate)
        {
            return tempCoordinate.RemainingWorkEffort *
                    ViewModel.BurndownLowerWarningLimitInPercent;
        }

        private void SetAcutalBurndownOnViewModel()
        {
            var coordinates = CreateListOfActualCoordinatesToUse();

            MovePointsOneStepToTheRight(coordinates);
            coordinates.Add(GetStartCoordinate());
            coordinates.Sort();
            AssureLastCoordinateIsNow(coordinates);

            uiInvoker.Invoke(() => ViewModel.ActualBurndown = coordinates);
        }

        private List<BurndownChartCoordinate> CreateListOfActualCoordinatesToUse()
        {
            return (from day in allDatesWithWorkItems
                    where (day.Date >= CurrentProject.CurrentIteration.StartDate.Date) || (settingsViewModel.IncludeWorkItemsBeforeIterationStartdate)
                    let hoursRemaining = GetHoursRemainingForDay(CurrentProject.CurrentIteration, day)
                    select new BurndownChartCoordinate(hoursRemaining, day)).ToList();
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

        private static float GetLastRemainingHoursForDay(Task task, DateTime day)
        {
            return (from workEffort in task.WorkEffortHistory
                    orderby workEffort.TimeStampForUpdate
                    where workEffort.TimeStampForUpdate.Date.Equals(day.Date)
                    select workEffort.RemainingWorkEffort).Last();
        }

        private static bool IsDayBeforeHistory(Task task, DateTime day)
        {
            return task.WorkEffortHistory.All(w => w.TimeStampForUpdate.Date >= day.Date);
        }

        private static DateTime GetPreviousDayInHistoryForDay(Task task, DateTime day)
        {
            return (from workEffort in task.WorkEffortHistory
                    orderby workEffort.TimeStampForUpdate
                    where workEffort.TimeStampForUpdate.Date < day.Date
                    select workEffort.TimeStampForUpdate.Date).Last();
        }

        private void MovePointsOneStepToTheRight(IEnumerable<BurndownChartCoordinate> actualCoordinates)
        {
            var iteration = CurrentProject.CurrentIteration;
            uiInvoker.Invoke(() =>
            {
                ViewModel.IdealBurndown.Last().TimeStampForUpdate = iteration.EndDate.AddDays(1);
                ViewModel.LowerWarningLimit.Last().TimeStampForUpdate = iteration.EndDate.AddDays(1);
                ViewModel.UpperWarningLimit.Last().TimeStampForUpdate = iteration.EndDate.AddDays(1);
            });

            foreach (BurndownChartCoordinate coordinate in actualCoordinates)
            {
                coordinate.TimeStampForUpdate = coordinate.TimeStampForUpdate.AddDays(1);
            }
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

        protected override void OnNotifiedToRefresh(object sender, EventArgs e)
        {
            ReloadViewModel();
        }

        public void ConfigurationChanged(Configuration newConfig)
        {
            currentConfig = newConfig;
            UpdateSettingsViewModelFromConfiguration(newConfig);
            ReloadViewModel();
        }

        public void BeforeSaving()
        {
            currentConfig.ChangeSetting(INCLUDE_WORK_ITEMS_BEFORE_ITERATION_STARTDATE,
                                                        settingsViewModel.IncludeWorkItemsBeforeIterationStartdate.ToString());
            currentConfig.ChangeSetting(SELECTED_PROJECT_NAME, settingsViewModel.SelectedProjectName);
            currentConfig.IsConfigured = true;
        }


        public void AfterSave(object sender, EventArgs e)
        {
            uiInvoker.Invoke(() =>
            {
                settingsViewModel.HasChanges = false;
            });
            UpdateSettingsViewModelFromConfiguration(currentConfig);
            ReloadViewModel();
        }

        public static Configuration GetDefaultConfiguration()
        {
            var config = new Configuration(SETTINGS_ENTRY_NAME);
            config.NewSetting(INCLUDE_WORK_ITEMS_BEFORE_ITERATION_STARTDATE, "false");
            config.NewSetting(SELECTED_PROJECT_NAME, DEFAULT_PROJECT_NAME);
            config.IsConfigured = false;
            return config;
        }

        private void LogErrorMsg(Exception exception)
        {
            logger.WriteEntry(new ErrorLogEntry
            {
                Message = exception.ToString(),
                Source = GetType().ToString(),
                TimeStamp = DateTime.Now
            });
        }
    }
}