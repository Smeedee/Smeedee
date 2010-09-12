using System;
using System.Collections.Generic;
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
    public class WorkingDaysLeftRxController
    {
        public const string CONFIG_NAME = "project-info";
        public const string SETTING_USE_CONFIG_END_DATE = "use-config-repo";
        public const string SETTING_END_DATE = "end-date";
        public const string SETTING_SERVER = "project-server";
        public const string SETTING_PROJECT = "project";
        public const string SETTING_NON_WORK_DAYS = "non-work-days";

        private readonly IAsyncRepository<ProjectInfoServer> _projectInforepository;
        private readonly ILog _logger;
        private readonly IAsyncRepository<Configuration> _configRepository;
        private readonly IPersistDomainModels<Configuration> _configPersisterRepository;
        private readonly WorkingDaysLeftViewModel _viewModel;
        private readonly WorkingDaysLeftSettingsViewModel _settingsViewModel;
        private readonly IAsyncRepository<Holiday> _holidayRepository;

        private Dictionary<string, List<string>> _allProjectInfoServersAndProjects;
        private DateTime _endDateToUse;

        public WorkingDaysLeftRxController(
            WorkingDaysLeftViewModel viewModel, 
            WorkingDaysLeftSettingsViewModel settingsViewModel,
            IAsyncRepository<ProjectInfoServer> projectInforepository,
            IAsyncRepository<Configuration> configRepository,
            IAsyncRepository<Holiday> holidayRepository, 
            IPersistDomainModels<Configuration> configPersisterRepository,
            ITimer refreshNotifier,
            ILog logger)
        {
            _viewModel = viewModel;
            _settingsViewModel = settingsViewModel;
            this._holidayRepository = holidayRepository;
            this._projectInforepository = projectInforepository;
            this._logger = logger;
            this._configRepository = configRepository;
            this._configPersisterRepository = configPersisterRepository;

            SetupReactiveEvents();
        }

        private void SetupReactiveEvents()
        {
            var repositoryResponseEvent = 
                Observable.FromEvent<GetCompletedEventArgs<ProjectInfoServer>>(_projectInforepository, "GetCompleted");

            var holidayResponseEvent =
                Observable.FromEvent<GetCompletedEventArgs<Holiday>>(_holidayRepository, "GetCompleted");

            var configReceivedEvent =
                Observable.FromEvent<GetCompletedEventArgs<Configuration>>(_configRepository, "GetCompleted");

            var updatingAllAvailableProjectServersCase = 
                repositoryResponseEvent.
                Where(e => e.EventArgs.Specification is AllSpecification<ProjectInfoServer>).
                Subscribe(e => UpdateAvailableProjectInfoServers(e.EventArgs.Result), OnError );

            var projectServerInfoReceivedEvent =
                repositoryResponseEvent.
                    Where(e => e.EventArgs.Specification.Equals(new ProjectInfoServerByName(_settingsViewModel.SelectedServer)));

            var validConfigReceivedEvent =
                configReceivedEvent.
                Where(e => ConfigIsValid(e.EventArgs.Result)).
                Select(e=>e.EventArgs.Result.First());

            validConfigReceivedEvent.Subscribe(UpdateSettingsViewModelFromConfig, OnError);

            var manuallyConfiguredEventCase = 
                validConfigReceivedEvent.
                Where(e => _settingsViewModel.IsManuallyConfigured).
                Do(GetEndDateFromConfig).
                Subscribe(e => BeginGetHolidays(), OnError);

            var projectConfiguredEventCase =
                validConfigReceivedEvent.
                    Where(ConfigSaysToUseProjectInfo).
                    Do(BeginGetProjectInfo).
                    CombineLatest(projectServerInfoReceivedEvent, (c, p) => p).
                    Select(ProjectServerInformation).
                    Where(ProjectServerInfoIsValid).
                    Select(GetConfiguredProject).
                    Do(GetEndDateFromProject).
                    Subscribe(p=>BeginGetHolidays(), OnError);
        }

        private IEnumerable<ProjectInfoServer> ProjectServerInformation(IEvent<GetCompletedEventArgs<ProjectInfoServer>> e)
        {
            return e.EventArgs.Result;
        }

        private bool ConfigSaysToUseProjectInfo(Configuration config)
        {
            return _settingsViewModel.IsManuallyConfigured == false;
        }

        private Project GetConfiguredProject(IEnumerable<ProjectInfoServer> allProjectInfoServers)
        {
            return allProjectInfoServers.First().Projects.Single(p => p.Name.Equals(_settingsViewModel.SelectedProject));
        }

        private bool ProjectServerInfoIsValid(IEnumerable<ProjectInfoServer> p)
        {
            return p != null && p.Count() == 1;
        }

        private void GetEndDateFromProject(Project projectServerInfo)
        {
            _endDateToUse = projectServerInfo.CurrentIteration.EndDate;
        }

        private void BeginGetProjectInfo(Configuration config)
        {
            _projectInforepository.BeginGet(new ProjectInfoServerByName(_settingsViewModel.SelectedServer));
        }

        private void UpdateAvailableProjectInfoServers(IEnumerable<ProjectInfoServer> servers)
        {
            _allProjectInfoServersAndProjects = servers.ToDictionary(s => s.Name, s => s.Projects.Select(p => p.Name).ToList());

            if (_allProjectInfoServersAndProjects != null)
                _settingsViewModel.AvailableServers = _allProjectInfoServersAndProjects.Keys.ToList();
        }

        private void BeginGetHolidays()
        {
            _holidayRepository.BeginGet(new HolidaySpecification
                                           {
                                               StartDate = DateTime.Now.Date,
                                               EndDate = _settingsViewModel.SelectedEndDate
                                           });
        }

        private void GetEndDateFromConfig(Configuration config)
        {
            _endDateToUse = DateTime.Parse(config.GetSetting(SETTING_END_DATE).Value, CultureInfo.InvariantCulture);
        }

        private void UpdateSettingsViewModelFromConfig(Configuration configuration)
        {
            _settingsViewModel.IsManuallyConfigured = bool.Parse(configuration.GetSetting(SETTING_USE_CONFIG_END_DATE).Value);
            _settingsViewModel.SelectedServer = configuration.GetSetting(SETTING_SERVER).Value;
            _settingsViewModel.SelectedProject = configuration.GetSetting(SETTING_PROJECT).Value;
            if (_settingsViewModel.IsManuallyConfigured)
                _settingsViewModel.SelectedEndDate = DateTime.Parse(configuration.GetSetting(SETTING_END_DATE).Value, CultureInfo.InvariantCulture);
                    
            foreach (string nonWorkingDay in configuration.GetSetting(SETTING_NON_WORK_DAYS).Vals)
            {
                var dayFromSetting = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), nonWorkingDay, true);
                _settingsViewModel.NonWorkWeekDays.First(d => d.Day == dayFromSetting).IsNotWorkingDay = true;
            }
        }

        private bool ConfigIsValid(IEnumerable<Configuration> configurations)
        {
            bool hasValues =  configurations != null &&
                configurations.Count() > 0 &&
                configurations.First().ContainsSetting(SETTING_USE_CONFIG_END_DATE);

            if( !hasValues )
                throw new ArgumentException("The configuration is not valid. It is missing values. Please configure the widget from the settings view.");

            bool isToUseManualEndDate = bool.Parse(configurations.First().GetSetting(SETTING_USE_CONFIG_END_DATE).Value);

           // bool manualEndDateIsValid = isToUseManualEndDate && 

            // bool projectSetupIsValid = ....


            return true;

        }

        private void OnError(Exception exception)
        {
            // This method is used to catch all errors that oc
            _viewModel.HasInformationToShow = false;
            _logger.WriteEntry(new ErrorLogEntry(this.ToString(), exception.ToString()));
        }

    }
}
