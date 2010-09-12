using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Smeedee.Client.Framework.Services;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.Users;
using Smeedee.Framework;
using Smeedee.Widget.Admin.Users.ViewModels;
using TinyMVVM.Framework.Services;


namespace Smeedee.Widget.Admin.Users.Controllers
{
    public class UsersController 
    {
        protected const string SAVING_DATA_MESSAGE = "Saving data...";
        protected const string LOADING_DATA_MESSAGE = "Loading data from server...";
        private const string DEFAULT_USERDB_NAME = "default";

        private readonly IRepository<Userdb> userdbRepository;
        private readonly IPersistDomainModelsAsync<Userdb> userdbPersisterRepository;
        private readonly IInvokeBackgroundWorker<IEnumerable<Userdb>> asyncClient;
        private readonly ILog logger;
        private readonly IMetadataService metadataService;
        private readonly IUIInvoker uiInvoker;
        private readonly IProgressbar loadingNotifier;
        private UsersViewModel viewModel;

        public UsersController(
            UsersViewModel viewModel,
            IRepository<Userdb> userdbRepository,
            IPersistDomainModelsAsync<Userdb> userdbPersisterRepository,
            IInvokeBackgroundWorker<IEnumerable<Userdb>> asyncClient,
            ILog logger,
            IUIInvoker uiInvoker,
            IProgressbar loadingNotifier, 
            IMetadataService metadataService)
        {
            Guard.ThrowExceptionIfNull(userdbRepository, "userdbRepository");
            Guard.ThrowExceptionIfNull(userdbPersisterRepository, "userdbPersisterRepository");
            Guard.ThrowExceptionIfNull(asyncClient, "asyncClient");
            Guard.ThrowExceptionIfNull(logger, "logger");

            this.userdbRepository = userdbRepository;
            this.metadataService = metadataService;
            this.userdbPersisterRepository = userdbPersisterRepository;
            this.asyncClient = asyncClient;
            this.logger = logger;
            this.uiInvoker = uiInvoker;
            this.loadingNotifier = loadingNotifier;
            this.viewModel = viewModel;

            this.viewModel.Save.AfterExecute += (sender, args) => SaveDataToRepository();
            this.viewModel.Refresh.AfterExecute += (sender, args) => LoadData();
            this.viewModel.PropertyChanged += SelectedDbChanged;
            this.userdbPersisterRepository.SaveCompleted += OnSaveCompleted;

            LoadData();
        }

        private void SaveDataToRepository()
        {
            SetIsSavingData();
            SaveCurrentUserdb();
        }

        private void SelectedDbChanged(object sender, PropertyChangedEventArgs propertyEvent)
        {
            if (propertyEvent.PropertyName.Equals("CurrentDBName"))
            {
                LoadData();
            }
        }

        private void OnSaveCompleted(object sender, SaveCompletedEventArgs e)
        {
            SetIsNotSavingData();
        }

        private void SaveCurrentUserdb()
        {
            var userdb = new Userdb(viewModel.CurrentDBName);

            foreach (var userViewModel in viewModel.Users)
            {
                userdb.AddUser(CreateUserDomainModelFromUserViewModel(userViewModel));
            }

            userdbPersisterRepository.Save(userdb);
        }

        private void LoadData()
        {
            asyncClient.RunAsyncVoid(() =>
            {
                if (!viewModel.IsLoading)
                {
                    SetIsLoadingData();
                    var allUserdbs = GetAllUserdbs();

                    try
                    {
                        if (!viewModel.HasDatabase)
                        {
                            /*Note: 
                             * Temp fix to deal with the process of getting widget type in
                             *  MEF which results in at least two widgets, 
                             *  where one widget will not get Userdbs and subsequently create a default userdb overwriting 
                             *  any other userdbs named default. Todo: Remove quote when MEF metadata loading is fixed
                             */
                            //CreateDefaultUserdb();

                            SetDefaultDbOnViewModel();
                        }
                        else
                            UpdateViewModel(allUserdbs);
                    }
                    catch (Exception e)
                    {
                        LogError(e);
                    }
                }

                SetIsNotLoadingData();
            });
        }

        private void CreateDefaultUserdb()
        {
            var newUserDb = new Userdb(DEFAULT_USERDB_NAME);
            SetIsSavingData();
            userdbPersisterRepository.Save(newUserDb);
        }

        private void SetDefaultDbOnViewModel()
        {
            uiInvoker.Invoke(() =>
            {
                viewModel.Userdbs.Clear();
                viewModel.Userdbs.Add(DEFAULT_USERDB_NAME);
                viewModel.CurrentDBName = DEFAULT_USERDB_NAME;
                viewModel.HasDatabase = true;
            });
        }

        private void UpdateViewModel(IEnumerable<Userdb> allUserdbs)
        {
            uiInvoker.Invoke(() =>
            {
                UpdateUserdbNames(allUserdbs);
                UpdateUsers(allUserdbs);
                SetDeploymentPathOnViewModel();
            });
        }

        private void SetIsNotLoadingData()
        {
            uiInvoker.Invoke(() => viewModel.IsLoading = false );
            loadingNotifier.HideInSettingsView();
        }

        private void SetIsLoadingData()
        {
            uiInvoker.Invoke(() =>  viewModel.IsLoading = true );
            loadingNotifier.ShowInSettingsView(LOADING_DATA_MESSAGE);
        }

        protected void SetIsNotSavingData()
        {
            uiInvoker.Invoke(() => viewModel.IsSaving = false );
            loadingNotifier.HideInSettingsView();
        }

        private void SetIsSavingData()
        {
            uiInvoker.Invoke(() => viewModel.IsSaving = true );
            loadingNotifier.ShowInSettingsView(SAVING_DATA_MESSAGE);
        }

        private IEnumerable<Userdb> GetAllUserdbs()
        {
            IEnumerable<Userdb> allUserdbs = null;

            try
            {
                allUserdbs = userdbRepository.Get(new AllSpecification<Userdb>());
                viewModel.HasConnectionProblems = false;
            }
            catch (Exception exception)
            {
                viewModel.HasConnectionProblems = true;
                LogError(exception);
            }

           uiInvoker.Invoke(()=> viewModel.HasDatabase = allUserdbs != null && allUserdbs.Count() != 0);

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

        private void UpdateUserdbNames(IEnumerable<Userdb> allUserdbs)
        {

            var selectedUserdb = viewModel.CurrentDBName;
            var userdbNames = allUserdbs.Select(userdb => userdb.Name).ToList();

            UpdateList(viewModel.Userdbs, userdbNames);
            viewModel.CurrentDBName = selectedUserdb ?? allUserdbs.First().Name;
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

        private void UpdateUsers(IEnumerable<Userdb> allUserdbs)
        {
            var currentUserdb = allUserdbs.Where(db => db.Name.Equals(viewModel.CurrentDBName)).SingleOrDefault();

            if (currentUserdb != null)
            {
                viewModel.Users.Clear();

                foreach (var user in currentUserdb.Users)
                {
                    viewModel.Users.Add(CreateUserViewModelFromUserDomainModel(user));
                }
            }
        }

        private void SetDeploymentPathOnViewModel()
        {
            viewModel.DeploymentPath = metadataService.GetDeploymentUrl().AbsolutePath;
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
    }

}
