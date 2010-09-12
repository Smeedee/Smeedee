using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Smeedee.Client.Framework.Services;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.ProjectInfo;
using Smeedee.DomainModel.Users;
using Smeedee.Framework;
using Smeedee.Widget.TeamMembers.ViewModels;
using TinyMVVM.Framework.Services;
using System.Linq;

namespace Smeedee.Widget.TeamMembers.Controllers
{
    public class TeamMembersController
    {
        public static readonly Configuration DefaultConfig = CreateConfiguration(true, true, false, false, "default");        
        private const String SETTINGS_ENTRY_NAME = "TeamMembers";
        private const String FIRSTNAME_ENTRY_NAME = "firstnameIsChecked";
        private const String SURNAME_ENTRY_NAME = "surnameIsChecked";
        private const String MIDDLENAME_ENTRY_NAME = "middlenameIsChecked";
        private const String USERNAME_ENTRY_NAME = "usernameIsChecked";
        protected const String SELECTED_USERDB_ENTRY_NAME = "selectedUserdb";
        private const int REFRESH_INTERVAL = 25 * 1000;
        private const string LOADING_DATA_MESSAGE = "Loading data from server...";
        private const string SAVING_CONFIG_MESSAGE = "Saving...";
        
        private Configuration currentConfiguration;
        private readonly TeamMembersViewModel teamMembersViewModel;
        private readonly TeamMembersSettingsViewModel teamMembersSettingsViewModel;
        private readonly IRepository<Userdb> userdbRepository;
        private readonly IRepository<Configuration> configRepository;
        private readonly IPersistDomainModelsAsync<Configuration> configPersisterRepository;
        private readonly IInvokeBackgroundWorker<IEnumerable<ProjectInfoServer>> asyncClient;
        private readonly ILog logger;
        private readonly ITimer timer;
        private readonly IUIInvoker uiInvoker;
        private readonly IProgressbar loadingNotifier;
        private readonly IMetadataService metadataService;

        public TeamMembersController(
            TeamMembersViewModel teamMembersViewModel,
            TeamMembersSettingsViewModel teamMembersSettingsViewModel,
            IRepository<Userdb> userdbRepository,
            IRepository<Configuration> configRepo,
            IPersistDomainModelsAsync<Configuration> configPersister,
            IInvokeBackgroundWorker<IEnumerable<ProjectInfoServer>> asyncClient, 
            ILog logger,
            ITimer timer,
            IUIInvoker uiInvoker,
            IProgressbar loadingNotifier, IMetadataService metadataService)
            
        {
            Guard.ThrowExceptionIfNull(userdbRepository, "userdbRepository");
            Guard.ThrowExceptionIfNull(configRepo, "configRepository");
            Guard.ThrowExceptionIfNull(configPersister, "configPersisterRepository");
            Guard.ThrowExceptionIfNull(asyncClient, "asyncClient");
            Guard.ThrowExceptionIfNull(logger, "logger");

            this.teamMembersViewModel = teamMembersViewModel;
            this.teamMembersSettingsViewModel = teamMembersSettingsViewModel;
            this.userdbRepository = userdbRepository;
            this.metadataService = metadataService;
            this.configRepository = configRepo;
            this.configPersisterRepository = configPersister;
            this.asyncClient = asyncClient;
            this.logger = logger;
            this.timer = timer;
            this.uiInvoker = uiInvoker;
            this.loadingNotifier = loadingNotifier;

            timer.Start(REFRESH_INTERVAL);
            timer.Elapsed += OnNotifiedToRefresh;
            teamMembersSettingsViewModel.Save.AfterExecute += (s, e) => OnSave();
            teamMembersSettingsViewModel.Refresh.AfterExecute += (s, e) => OnRefresh();
            teamMembersSettingsViewModel.PropertyChanged += PropertyChanged;
            configPersister.SaveCompleted += ConfigPersisterSaveCompleted;

            
            LoadConfig();
            LoadData();
        }

        private void OnSave()
        {
            UpdateVisibilityOnViewModel();
            SetIsSavingConfig();
            SaveSettings();
        }

