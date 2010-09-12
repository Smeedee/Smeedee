using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Smeedee.Client.Framework.Services;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.Holidays;
using Smeedee.DomainModel.ProjectInfo;
using Smeedee.Widget.ProjectInfo.ViewModels;

namespace Smeedee.Widget.ProjectInfo
{

    public class WorkingDaysLeftController
    {
        public const string CONFIG_NAME = "project-info";
        public const string SETTING_IS_MANUALLY_CONFIGURED = "use-config-repo";
        public const string SETTING_END_DATE = "end-date";
        public const string SETTING_SERVER = "project-server";
        public const string SETTING_PROJECT = "project";
        public const string SETTING_NON_WORK_DAYS = "non-work-days";

        private const int REFRESH_INTERVAL = 10 * 1000;

        private bool gotHolidays;
        private bool gotProjectInfo;

        private readonly IAsyncRepository<ProjectInfoServer> projectInforepository;
        private readonly ILog logger;
        private readonly IProgressbar loadingNotifier;
        private readonly ITimer refreshNotifier;
        private readonly WorkingDaysLeftViewModel leftViewModel;
        private readonly WorkingDaysLeftSettingsViewModel settingsViewModel;
        private readonly IAsyncRepository<Holiday> holidayRepository;

        private ProjectInfoServer projectInfoServerToUse;
        private Configuration config;
        private DateTime endDateToUse;
        private Dictionary<string, List<string>> allProjectInfoServersAndProjects;
        private int Rand;

        protected bool HasValidConfig { get { return config != null && config.IsConfigured; } }

        public WorkingDaysLeftController(
            WorkingDaysLeftViewModel leftViewModel,
            WorkingDaysLeftSettingsViewModel settingsViewModel,
            IAsyncRepository<ProjectInfoServer> projectInforepository,
            IAsyncRepository<Holiday> holidayRepository,
            ITimer refreshNotifier,
            ILog logger,
            IProgressbar loadingNotifier,
            Configuration config)
        {
            this.leftViewModel = leftViewModel;
            this.settingsViewModel = settingsViewModel;
            this.holidayRepository = holidayRepository;
            this.projectInforepository = projectInforepository;
            this.logger = logger;
            this.loadingNotifier = loadingNotifier;
            this.refreshNotifier = refreshNotifier;

            this.Rand = new Random((int)DateTime.Now.Ticks).Next();

            projectInforepository.GetCompleted += GotProjectInfo;
            holidayRepository.GetCompleted += GotHolidays;

            this.settingsViewModel.RefreshAvailableServers.ExecuteDelegate = OnRefreshAvailableServers;
            this.settingsViewModel.ReloadSettings.ExecuteDelegate = settingsViewModel.Reset;

            refreshNotifier.Elapsed += refreshNotifier_Elapsed;
            settingsViewModel.PropertyChanged += RefreshAvailableProjectsWhenSelectedServerChanges;
            refreshNotifier.Start(10000);

            OnRefreshAvailableServers();
            SetConfigAndUpdate(config);
        }

        private void RefreshAvailableProjectsWhenSelectedServerChanges(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("SelectedServer") && allProjectInfoServersAndProjects != null &&
                settingsViewModel.SelectedServer != null &&
                allProjectInfoServersAndProjects.ContainsKey(settingsViewModel.SelectedServer))
            {
                settingsViewModel.AvailableProjects =
                    allProjectInfoServersAndProjects[settingsViewModel.SelectedServer];
            }
        }

        private void OnRefreshAvailableServers()
        {
            projectInforepository.BeginGet(new AllSpecification<ProjectInfoServer>());
        }

        private void LoadProjectInfoAsync()
        {
            projectInforepository.BeginGet(new ProjectInfoServerByName(settingsViewModel.SelectedServer));
        }

