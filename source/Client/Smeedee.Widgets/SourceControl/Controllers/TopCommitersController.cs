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
// The project webpage is located at http://smeedee.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.SourceControl;
using Smeedee.DomainModel.Users;
using Smeedee.Framework;
using Smeedee.Widgets.SourceControl.ViewModels;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widgets.SourceControl.Controllers
{
    public class TopCommitersController : ChangesetStandAloneController<CodeCommiterViewModel>
    {
        private IEnumerable<User> allUsers;
        private readonly IRepository<User> userRepository;
        private readonly IAsyncRepository<Configuration> configRepository;
        private readonly IPersistDomainModelsAsync<Configuration> configPersisterRepository;

        private const string SETTINGS_ENTRY_NAME = "TopCommiters";
        private const string DATE_ENTRY_NAME = "SinceDate";
        private const string TIMESPAN_ENTRY_NAME = "TimeSpanInDays";
        private const string IS_USING_DATE_ENTRY_NAME = "IsUsingDate";
        private const string MAX_NUM_OF_COMMITERS_ENTRY_NAME = "MaxNumOfCommiters";
        private const string ACKNOWLEDGE_OTHERS_ENTRY_NAME = "AcknowledgeOthers";

        private readonly List<string> listOfSettings = new List<string>
            {SETTINGS_ENTRY_NAME, DATE_ENTRY_NAME, TIMESPAN_ENTRY_NAME, IS_USING_DATE_ENTRY_NAME, 
                MAX_NUM_OF_COMMITERS_ENTRY_NAME, ACKNOWLEDGE_OTHERS_ENTRY_NAME};

        private static readonly CultureInfo dateFormatingRules = new CultureInfo("en-US");
        public static readonly Configuration defaultConfig = CreateConfiguration(DateTime.Now.Date, 10, true, 15, true);

        public TopCommitersController(
            BindableViewModel<CodeCommiterViewModel> viewModel, 
            IRepository<Changeset> changesetRepo, 
            IInvokeBackgroundWorker<IEnumerable<Changeset>> backgroundWorker, 
            ITimer timer, IUIInvoker uiInvoke, 
            IAsyncRepository<Configuration> configRepo, 
            IPersistDomainModelsAsync<Configuration> configPersister,
            IRepository<User> userRepo,
            ILog logger,
            IProgressbar loadingNotifier)
            : base(viewModel, changesetRepo, backgroundWorker, timer, uiInvoke, logger, loadingNotifier)
        {
            Guard.Requires<ArgumentException>(userRepo != null, "userRepo");
            Guard.Requires<ArgumentException>(configRepo != null, "configRepo");
            Guard.Requires<ArgumentException>(configPersister != null, "configPersister");
            
            userRepository = userRepo;
            configRepository = configRepo;
            configPersisterRepository = configPersister;

            ViewModel.SaveSettings.AfterExecute += (s, e) =>  SaveAndReload();
            ViewModel.ReloadFromRepository.AfterExecute += (s, e) => LoadConfigAndData();

            ViewModel.PropertyChanged += ViewModelPropertyChanged;
            configPersisterRepository.SaveCompleted += ConfigPersisterRepositorySaveCompleted;
            configRepository.GetCompleted += ConfigRepositoryGetCompleted;

            Start();

            LoadDefaultValuesIntoViewModel();
            LoadConfigAndData();
        }

        private void ConfigPersisterRepositorySaveCompleted(object sender, SaveCompletedEventArgs e)
        {
            SetIsNotSavingConfig();
        }

        private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (listOfSettings.Contains(e.PropertyName))
            {
                configIsChanged = true;
            }
        }

        private void SaveAndReload()
        {
            SaveConfigToRepository();
            ReloadViewModelData();
        }

        private void SaveConfigToRepository()
        {
            SetIsSavingConfig();

            var newConfig = CreateConfiguration(ViewModel.SinceDate, ViewModel.TimeSpanInDays, ViewModel.IsUsingDate,
                                                ViewModel.MaxNumOfCommiters, ViewModel.AcknowledgeOthers);

            configPersisterRepository.Save(newConfig);
        }

        protected override void OnNotifiedToRefresh(object sender, EventArgs e)
        {
            LoadConfigAndData();
        }

        private void UpdateViewModel()
        {
            if (configIsChanged)
            {
                ReloadViewModelData();
            }
            else
            {
                LoadData(new ChangesetsAfterRevisionSpecification(ViewModel.CurrentRevision));
            }
        }

        private void ReloadViewModelData()
        {
            uiInvoker.Invoke(() =>
            {
                ViewModel.Data.Clear();
                ViewModel.CurrentRevision = 0;
            });
            
            LoadData();
        }

        private void LoadDefaultValuesIntoViewModel()
        {
            LoadSettings(defaultConfig);
        }

        private void LoadConfigAndData()
        {
            try
            {
                SetIsLoadingConfig();
                configRepository.BeginGet(new ConfigurationByName(SETTINGS_ENTRY_NAME));
            }
            catch (Exception exception)
            {
                LogErrorMsg(exception);
                SetIsNotLoadingConfig();
            }
        }

        private void ConfigRepositoryGetCompleted(object sender, GetCompletedEventArgs<Configuration> eventArgs)
        {
            var config = eventArgs.Result.FirstOrDefault();
            SetIsNotLoadingConfig();
            SetSettingsOnViewModel(config);

            UpdateViewModel();
        }

        private void SetSettingsOnViewModel(Configuration config)
        {
            if (ConfigurationContainsSettings(config))
            {
                LoadSettings(config);
            }
            else
            {
                configPersisterRepository.Save(defaultConfig);
                SetIsNotLoadingConfig();
            }
            
        }

        private static bool ConfigurationContainsSettings(Configuration config)
        {
            return config != null && config.Settings.Count() != 0;
        }
       
        private void LoadSettings(Configuration config)
        {
            uiInvoker.Invoke(() =>
            {
                TryToLoadSpecificSetting(config.GetSetting(DATE_ENTRY_NAME));
                TryToLoadSpecificSetting(config.GetSetting(TIMESPAN_ENTRY_NAME));
                TryToLoadSpecificSetting(config.GetSetting(IS_USING_DATE_ENTRY_NAME));
                TryToLoadSpecificSetting(config.GetSetting(MAX_NUM_OF_COMMITERS_ENTRY_NAME));
                TryToLoadSpecificSetting(config.GetSetting(ACKNOWLEDGE_OTHERS_ENTRY_NAME));

                SetIsNotLoadingConfig();
            });
           
        }

        private void TryToLoadSpecificSetting(SettingsEntry setting)
        {
            try
            {
                LoadSpecificSetting(setting);
            }
            catch (Exception exception)
            {
                LogErrorMsg(exception);
            }
        }

        private void LoadSpecificSetting(SettingsEntry setting)
        {
            switch (setting.Name)
            {
                case DATE_ENTRY_NAME:
                    ViewModel.SinceDate = DateTime.Parse(setting.Value.Trim(), dateFormatingRules).Date;
                    break;
                case TIMESPAN_ENTRY_NAME:
                    ViewModel.TimeSpanInDays = Int32.Parse(setting.Value.Trim());
                    break;
                case IS_USING_DATE_ENTRY_NAME:
                    ViewModel.IsUsingDate = Boolean.Parse(setting.Value.Trim());
                    break;
                case MAX_NUM_OF_COMMITERS_ENTRY_NAME:
                    ViewModel.MaxNumOfCommiters = Int32.Parse(setting.Value.Trim());
                    break;
                case ACKNOWLEDGE_OTHERS_ENTRY_NAME:
                    ViewModel.AcknowledgeOthers = Boolean.Parse(setting.Value.Trim());
                    break;
            }
        }

        private static Configuration CreateConfiguration(DateTime date, int timespan, bool isUsingDate, int maxNumOfCommiters, bool acknowledgeOthers)
        {
            var newConfig = new Configuration(SETTINGS_ENTRY_NAME){IsConfigured = true};

            newConfig.NewSetting(DATE_ENTRY_NAME, date.ToString(dateFormatingRules));
            newConfig.NewSetting(TIMESPAN_ENTRY_NAME, timespan.ToString());
            newConfig.NewSetting(IS_USING_DATE_ENTRY_NAME, isUsingDate.ToString());
            newConfig.NewSetting(MAX_NUM_OF_COMMITERS_ENTRY_NAME, maxNumOfCommiters.ToString());
            newConfig.NewSetting(ACKNOWLEDGE_OTHERS_ENTRY_NAME, acknowledgeOthers.ToString());

            return newConfig;
        }

        protected override void LoadDataIntoViewModel(IEnumerable<Changeset> qChangesets)
        {
            qChangesets = FilterChangesets(qChangesets);

            UpdateRevision(qChangesets);
            if (!ViewModel.RevisionIsChanged && !configIsChanged) return;

            UpdateCommitersInViewModel(qChangesets);

            UpdateNumberOfCommitsShown();

            configIsChanged = false;
        }

        private static IEnumerable<Changeset> FilterChangesets(IEnumerable<Changeset> qChangesets)
        {
            return qChangesets.Where(c => c != null && c.Author != null && c.Author.Username != null);
        }

        private void UpdateCommitersInViewModel(IEnumerable<Changeset> qChangesets)
        {
            if (qChangesets == null || qChangesets.Count() == 0) return;

            var committers = GetFilteredCommitters(qChangesets);

            committers = GetMergedCommiters(committers);

            committers = OrderByDescending(committers);

            committers = RestrictCommitersToMax(committers);

            if (HasUpdatedChangesets(committers))
            {
                ReloadCommitters(committers);
            }
            else
            {
                UpdateNumberOfCommits(committers);
            }
        }

        private IEnumerable<CodeCommiterViewModel> GetFilteredCommitters(IEnumerable<Changeset> qChangesets)
        {
            return (from changeset in qChangesets
                    where (changeset.Time >= ViewModel.ActualDateUsed.Date)
                    group changeset by changeset.Author.Username
                        into g
                        select new CodeCommiterViewModel
                        {
                            Username = g.Key,
                            Firstname = g.Key,
                            NumberOfCommits = g.Count()
                        });
        }

        private IEnumerable<CodeCommiterViewModel> GetMergedCommiters(IEnumerable<CodeCommiterViewModel> committers)
        {
            var mergedCommiters = UpdateOldCommiters(committers);

            AddNewCommiters(committers, mergedCommiters);

            return mergedCommiters;
        }

        private ObservableCollection<CodeCommiterViewModel> UpdateOldCommiters(IEnumerable<CodeCommiterViewModel> committers)
        {
            var mergedCommiters = new ObservableCollection<CodeCommiterViewModel>();
            foreach (var oldCommiter in ViewModel.Data)
            {
                foreach (var newCommiter in committers)
                {
                    if (oldCommiter.Username.Equals(newCommiter.Username))
                    {
                        oldCommiter.NumberOfCommits += newCommiter.NumberOfCommits;
                    }
                }
                mergedCommiters.Add(oldCommiter);
            }
            return mergedCommiters;
        }

        private static void AddNewCommiters(IEnumerable<CodeCommiterViewModel> committers, ICollection<CodeCommiterViewModel> mergedCommiters)
        {
            foreach (var newCommiter in committers)
            {
                var exists = false;
                foreach (var mergedCommiter in mergedCommiters)
                {
                    if (mergedCommiter.Username.Equals(newCommiter.Username))
                    {
                        exists = true;
                    }
                }

                if (!exists)
                {
                    mergedCommiters.Add(newCommiter);
                }
            }
        }

        private static IEnumerable<CodeCommiterViewModel> OrderByDescending(IEnumerable<CodeCommiterViewModel> committers)
        {
            return committers.OrderByDescending(c => c.NumberOfCommits);
        }

        private IEnumerable<CodeCommiterViewModel> RestrictCommitersToMax(IEnumerable<CodeCommiterViewModel> commiters)
        {
            var restrictedCommiters = new List<CodeCommiterViewModel>();
            var sumOfOtherCommiters = new CodeCommiterViewModel { Firstname = "Others", Username = "Others"};

            var i = 0;

            foreach (var codeCommiter in commiters)
            {
                if (i < ViewModel.MaxNumOfCommiters)
                {
                    restrictedCommiters.Add(codeCommiter);
                }
                else
                {
                    sumOfOtherCommiters.NumberOfCommits += codeCommiter.NumberOfCommits;
                }
                i++;
            }

            if(ViewModel.AcknowledgeOthers && sumOfOtherCommiters.NumberOfCommits != 0)
                {
                    restrictedCommiters.Add(sumOfOtherCommiters);
                }

            return restrictedCommiters;
        }

        protected bool HasUpdatedChangesets(IEnumerable<CodeCommiterViewModel> committers)
        {
            if (committers.Count() != ViewModel.Data.Count)
                return true;

            var committersArray = committers.ToArray();

            var atLestOneUsernameDiffers = ViewModel.Data.Where((t, i) => !t.Username.Equals(committersArray[i].Username)).Any();

            return atLestOneUsernameDiffers;
        }

        private void ReloadCommitters(IEnumerable<CodeCommiterViewModel> committers) 
        {
            ViewModel.Data.Clear();

            foreach (var committer in committers)
            {
                var userInfo = allUsers.Where(u => u.Username.Equals(committer.Username)).SingleOrDefault();

                if (userInfo != null)
                {
                    committer.Firstname = userInfo.Firstname;
                    committer.Middlename = userInfo.Middlename;
                    committer.Surname = userInfo.Surname;
                    committer.Email = userInfo.Email;
                    committer.ImageUrl = userInfo.ImageUrl;
                }

                ViewModel.Data.Add(committer);
            }
        }

        protected void UpdateNumberOfCommits(IEnumerable<CodeCommiterViewModel> committers)
        {
            var committersArray = committers.ToArray();

            for (var i = 0; i < ViewModel.Data.Count; i++)
                ViewModel.Data[i].NumberOfCommits = committersArray[i].NumberOfCommits;
        }

        private void UpdateNumberOfCommitsShown()
        {
            ViewModel.NumberOfCommitsShown = ViewModel.Data.Sum(commiter => commiter.NumberOfCommits);
        }

        protected override void AfterQueryAllChangesets()
        {
            allUsers = userRepository.Get(new AllSpecification<User>());
        }

        public void ConfigurationUpdated(object sender, EventArgs eventArgs)
        {
            configIsChanged = true;
            LoadConfigAndData();
        }
    }
}