        private void UpdateVisibilityOnViewModel()
        {
            uiInvoker.Invoke( ()=>
                {
                    teamMembersViewModel.UsernameIsChecked = teamMembersSettingsViewModel.UsernameIsChecked;
                    teamMembersViewModel.SurnameIsChecked = teamMembersSettingsViewModel.SurnameIsChecked;
                    teamMembersViewModel.FirstnameIsChecked = teamMembersSettingsViewModel.FirstnameIsChecked;
                    teamMembersViewModel.MiddlenameIsChecked = teamMembersSettingsViewModel.MiddlenameIsChecked;
                }
            );
        }
        
        private void SetIsSavingConfig()
        {
            uiInvoker.Invoke(()=> teamMembersSettingsViewModel.IsSaving = true );
            loadingNotifier.ShowInSettingsView(SAVING_CONFIG_MESSAGE);
        }
        
        private void SaveSettings()
        {
            var newConfig = CreateConfiguration(teamMembersSettingsViewModel.FirstnameIsChecked, teamMembersSettingsViewModel.SurnameIsChecked,
                                                teamMembersSettingsViewModel.MiddlenameIsChecked, teamMembersSettingsViewModel.UsernameIsChecked, 
                                                teamMembersSettingsViewModel.CurrentDBName);

            configPersisterRepository.Save(newConfig);
        }

        private static Configuration CreateConfiguration(bool firstName, bool surname, bool middlename, bool username, string userdbName)
        {
            var newConfig = new Configuration(SETTINGS_ENTRY_NAME);

            newConfig.NewSetting(FIRSTNAME_ENTRY_NAME, firstName.ToString());
            newConfig.NewSetting(SURNAME_ENTRY_NAME, surname.ToString());
            newConfig.NewSetting(MIDDLENAME_ENTRY_NAME, middlename.ToString());
            newConfig.NewSetting(USERNAME_ENTRY_NAME, username.ToString());
            newConfig.NewSetting(SELECTED_USERDB_ENTRY_NAME, userdbName);

            return newConfig;
        }

        private void OnRefresh()
        {
            LoadConfig();
            LoadData();
        }

        private void LoadConfig()
        {
            SetIsLoadingConfig();
            asyncClient.RunAsyncVoid(() =>
            {
                try
                {
                    var allConfigSpec = new ConfigurationByName(SETTINGS_ENTRY_NAME);
                    currentConfiguration = configRepository
                        .Get(allConfigSpec)
                        .SingleOrDefault();

                    LoadSettings(currentConfiguration);
                }
                catch (Exception exception)
                {
                    LogError(exception);
                }
            });
            SetIsNotLoadingConfig();
        }

        private void SetIsNotLoadingConfig()
        {
            uiInvoker.Invoke(()=> teamMembersSettingsViewModel.IsLoading = false );
            loadingNotifier.HideInSettingsView();
        }

        private void SetIsLoadingConfig()
        {
            uiInvoker.Invoke(()=> teamMembersSettingsViewModel.IsLoading = true );
            loadingNotifier.ShowInSettingsView(LOADING_DATA_MESSAGE);
        }

        private void LoadSettings(Configuration config)
        {
            if (config != null && config.Settings.Count() != 0)
            {
                TryLoadSpecificSetting(config.GetSetting(FIRSTNAME_ENTRY_NAME));
                TryLoadSpecificSetting(config.GetSetting(SURNAME_ENTRY_NAME));
                TryLoadSpecificSetting(config.GetSetting(MIDDLENAME_ENTRY_NAME));
                TryLoadSpecificSetting(config.GetSetting(USERNAME_ENTRY_NAME));
            }
            else
            {
                configPersisterRepository.Save(DefaultConfig);
                LoadSettings(DefaultConfig);
            }
        }

        private bool ConfiguredCurrentUserdbExists()
        {
            try
            {
                var settingsEntry = currentConfiguration.GetSetting(SELECTED_USERDB_ENTRY_NAME);
                var userdbName = settingsEntry.Value.Trim();
                var viewModelHasUserdb = teamMembersSettingsViewModel.Userdbs.Contains(userdbName);
                return viewModelHasUserdb;
            
            }
            catch (Exception exception)
            {
                LogError(exception);
            }

            return false;
        }

        private void TryLoadSpecificSetting(SettingsEntry setting)
        {
            if (setting != null)
            {
                try
                {
                    SetSpecificSettingOnViewModel(setting);
                }
                catch (Exception exception)
                {
                    LogError(exception);
                }
            }
        }