        private void GotProjectInfo(object sender, GetCompletedEventArgs<ProjectInfoServer> e)
        {
            if (e.Specification is AllSpecification<ProjectInfoServer>)
            {
                allProjectInfoServersAndProjects = e.Result.ToDictionary(s => s.Name,
                                                                         s => s.Projects.Select(p => p.Name).ToList());
                settingsViewModel.AvailableServers = allProjectInfoServersAndProjects.Keys.ToList();
            }
            gotProjectInfo = true;
            projectInfoServerToUse = e.Result.FirstOrDefault();
            NextStep();
        }

        private void LoadHolidaysAsync()
        {
            holidayRepository.BeginGet(new HolidaySpecification
                                           {
                                               StartDate = DateTime.Now.Date,
                                               EndDate = endDateToUse
                                           });
        }

        private void GotHolidays(object sender, GetCompletedEventArgs<Holiday> e)
        {
            gotHolidays = true;
            leftViewModel.Holidays = e.Result;
            NextStep();
        }

        private void NextStep()
        {
            if (!HasValidConfig)
                return;

            loadingNotifier.ShowInView("Loading data from server...");
            leftViewModel.IsLoading = !leftViewModel.HasInformationToShow;
            
            if (settingsViewModel.IsManuallyConfigured)
                NextStepForManualConfig();
            else
                NextStepForServerConfig();
        }

        private void NextStepForManualConfig()
        {
            if (gotHolidays)
            {
                UpdateEndDateOnViewModels();
                OnFinishedUpdating();
            } else
            {
                SetEndDateToUseFromConfig();
                GetHolidays();
            }
        }

        private void NextStepForServerConfig()
        {
            if (!gotHolidays && !gotProjectInfo)
            {
                LoadProjectInfoAsync();
            }
            else if (!gotHolidays && gotProjectInfo)
            {
                SetEndDateToUseFromProjectInfo();
                GetHolidays();
            }
            else if (gotHolidays && gotProjectInfo)
            {
                UpdateEndDateOnViewModels();
                OnFinishedUpdating();
            }
        }

        private void GetHolidays()
        {
            if (IsOnOvertime())
            {
                UseEmptyListOfHolidays();
            }
            else
            {
                LoadHolidaysAsync();
            }
        }

        private bool IsOnOvertime()
        {
            return endDateToUse < DateTime.Now.Date;
        }

        private void UseEmptyListOfHolidays()
        {
            leftViewModel.Holidays = new List<Holiday>();
            gotHolidays = true;
            NextStep();
        }

        private void SetEndDateToUseFromConfig()
        {
            try
            {
                endDateToUse = DateTime.Parse(config.GetSetting(SETTING_END_DATE).Value, CultureInfo.InvariantCulture);
                leftViewModel.HasInformationToShow = true;
            }
            catch (Exception e)
            {
                leftViewModel.HasInformationToShow = false;
                OnFinishedUpdating();
                LogException(e);
            }
        }

        private void OnFinishedUpdating()
        {
            loadingNotifier.HideInView();
            gotHolidays = false;
            gotProjectInfo = false;
            leftViewModel.IsLoading = false;
        }

        private void refreshNotifier_Elapsed(object sender, EventArgs e)
        {
            if (!gotHolidays && !gotProjectInfo)
                NextStep();
        }

        private void UpdateEndDateOnViewModels()
        {
            settingsViewModel.SelectedEndDate = endDateToUse;
            leftViewModel.EndDate = endDateToUse;

            var nonWorkingDays = settingsViewModel.NonWorkWeekDays.Where(d => d.IsNotWorkingDay).Select(d => d.Day);
            leftViewModel.UpdateDaysRemaining(nonWorkingDays);
        }

