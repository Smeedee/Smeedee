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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Smeedee.Client.Framework.Controller;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.Users;
using Smeedee.Widget.CI.ViewModels;
using TinyMVVM.Framework;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widget.CI.Controllers
{
    public class CIController : ControllerBase<CIViewModel>
    {
        private const int DB_CACHE_EXPIRATION_IN_MS = 33 * 1000;

        private readonly IRepository<User> userRepository;
        private CISettingsViewModel settingsViewModel;

        private IInvokeBackgroundWorker<IEnumerable<CIProject>> asyncClient;
        private IEnumerable<CIProject> projects = new List<CIProject>();
        private readonly ILog logger;
        private IRepository<CIServer> ciServerRepository;
        private IRepository<Configuration> ciConfigRepository;
        private IPersistDomainModelsAsync<Configuration> ciConfigPersister;
        private SettingsHandler settings;
        private DateTime lastDbCheck = DateTime.MinValue;

        private bool isSoundEnabled = false;

        #region Constructor

        public CIController(CIViewModel viewModel, 
            CISettingsViewModel settingsViewModel, 
            IRepository<User> userRepository, 
            IRepository<CIServer> ciServerRepository,
            IRepository<Configuration> ciConfigRepository,
            IPersistDomainModelsAsync<Configuration> ciConfigPersister,
            IInvokeBackgroundWorker<IEnumerable<CIProject>> asyncClient, 
            ITimer timer, 
            IUIInvoker uiInvoke, 
            ILog logger, 
            IProgressbar progressbar)
            : base(viewModel, timer, uiInvoke, progressbar)
        {
            this.settingsViewModel = settingsViewModel;
            this.userRepository = userRepository;
            this.asyncClient = asyncClient;
            this.ciServerRepository = ciServerRepository;
            this.ciConfigRepository = ciConfigRepository;
            this.ciConfigPersister = ciConfigPersister;
            this.logger = logger;

            settings = new SettingsHandler(settingsViewModel);

            settingsViewModel.SaveSettings.AfterExecute += (o, e) => LoadData();

            ciConfigPersister.SaveCompleted += ConfigPersisterRepositorySaveCompleted;
            settingsViewModel.SaveSettings.ExecuteDelegate = Save;
            settingsViewModel.ReloadSettings = new DelegateCommand(() => settings.Reset());

            var dummyServers = new List<CIServer>() { new CIServer("Loading server list", "") };
            var dummyConfig = settings.GetDummyConfig(dummyServers);
            settingsViewModel.Servers = WrapServersAndConfig(dummyServers, dummyConfig);
            settings.LoadIntoViewModel(dummyConfig);

            Start();
            LoadData();
        }

        #endregion
        
        private void LoadData()
        {
            asyncClient.RunAsyncVoid(() =>
            {
                SetIsLoadingData();

                projects = TryGetProjects();
                LoadDataIntoViewModel(projects);

                SetIsNotLoadingData();
            });
        }

        public IEnumerable<CIProject> GetSelectedAndActiveProjects()
        {
            GetServersFromDbOrCache();
            settingsViewModel.EachProject(project => MakePlaceholderBuildIfNeeded(project.Project));

            return from server in settingsViewModel.Servers
                   from project in server.Projects
                   where project.IsSelected && ProjectIsActive(project)
                   select project.Project;
        }

        private void GetServersFromDbOrCache()
        {
            bool cacheHasExpired = (DateTime.Now - lastDbCheck).TotalMilliseconds > DB_CACHE_EXPIRATION_IN_MS;
            if (cacheHasExpired)
            {
                lastDbCheck = DateTime.Now;
                var newServers = ciServerRepository.Get(new AllSpecification<CIServer>());
                var config = settings.LoadConfigFromDb(ciConfigRepository, newServers);

                uiInvoker.Invoke(() =>
                {
                    settingsViewModel.Servers = WrapServersAndConfig(newServers, config);
                    settings.LoadIntoViewModel(config);
                    settings.SetResetPoint();
                });
            }
        }

        private bool ProjectIsActive(ProjectConfigViewModel projectWrap)
        {
            if (!settingsViewModel.FilterInactiveProjects)
                return true;
            return projectWrap.Project.LatestBuild.StartTime > DateTime.Now.AddDays(-settingsViewModel.InactiveProjectThreshold);
        }

        private void MakePlaceholderBuildIfNeeded(CIProject project)
        {
            //We want to display projects with no builds - presumably such a project is brand new.
            if (project.LatestBuild == null)
            {
                project.AddBuild(new Build()
                {
                    Status = DomainModel.CI.BuildStatus.Unknown,
                    StartTime = DateTime.Now,
                    FinishedTime = DateTime.Now
                });
            }
        }

        private void Save()
        {
            settings.SetResetPoint();
            lastDbCheck = DateTime.Now;

            SetIsSavingConfig();

            ciConfigPersister.Save(settings.GetUpdatedConfig());
        }

        private void ConfigPersisterRepositorySaveCompleted(object sender, SaveCompletedEventArgs e)
        {
            SetIsNotSavingConfig();
        }

        private ObservableCollection<ServerConfigViewModel> WrapServersAndConfig(IEnumerable<CIServer> servers, Configuration config)
        {
            var wrapped = (from server in servers select new ServerConfigViewModel(server, config));
            return new ObservableCollection<ServerConfigViewModel>(wrapped);
        }

        protected override void OnNotifiedToRefresh(object sender, EventArgs e)
        {
            if (!ViewModel.IsLoading)
                LoadData();
        }

        private IEnumerable<CIProject> TryGetProjects()
        {
            try
            {
                projects = GetSelectedAndActiveProjects();
                ViewModel.HasConnectionProblems = false;
            }
            catch( Exception exception )
            {
                ViewModel.HasConnectionProblems = true;
                logger.WriteEntry(new ErrorLogEntry()
                                      {
                                          Message = exception.ToString(),
                                          Source = this.GetType().ToString(),
                                          TimeStamp = DateTime.Now
                                      });
            }
            projects = SortProjects(projects);

            return projects;
        }

        private IOrderedEnumerable<CIProject> SortProjects(IEnumerable<CIProject> activeProjects)
        {
            return from project in activeProjects
                   orderby project
                   select project;
        }

        private void LoadDataIntoViewModel(IEnumerable<CIProject> projects)
        {
            uiInvoker.Invoke(() =>
            {
                ViewModel.Data.Clear();
                foreach (var projectInfo in projects)
                    AddProject(projectInfo);
            });
        }

        private void AddProject(CIProject CIProject)
        {
            var model = new ProjectInfoViewModel();
            AddProjectInfo(model, CIProject);
            ViewModel.Data.Add(model);
        }

        private void AddProjectInfo(ProjectInfoViewModel model, CIProject CIProject)
        {
            BuildViewModel buildModel = model.LatestBuild;

            model.ProjectName = CIProject.ProjectName;
            model.IsSoundEnabled = isSoundEnabled;
            buildModel.StartTime = CIProject.LatestBuild.StartTime;
            buildModel.FinishedTime = CIProject.LatestBuild.FinishedTime;
            buildModel.Status = (BuildStatus) CIProject.LatestBuild.Status;
            SetBuildTrigger(CIProject.LatestBuild.Trigger, ref buildModel);

            model.ShowDuration = settingsViewModel.ShowDuration;
            model.ShowStartTime = settingsViewModel.ShowStartTime;
            model.ShowStatus = settingsViewModel.ShowStatus;
            model.ShowTriggerCause = settingsViewModel.ShowTriggerCause;
            model.ShowTriggeredBy = settingsViewModel.ShowTriggeredBy;
        }

        private void SetBuildTrigger(Trigger trigger, ref BuildViewModel buildViewModel)
        {
            string username = RemoveEmailFromUsernameIfExists(trigger);
            buildViewModel.TriggeredBy = TryGetPerson(username);
            buildViewModel.TriggerCause = trigger.Cause;
        }

        private string RemoveEmailFromUsernameIfExists(Trigger trigger)
        {
            var usr = trigger.InvokedBy;
            if (usr.Contains("<") && usr.Contains("@"))
            {
                // InvokedBy in git is "firstname lastname <email@address.org>"
                usr = usr.Substring(0, usr.IndexOf("<") - 1);
                var firstAndLastName = usr.Split(' ');
                var firstName = firstAndLastName[0];
                var lastName = firstAndLastName[1];
                firstName = firstName.Substring(0, 1).ToUpper() + firstName.Substring(1);
                lastName = lastName.Substring(0, 1).ToUpper() + lastName.Substring(1);
                
                return string.Format("{0} {1}", firstName, lastName );
                
            }
            return usr;
        }

        private Person TryGetPerson(string username)
        {
            Person returnPerson = new UnknownPerson();

            try
            {
                var userList = userRepository.Get(new UserByUsername(username));

                if (userList.Count() > 0)
                {
                    var user = userList.First();
                    returnPerson = new Person()
                                   {
                                       Username = username,
                                       Firstname = user.Firstname,
                                       Middlename = user.Middlename,
                                       Surname = user.Surname,
                                       Email = user.Email,
                                       ImageUrl = user.ImageUrl,
                                   };
                }
                else
                {
                    returnPerson = new Person()
                    {
                        Username = username,
                        ImageUrl = User.unknownUser.ImageUrl
                    };
                }
            }
            catch (Exception)
            {
                returnPerson = new Person()
                {
                    Username = "unknown"
                };

            }

            return returnPerson;
        }

        private class SettingsHandler
        {
            private const string CONFIG_NAME = "CIWidgetSettings";

            private const string THRESHOLD_SETTING_NAME = "InactiveProjectThreshold";
            private const int THRESHOLD_SETTING_DEFAULT = 90;
            private const string FILTER_SETTING_NAME = "FilterInactiveProjects";
            private const bool FILTER_SETTING_DEFAULT = false;

            private const string SHOW_TRIGGEREDBY_SETTING_NAME = "showTriggeredBy";
            private const string SHOW_TRIGGERCAUSE_SETTING_NAME = "showTriggerCause";
            private const string SHOW_STARTTIME_SETTING_NAME = "showStartTime";
            private const string SHOW_DURATION_SETTING_NAME = "showDuration";
            private const string SHOW_STATUS_SETTING_NAME = "showStatus";



            private int initialInactiveProjectThreshold = THRESHOLD_SETTING_DEFAULT;
            private bool initialFilterInactiveProjects = FILTER_SETTING_DEFAULT;
            private bool initialShowDuration = true;
            private bool initialShowStartTime = true;
            private bool initialShowTriggerCause = true;
            private bool initialShowTriggeredBy = true;
            private bool initialShowStatus = true;

            private CISettingsViewModel settingsViewModel;

            public SettingsHandler(CISettingsViewModel viewModel)
            {
                this.settingsViewModel = viewModel;
            }

            public Configuration GetUpdatedConfig()
            {
                var config = new Configuration(CONFIG_NAME);
                settingsViewModel.EachProject(project => config.NewSetting(project.SelectedSetting));
                config.NewSetting(THRESHOLD_SETTING_NAME, settingsViewModel.InactiveProjectThreshold.ToString());
                config.NewSetting(FILTER_SETTING_NAME, settingsViewModel.FilterInactiveProjects.ToString());
                config.NewSetting(SHOW_DURATION_SETTING_NAME, settingsViewModel.ShowDuration.ToString());
                config.NewSetting(SHOW_STARTTIME_SETTING_NAME, settingsViewModel.ShowStartTime.ToString());
                config.NewSetting(SHOW_STATUS_SETTING_NAME, settingsViewModel.ShowStatus.ToString());
                config.NewSetting(SHOW_TRIGGERCAUSE_SETTING_NAME, settingsViewModel.ShowTriggerCause.ToString());
                config.NewSetting(SHOW_TRIGGEREDBY_SETTING_NAME, settingsViewModel.ShowTriggeredBy.ToString());

                config.IsConfigured = true;
                return config;
            }

            public Configuration GetDummyConfig(IEnumerable<CIServer> servers)
            {
                return FillInMissingValues(new Configuration(CONFIG_NAME), servers);
            }

            public void LoadIntoViewModel(Configuration config)
            {
                settingsViewModel.InactiveProjectThreshold = int.Parse(config.GetSetting(THRESHOLD_SETTING_NAME).Value);
                settingsViewModel.FilterInactiveProjects = bool.Parse(config.GetSetting(FILTER_SETTING_NAME).Value);
                settingsViewModel.ShowDuration = bool.Parse(config.GetSetting(SHOW_DURATION_SETTING_NAME).Value);
                settingsViewModel.ShowStartTime = bool.Parse(config.GetSetting(SHOW_STARTTIME_SETTING_NAME).Value);
                settingsViewModel.ShowStatus = bool.Parse(config.GetSetting(SHOW_STATUS_SETTING_NAME).Value);
                settingsViewModel.ShowTriggerCause = bool.Parse(config.GetSetting(SHOW_TRIGGERCAUSE_SETTING_NAME).Value);
                settingsViewModel.ShowTriggeredBy = bool.Parse(config.GetSetting(SHOW_TRIGGEREDBY_SETTING_NAME).Value);
            }

            public Configuration LoadConfigFromDb(IRepository<Configuration> configRepo, IEnumerable<CIServer> servers)
            {
                var spec = new AllSpecification<Configuration>();
                var config = configRepo.Get(spec).Where(c => c.Name == CONFIG_NAME).FirstOrDefault();
                config = config ?? new Configuration(CONFIG_NAME);
                return FillInMissingValues(config, servers);
            }

            private Configuration FillInMissingValues(Configuration config, IEnumerable<CIServer> servers)
            {
                foreach (var server in servers)
                    foreach (var project in server.Projects)
                        AddIfNotExists(config, CISettingsViewModel.GetProjectSelectedSettingName(project), true.ToString());

                AddIfNotExists(config, THRESHOLD_SETTING_NAME, THRESHOLD_SETTING_DEFAULT.ToString());
                AddIfNotExists(config, FILTER_SETTING_NAME, FILTER_SETTING_DEFAULT.ToString());
                AddIfNotExists(config, SHOW_DURATION_SETTING_NAME, "true");
                AddIfNotExists(config, SHOW_STARTTIME_SETTING_NAME, "true");
                AddIfNotExists(config, SHOW_STATUS_SETTING_NAME, "true");
                AddIfNotExists(config, SHOW_TRIGGERCAUSE_SETTING_NAME, "true");
                AddIfNotExists(config, SHOW_TRIGGEREDBY_SETTING_NAME, "true");
                return config;
            }

            private void AddIfNotExists(Configuration config, string name, string value)
            {
                if (!config.ContainsSetting(name))
                    config.NewSetting(name, value);
            }

            public void SetResetPoint()
            {
                initialInactiveProjectThreshold = settingsViewModel.InactiveProjectThreshold;
                initialFilterInactiveProjects = settingsViewModel.FilterInactiveProjects;
                initialShowDuration = settingsViewModel.ShowDuration;
                initialShowStartTime = settingsViewModel.ShowStartTime;
                initialShowTriggerCause = settingsViewModel.ShowTriggerCause;
                initialShowStatus = settingsViewModel.ShowStatus;
                initialShowTriggeredBy = settingsViewModel.ShowTriggeredBy;
                settingsViewModel.EachProject(project => project.SetResetPoint());
            }

            public void Reset()
            {
                settingsViewModel.InactiveProjectThreshold = initialInactiveProjectThreshold;
                settingsViewModel.FilterInactiveProjects = initialFilterInactiveProjects;
                settingsViewModel.EachProject(project => project.ResetSelectedState());

                settingsViewModel.ShowDuration = initialShowDuration;
                settingsViewModel.ShowStartTime = initialShowStartTime;
                settingsViewModel.ShowTriggerCause = initialShowTriggerCause;
                settingsViewModel.ShowTriggeredBy = initialShowTriggeredBy;
                settingsViewModel.ShowStatus = initialShowStatus;

            }
        }
    }

    
    
}