        private void SetSpecificSettingOnViewModel(SettingsEntry setting)
        {
            var settingsValue = Boolean.Parse(setting.Value.Trim());

            uiInvoker.Invoke(() =>
            {
                switch (setting.Name)
                {
                    case FIRSTNAME_ENTRY_NAME:
                        teamMembersSettingsViewModel.FirstnameIsChecked = settingsValue;
                        break;
                    case SURNAME_ENTRY_NAME:
                        teamMembersSettingsViewModel.SurnameIsChecked = settingsValue;
                        break;
                    case MIDDLENAME_ENTRY_NAME:
                        teamMembersSettingsViewModel.MiddlenameIsChecked = settingsValue;
                        break;
                    case USERNAME_ENTRY_NAME:
                        teamMembersSettingsViewModel.UsernameIsChecked = settingsValue;
                        break;
                }                    
            });
            UpdateVisibilityOnViewModel();
        }

        private void PropertyChanged(object sender, PropertyChangedEventArgs propertyEvent)
        {
            if (propertyEvent.PropertyName.Equals("CurrentDBName"))
            {
                LoadData();
            }
        }

        protected void OnNotifiedToRefresh(object sender, EventArgs e)
        {
            LoadConfig();         
            LoadData();
        }


        private void LoadData()
        {
            asyncClient.RunAsyncVoid(() =>
            {
                if (!teamMembersViewModel.IsLoading)
                {
                    SetIsLoadingData();

                    var allUserdbs = GetAllUserdbs();

                    if (teamMembersViewModel.HasDatabase)
                    {
                        uiInvoker.Invoke(() =>
                        {
                            try
                            {
                                UpdateViewModel(allUserdbs);
                                SetDeploymentPathOnViewModel(); 
                                CheckForGeneratedUserdb(allUserdbs);
                            }
                            catch (Exception e)
                            {
                                LogError(e);
                            }
                        });
                    }

                    SetIsNotLoadingData();
                }
            });
        }

        private Userdb GetCurrentUserdb(IEnumerable<Userdb> allUserdbs)
        {
            return allUserdbs.Where(db => db.Name.Equals(teamMembersSettingsViewModel.CurrentDBName)).SingleOrDefault();
        }

        private void SetIsNotLoadingData()
        {
            uiInvoker.Invoke(()=> teamMembersViewModel.IsLoading = false); 
            loadingNotifier.HideInView();
        }

        private void SetIsLoadingData()
        {
            uiInvoker.Invoke(()=> teamMembersViewModel.IsLoading = true);
            loadingNotifier.ShowInView(LOADING_DATA_MESSAGE);
        }

        private void SetDeploymentPathOnViewModel()
        {
            teamMembersViewModel.DeploymentPath = metadataService.GetDeploymentUrl().AbsolutePath;
        }
        
        private void ConfigPersisterSaveCompleted(object sender, SaveCompletedEventArgs e)
        {
            SetIsNotSavingConfig();
        }
        
        private void SetIsNotSavingConfig()
        {
            uiInvoker.Invoke(()=> teamMembersSettingsViewModel.IsSaving = false );
            loadingNotifier.HideInSettingsView();
        }
         

        private IEnumerable<Userdb> GetAllUserdbs()
        {
            IEnumerable<Userdb> allUserdbs = null;

            try
            {
                allUserdbs = userdbRepository.Get(new AllSpecification<Userdb>());
                teamMembersViewModel.HasConnectionProblems = false;
            }
            catch (Exception exception)
            {
                teamMembersViewModel.HasConnectionProblems = true;
                LogError(exception);
            }

            uiInvoker.Invoke(() => teamMembersViewModel.HasDatabase = allUserdbs != null && allUserdbs.Count() != 0);

            return allUserdbs;
        }

        private void LogError(Exception exception)
        {
            logger.WriteEntry(new ErrorLogEntry
            {
                Message = exception.ToString(),
                Source = GetType().ToString(),
                TimeStamp = DateTime.Now
            });
        }

        private void UpdateViewModel(IEnumerable<Userdb> allUserDBs)
        {
            UpdateUserdbNames(allUserDBs);
            UpdateUsers(allUserDBs);
        }