        private void UpdateSettingsViewModelFromConfig()
        {
            if (!HasValidConfig)
            {
                SetNoInfoFlag();
                return;
            }

            settingsViewModel.IsManuallyConfigured = bool.Parse(config.GetSetting(SETTING_IS_MANUALLY_CONFIGURED).Value);
            settingsViewModel.SelectedServer = config.GetSetting(SETTING_SERVER).Value;
            settingsViewModel.SelectedProject = config.GetSetting(SETTING_PROJECT).Value;
            settingsViewModel.SelectedEndDate = DateTime.Parse(config.GetSetting(SETTING_END_DATE).Value, CultureInfo.InvariantCulture);

            var nonWorkingDays = config.GetSetting(SETTING_NON_WORK_DAYS).Vals;
            foreach (var workingDay in settingsViewModel.NonWorkWeekDays)
            {
                workingDay.IsNotWorkingDay = nonWorkingDays.Contains(workingDay.Day.ToString());
            }

            settingsViewModel.SetResetPoint();
        }

        public void BeforeSave()
        {
            settingsViewModel.SetResetPoint();
            config = GetConfigFromSettingsViewModel();
            RefreshViewModelsWithDataFromSettingsView();
        }

        private void RefreshViewModelsWithDataFromSettingsView()
        {
            NextStep();
        }

        private Configuration GetConfigFromSettingsViewModel()
        {
            config.NewSetting(SETTING_IS_MANUALLY_CONFIGURED, settingsViewModel.IsManuallyConfigured.ToString());
            config.NewSetting(SETTING_END_DATE, settingsViewModel.SelectedEndDate.ToString(CultureInfo.InvariantCulture));
            config.NewSetting(SETTING_SERVER, settingsViewModel.SelectedServer);
            config.NewSetting(SETTING_PROJECT, settingsViewModel.SelectedProject);
            config.NewSetting(SETTING_NON_WORK_DAYS,
                                 settingsViewModel.NonWorkWeekDays.Where(kv => kv.IsNotWorkingDay).Select(kv => kv.Day.ToString())
                                     .ToArray());
            config.IsConfigured = true;
            return config;
        }

        private void SetEndDateToUseFromProjectInfo()
        {
            try
            {
                var projectToUse = projectInfoServerToUse.Projects.Single(p => p.Name.Equals(settingsViewModel.SelectedProject));
                GetEndDateFromProjectIteration(projectToUse);
            }
            catch (Exception e)
            {
                SetNoInfoFlag();
                LogException(new Exception("The configuration is bogus! Tried to use SelectedProject: " + settingsViewModel.SelectedProject, e));
            }
        }

        private void GetEndDateFromProjectIteration(Project projectToUse)
        {
            endDateToUse = projectToUse.CurrentIteration.EndDate;
            leftViewModel.HasInformationToShow = true;
        }

        private void SetNoInfoFlag()
        {
            var bogusEndDate = DateTime.Now.AddDays(1);
            endDateToUse = bogusEndDate;
            leftViewModel.HasInformationToShow = false;
            leftViewModel.IsLoading = false;
        }

        private void LogException(Exception exception)
        {
            logger.WriteEntry(ErrorLogEntry.Create(this, exception.ToString()));
        }

        public void Stop()
        {
            refreshNotifier.Stop();
        }

        public void Start()
        {
            refreshNotifier.Start(REFRESH_INTERVAL);
        }

        public static Configuration GetDefaultConfiguration()
        {
            var config = new Configuration(CONFIG_NAME);
            config.NewSetting(SETTING_IS_MANUALLY_CONFIGURED, "True");
            config.NewSetting(SETTING_END_DATE, DateTime.Now.AddDays(3).ToString(CultureInfo.InvariantCulture));
            config.NewSetting(SETTING_SERVER, "");
            config.NewSetting(SETTING_PROJECT, "");
            config.NewSetting(SETTING_NON_WORK_DAYS, new [] {"Saturday", "Sunday"});
            config.IsConfigured = false;
            return config;
        }

        public void SetConfigAndUpdate(Configuration configuration)
        {
            config = configuration;
            UpdateSettingsViewModelFromConfig();
            NextStep();
        }
    }
}
