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
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Smeedee.Client.Framework.Controller;
using Smeedee.Client.Framework.Services;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.SourceControl;
using Smeedee.DomainModel.Users;
using Smeedee.Widget.SourceControl.ViewModels;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widget.SourceControl.Controllers
{
    public class LatestCommitsController : ControllerBase<LatestCommitsViewModel>
    {

        private const string BlinkIsCheckedEntryName = "blinkOnBlankComment";
        private const string NumberOfCommittsEntryName = "numberOfCommits";
        private const string SettingsEntryName = "CheckInNotification";
        private const string KeywordsEntryName = "commentKeywords";

        private readonly List<string> listOfSettings = new List<string>
        {
            BlinkIsCheckedEntryName, NumberOfCommittsEntryName
        };

        private long lastRevision = 1;
        private IRepository<Changeset> repository;
        private IRepository<User> userRepository;
        private readonly IRepository<Configuration> configRepo;
        private readonly IPersistDomainModelsAsync<Configuration> configPersister;
        private IInvokeBackgroundWorker<IEnumerable<Changeset>> asyncClient;
        private ILog logger;

        public LatestCommitsController(
            LatestCommitsViewModel viewModel, 
            IUIInvoker uiInvoke, 
            ITimer timer, 
            IRepository<Changeset> changesetRepo, 
            IRepository<User> userRepository, 
            IRepository<Configuration> configRepo, 
            IPersistDomainModelsAsync<Configuration> configPersister, 
            IInvokeBackgroundWorker<IEnumerable<Changeset>> asyncClient, 
            ILog logger, 
            IProgressbar loadingNotifier)
            : base(viewModel, timer, uiInvoke, loadingNotifier)
        {
            this.repository = changesetRepo;
            this.userRepository = userRepository;
            this.configRepo = configRepo;
            this.configPersister = configPersister;
            this.asyncClient = asyncClient;
            this.logger = logger;

            ViewModel.SaveSettings.ExecuteDelegate = SaveSettings;
            ViewModel.AddWordAndColorSettings.ExecuteDelegate = AddWordAndColorSettings;

            configPersister.SaveCompleted += ConfigPersisterRepositorySaveCompleted;

            Start();
            LoadSettingsAndData();
        }

        protected override void OnNotifiedToRefresh(object sender, EventArgs e)
        {
            LoadSettingsAndData();
        }

        private void  SaveSettings()
        {
            SetIsSavingConfig();

            ViewModel.SetResetPoint();

            var configToSave = new Configuration(SettingsEntryName);
            configToSave.NewSetting(NumberOfCommittsEntryName, ViewModel.NumberOfCommits.ToString());
            configToSave.NewSetting(BlinkIsCheckedEntryName, ViewModel.BlinkWhenNoComment.ToString());

            var keywords = new List<string>();
            foreach (var keywordColor in ViewModel.KeywordList)
            {
                if (keywordColor.Keyword == "")
                    continue;
                keywords.Add(keywordColor.Keyword);
                keywords.Add(keywordColor.ColorName);
            }
            configToSave.NewSetting(KeywordsEntryName, keywords.ToArray());
           
            configToSave.IsConfigured = true;

            configPersister.Save(configToSave);
            ReloadViewModel();
        }

        private void AddWordAndColorSettings()
        {
            var pair = new KeywordColorPairViewModel {Keyword = "word" + ViewModel.KeywordList.Count()};
            pair.KeywordChanged = KeywordChangedHandler;
            ViewModel.KeywordList.Add(pair);
        }

        public void KeywordChangedHandler(KeywordColorPairViewModel sender)
        {
            if (sender.Keyword == "")
                ViewModel.KeywordList.Remove(sender);
        }

        private void ConfigPersisterRepositorySaveCompleted(object sender, SaveCompletedEventArgs e)
        {
            SetIsNotSavingConfig();
        }

        private void ReloadViewModel()
        {
            ClearChangesets();
            lastRevision = 1;
            LoadData();
        }

        private void ClearChangesets()
        {
            var count = ViewModel.Changesets.Count;
            for (int i = 0; i < count; i++)
            {
                ViewModel.Changesets.RemoveAt(0);
            }
        }

        private void LoadSettingsAndData()
        {
            asyncClient.RunAsyncVoid(() => Try(() =>
            {
                LoadSettingsSync();
                LoadDataSync();
            }));
        }

        private void LoadSettingsSync()
        {
            SetIsLoadingConfig();
            var currentSettings = configRepo.Get(new ConfigurationByName(SettingsEntryName)).SingleOrDefault();
            
          

            if (ConfigurationHasSettings(currentSettings) && DbSettingsIsChanged(currentSettings))
            {
                SetSettings(currentSettings);
            }
            if (ConfigurationHasSettings(currentSettings))
            {
                SetColorSettings(currentSettings);
            }

            uiInvoker.Invoke(() => ViewModel.SetResetPoint());
            
            SetIsNotLoadingConfig();
        }

        private static bool ConfigurationHasSettings(Configuration currentSettings)
        {
            return currentSettings != null && currentSettings.Settings.Count() != 0;
        }

        private void LoadData()
        {
            if (!ViewModel.IsLoading)
            {
                asyncClient.RunAsyncVoid(LoadDataSync);
            }
        }

        private void LoadDataSync()
        {
            SetIsLoadingData();
            var allUsers = GetAllUsers();
            var latestChangesets = GetLatestChangesets(ViewModel.NumberOfCommits);

            LoadChangesetIntoViewModel(latestChangesets);
            LoadUserInfoIntoViewModel(allUsers);

            SetIsNotLoadingData();
        }

        public bool DbSettingsIsChanged(Configuration dbSettings)
        {
            var dbNumberOfCommits = Int32.Parse(dbSettings.GetSetting(NumberOfCommittsEntryName).Value.Trim());
            var dbBlinkIsChecked = Boolean.Parse(dbSettings.GetSetting(BlinkIsCheckedEntryName).Value.Trim());
            
            return (dbNumberOfCommits != ViewModel.NumberOfCommits) || (dbBlinkIsChecked != ViewModel.BlinkWhenNoComment);
        }

        private void SetSettings(Configuration settings)
        {
            if (settings == null)
                return;

            Try(() => LoadSpecificSetting(settings.GetSetting(NumberOfCommittsEntryName)));
            Try(() => LoadSpecificSetting(settings.GetSetting(BlinkIsCheckedEntryName)));
        }

        private void SetColorSettings(Configuration settings)
        {
            Try(() => LoadColorSetting(settings.GetSetting(KeywordsEntryName)));
        }
        
        delegate void VoidDelegate();
        private void Try(VoidDelegate fn)
        {
            try
            {
                fn();
            }
            catch (Exception exception)
            {
                LogErrorMsg(exception);
            }
        }

        private void LoadSpecificSetting(SettingsEntry setting)
        {
            if (setting.Value == null) return;

            switch (setting.Name)
            {
                case NumberOfCommittsEntryName:
                    uiInvoker.Invoke(() =>
                    {
                        ViewModel.NumberOfCommits = Int32.Parse(setting.Value.Trim());
                    });
                    break;
                case BlinkIsCheckedEntryName:
                    uiInvoker.Invoke(() =>
                    {
                        ViewModel.BlinkWhenNoComment = Boolean.Parse(setting.Value.Trim());
                    });
                    break;
            }
        }

        private void LoadColorSetting(SettingsEntry setting)
        {
            uiInvoker.Invoke(() =>
            {
                ViewModel.KeywordList.Clear();
                if (!setting.HasMultipleValues) return;

                var config = setting.Vals.ToArray();
                for (int i = 0; i < config.Count(); i += 2)
                {
                    ViewModel.KeywordList.Add(new KeywordColorPairViewModel() { Keyword = config[i], ColorName = config[i + 1], KeywordChanged = this.KeywordChangedHandler});
                }
            });
        }

        private void LogErrorMsg(Exception exception)
        {
            logger.WriteEntry(new ErrorLogEntry()
            {
                Message = exception.ToString(),
                Source = GetType().ToString(),
                TimeStamp = DateTime.Now
            });
        }

        private IEnumerable<Changeset> GetLatestChangesets(int numberOfChangesets)
        {
            
            IEnumerable<Changeset> changeSets = null;
            Try(() => 
                changeSets = repository.Get(new ChangesetsAfterRevisionSpecification(lastRevision - ViewModel.NumberOfCommits)));
            
            if (changeSets == null)
            {
                throw new Exception(
                    "Violation of IChangesetRepository. Does not accept a null reference as a return value.");
            }

            return changeSets.Take(numberOfChangesets).ToList();
        }

        private void LoadChangesetIntoViewModel(IEnumerable<Changeset> changesets)
        {
            if (changesets.Count() > 0)
            {
                lastRevision = changesets.Max(c => c.Revision);
            }

            uiInvoker.Invoke(() => LoadChangesetsIntoViewModelSync(changesets));
        }

        private void LoadChangesetsIntoViewModelSync(IEnumerable<Changeset> changesets)
        {
            foreach (var changeset in changesets.Where(ViewModelDoesNotContainChangeset).OrderBy(c => c.Revision))
                AddChangeSetToViewModel(changeset);

            KeepLatestCommits();
        }

        private bool ViewModelDoesNotContainChangeset(Changeset changeset)
        {
            return !ViewModel.Changesets.Any(c => c.Revision.Equals(changeset.Revision));
        }

        private void AddChangeSetToViewModel(Changeset changeset)
        {
            var newChangeset = new ChangesetViewModel
                                   {
                                       Message = changeset.Comment,
                                       Date = changeset.Time,
                                       Revision = changeset.Revision,
                                       CommentIsBad = !changeset.IsValidComment(),
                                       ShouldBlink = ViewModel.BlinkWhenNoComment,
                                       BackgroundColor = FindBrushForChangeset(changeset.Comment)
                                   };

            newChangeset.Developer.Username = changeset.Author.Username;

            ViewModel.Changesets.Insert(0, newChangeset);
        }

        private string FindBrushForChangeset(String changesetComment)
        {
            string comment = changesetComment.ToLower();
            foreach (var binding in ViewModel.KeywordList)
            {
                var lowerCaseKeyword = binding.Keyword.ToLower();
                if (lowerCaseKeyword.Length == 0)
                    continue;
                if (comment.Contains(lowerCaseKeyword))
                    return ChangesetBackgroundProvider.GetBrushName(binding.ColorName);
            }
            return ChangesetViewModel.DEFAULT_BACKGROUND_COLOR;
        }


        private void KeepLatestCommits()
        {
            while (ViewModel.Changesets.Count > ViewModel.NumberOfCommits)
                ViewModel.Changesets.RemoveAt(ViewModel.Changesets.Count - 1);
        }

        private IEnumerable<User> GetAllUsers()
        {
            return userRepository.Get(new AllSpecification<User>());
        }

        private void LoadUserInfoIntoViewModel(IEnumerable<User> users)
        {
            uiInvoker.Invoke(() =>
            {
                foreach (var user in users)
                {
                    var csViewModels =
                        ViewModel.Changesets.Where(c => c.Developer.Username == user.Username);
                    foreach (var csViewModel in csViewModels)
                    {
                        csViewModel.Developer.Firstname = user.Firstname;
                        csViewModel.Developer.Middlename = user.Middlename;
                        csViewModel.Developer.Surname = user.Surname;
                        csViewModel.Developer.Email = user.Email;
                        csViewModel.Developer.ImageUrl = user.ImageUrl;
                    }
                }
            });
        }
    }
}