        private void CheckForGeneratedUserdb(IEnumerable<Userdb> allUserdbs)
        {
            var currentUserdb = GetCurrentUserdb(allUserdbs);

            if(currentUserdb != null)
            {
                var usersInCurrentDbOnlyHaveUsername = UsersInUserdbOnlyHaveUsername(currentUserdb);

                if (usersInCurrentDbOnlyHaveUsername)
                {
                    var usernameOnlyConfig = CreateConfiguration(false, false, false, true , teamMembersSettingsViewModel.CurrentDBName);
                    LoadSettings(usernameOnlyConfig);

                    uiInvoker.Invoke(() => teamMembersSettingsViewModel.InformationField = "Users in this userdb only have usernames!");
                }
                else
                {
                    LoadSettings(currentConfiguration);
                    uiInvoker.Invoke(() => teamMembersSettingsViewModel.InformationField = "" );
                }  
            }
            
        }

        private static bool UsersInUserdbOnlyHaveUsername(Userdb currentDb)
        {
            var usersOnlyHaveUsername = true;

            foreach (var user in currentDb.Users)
            {
                if (user.Firstname != null && !user.Firstname.Equals(""))
                {
                    usersOnlyHaveUsername = false;
                }
                if (user.Surname != null && !user.Surname.Equals(""))
                {
                    usersOnlyHaveUsername = false;
                }
            }
            return usersOnlyHaveUsername;
        }

        private void UpdateUserdbNames(IEnumerable<Userdb> allUserdbs)
        {
            var selectedUserdb = teamMembersSettingsViewModel.CurrentDBName;
            var userdbNames = allUserdbs.Select(userdb => userdb.Name).ToList();

            UpdateList(teamMembersSettingsViewModel.Userdbs, userdbNames);

            if (selectedUserdb != null && userdbNames.Contains(selectedUserdb))
            {
                teamMembersSettingsViewModel.CurrentDBName = selectedUserdb;
            }
            else if(ConfiguredCurrentUserdbExists())
            {
                teamMembersSettingsViewModel.CurrentDBName = GetConfiguredCurrentUserdb();
            }
            else 
            {
                teamMembersSettingsViewModel.CurrentDBName = allUserdbs.First().Name;
            }

            //teamMembersSettingsViewModel.CurrentDBName = selectedUserdb ?? allUserdbs.First().Name;
        }

        private string GetConfiguredCurrentUserdb()
        {
            string configuredCurrentUserdbName = null;
            try
            {
                configuredCurrentUserdbName =
                    currentConfiguration.GetSetting(SELECTED_USERDB_ENTRY_NAME).Value.Trim();
            }
            catch (Exception) {}

            return configuredCurrentUserdbName;
        }

        private static void UpdateList(ICollection<string> oldList, ICollection<string> newList)
        {
            var oldListCopy = new List<string>(oldList);
            var newListCopy = new List<string>(newList);

            foreach (var oldElement in oldListCopy)
            {
                if (!newListCopy.Contains(oldElement)) { oldList.Remove(oldElement); }
            }

            foreach (var newElement in newListCopy)
            {
                if (!oldListCopy.Contains(newElement)) { oldList.Add(newElement); }
            }
        }

        private void UpdateUsers(IEnumerable<Userdb> allUserDBs)
        {
            var currentDb = allUserDBs.Where(db => db.Name.Equals(teamMembersSettingsViewModel.CurrentDBName)).SingleOrDefault();

            teamMembersViewModel.TeamMembers = new ObservableCollection<UserViewModel>();

            foreach (var tempUserViewModel in currentDb.Users.Select(user => CreateUserViewModelFromUserDomainModel(user)))
            {
                teamMembersViewModel.AddTeamMember(tempUserViewModel);
            }
        }

        public static User CreateUserDomainModelFromUserViewModel(UserViewModel model)
        {
            var user = new User(model.Username)
            {
                Firstname = model.Firstname,
                Middlename = model.Middlename,
                Surname = model.Surname,
                Email = model.Email,
                ImageUrl = model.ImageUrl
            };

            return user;
        }

        public static UserViewModel CreateUserViewModelFromUserDomainModel(User user)
        {
            var model = new UserViewModel
            {
                Username = user.Username,
                Firstname = user.Firstname,
                Middlename = user.Middlename,
                Surname = user.Surname,
                Email = user.Email,
                ImageUrl = user.ImageUrl
            };
            return model;
        }

        public void StopRefreshTimer()
        {
            timer.Stop();
        }

        public void StartRefreshTimer()
        {
            timer.Start(REFRESH_INTERVAL);
        }
    }
}
