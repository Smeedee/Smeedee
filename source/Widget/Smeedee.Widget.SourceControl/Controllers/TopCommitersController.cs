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
using System.Globalization;
using System.Linq;
using Smeedee.Client.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.SourceControl;
using Smeedee.DomainModel.Users;
using Smeedee.Widget.SourceControl.ViewModels;
using TinyMVVM.Framework;
using TinyMVVM.Framework.Services;


namespace Smeedee.Widget.SourceControl.Controllers
{
    public class TopCommitersController : ChangesetStandAloneController<CodeCommiterViewModel>
    {
        private IEnumerable<User> allUsers;
        private IRepository<User> userRepository;
        private readonly IRepository<Configuration> configRepository;
        private readonly IPersistDomainModels<Configuration> configPersisterRepository;
        private long lastRevision = 1;
        public DateTime lastUpdated = DateTime.Now;   
        private int commitTimespan;
        private DateTime commitFromDate;
        private bool useFromDate;
        private bool configurationChanged;


        public TopCommitersController(
            BindableViewModel<CodeCommiterViewModel> viewModel, 
            IRepository<Changeset> changesetRepo, 
            IInvokeBackgroundWorker<IEnumerable<Changeset>> backgroundWorker, 
            ITimer timer, IUIInvoker uiInvoke, 
            IRepository<Configuration> configRepo, 
            IPersistDomainModels<Configuration> configPersister,
            IRepository<User> userRepo,
            ILog logger)
            : base(viewModel, changesetRepo, backgroundWorker, timer, uiInvoke, logger)
        {
            this.userRepository = userRepo;
            this.configRepository = configRepo;
            this.configPersisterRepository = configPersister;

            refreshNotifier.Start(60*5*1000);

            LoadConfig();
            LoadData();
        }

        private void LoadConfig()
        {
            asyncClient.RunAsyncVoid(() =>
            {
                try
                {
                    var allConfigSpec = new AllSpecification<Configuration>();
                    var config = configRepository
                        .Get(allConfigSpec)
                        .Where(c => c.Name.Equals(("Commit Heroes Slide")))
                        .SingleOrDefault();

                    LoadSettings(config);
                }
                catch (Exception exception)
                {
                    LogErrorMsg(exception);
                }
            });
        }

       
        private void LoadSettings(Configuration config)
        {
            if (config != null)
            {
                LoadTimespanSetting(config.GetSetting("timespan"));
                LoadFromDateSetting(config.GetSetting("from date"));
            }
            else
            {
                CreateDefaultSetting();
            }
        }

        private void LoadFromDateSetting(SettingsEntry fromDateSetting)
        {
            if (fromDateSetting != null)
            {
                try
                {
                    var newConfigValue = DateTime.Parse(fromDateSetting.Value.Trim(), new CultureInfo("en-US"));
                    configurationChanged = commitFromDate != newConfigValue;
                    commitFromDate = newConfigValue;
                    useFromDate = true;
                }
                catch (Exception exception)
                {
                    LogWarningMsg(exception);
                }
            }
        }

        private void LoadTimespanSetting(SettingsEntry timespanSetting)
        {
            if (timespanSetting != null)
            {
                try
                {
                    var newConfigValue = Int32.Parse(timespanSetting.Value.Trim());
                    configurationChanged = commitTimespan != newConfigValue;
                    commitTimespan = newConfigValue;
                }
                catch (Exception exception)
                {
                    LogWarningMsg(exception);
                }
            }
        }

        private void CreateDefaultSetting() 
        {
            var newConfig = new Configuration("Commit Heroes Slide");
            newConfig.NewSetting("from date", "");
            newConfig.NewSetting("timespan", "0");
            configPersisterRepository.Save(newConfig);
        }

        protected override void LoadDataIntoViewModel(IEnumerable<Changeset> qChangesets)
        {
            qChangesets = qChangesets.Where(c => c != null && c.Author != null && c.Author.Username != null);

            if (qChangesets != null && qChangesets.Count() != 0)
            {
                lastRevision = qChangesets.OrderByDescending(c => c.Revision).First().Revision;
            }

            var committers = GetFilteredCommitters(qChangesets);

            var mergedCommiters = GetMergedCommiters(committers);

            committers = mergedCommiters.OrderByDescending(c => c.NumberOfCommits);
            
            if (HasUpdatedChangesets((IOrderedEnumerable<CodeCommiterViewModel>) committers))
            {
                ReloadCommitters(committers);
            }
            else
            {
                UpdateNumberOfCommits((IOrderedEnumerable<CodeCommiterViewModel>) committers);
            }

            UpdateSinceDateValueInViewModel();
        }

        private void ReloadCommitters(IEnumerable<CodeCommiterViewModel> committers) {
            
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

        private IEnumerable<CodeCommiterViewModel> GetFilteredCommitters(IEnumerable<Changeset> qChangesets)
        {
            var filterDate = useFromDate ? commitFromDate : DateTime.Today.AddDays(-commitTimespan);
            
            return ( from changeset in qChangesets
                     where changeset.Time >= filterDate.Date
                     group changeset by changeset.Author.Username
                     into g
                         select new CodeCommiterViewModel()
                         {
                             Username = g.Key, 
                             Firstname = g.Key,
                             NumberOfCommits = g.Count()
                         });
        }

        private ObservableCollection<CodeCommiterViewModel> GetMergedCommiters(IEnumerable<CodeCommiterViewModel> committers) {
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

        protected void UpdateNumberOfCommits(IOrderedEnumerable<CodeCommiterViewModel> committers)
        {

            var committersArray = committers.ToArray();

            for (int i = 0; i < ViewModel.Data.Count; i++)
                ViewModel.Data[i].NumberOfCommits = committersArray[i].NumberOfCommits;

        }

        private void UpdateSinceDateValueInViewModel()
        {
            if (useFromDate)
            {
                ViewModel.SinceDate = commitFromDate;
            }
            else
            {
                ViewModel.SinceDate = DateTime.Now.AddDays(-commitTimespan);
            }
        }

        protected override void OnNotifiedToRefresh(object sender, EventArgs e)
        {
            LoadConfig();

            var newDayForTimespan = !useFromDate && lastUpdated.Date < DateTime.Now.Date;
            
            if (newDayForTimespan||configurationChanged)  
            {
                ReloadViewModelData();
            }
            else
            {
                LoadData(new ChangesetsAfterRevisionSpecification(lastRevision));    
            }
            
        }

        private void ReloadViewModelData()
        {
            uiInvoker.Invoke(() =>
            {
                ViewModel.Data.Clear();
                LoadData();
                lastUpdated = DateTime.Now;
                configurationChanged = false;
            });
        }

        protected bool HasUpdatedChangesets(IOrderedEnumerable<CodeCommiterViewModel> committers)
        {
           
            if (committers.Count() != ViewModel.Data.Count)
                return true;

            var committersArray = committers.ToArray();
            
            for (int i = 0; i < ViewModel.Data.Count; i++)
            {
                if (!ViewModel.Data[i].Username.Equals(committersArray[i].Username))
                    return true;
            }

            return false;

        }

        protected override void AfterQueryAllChangesets()
        {
            allUsers = userRepository.Get(new AllSpecification<User>());
        }
    }
}