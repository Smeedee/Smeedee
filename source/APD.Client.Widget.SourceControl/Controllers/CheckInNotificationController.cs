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
using APD.Client.Framework.ViewModels;
using APD.Client.Widget.SourceControl.ViewModels;
using APD.DomainModel.Framework;
using APD.DomainModel.Framework.Logging;
using APD.DomainModel.SourceControl;
using APD.DomainModel.Users;


namespace APD.Client.Widget.SourceControl.Controllers
{
    public class CheckInNotificationController : ControllerBase<CheckInNotificationViewModel>
    {
        private const int NUMBER_OF_CHANGESETS_TO_DISPLAY = 10;
        private const int NUMBER_OF_CHANGESETS_TO_GET = 10;

        private IInvokeBackgroundWorker<IEnumerable<Changeset>> asyncClient;
        private long lastRevision = 1;
        private IRepository<Changeset> repository;
        private IRepository<User> userRepository;

        private ILog logger;

        public CheckInNotificationController(IRepository<Changeset> repository,
                                             IRepository<User> userRepository,
                                             INotifyWhenToRefresh refreshNotifier,
                                             IInvokeUI uiInvoker,
                                             IInvokeBackgroundWorker<IEnumerable<Changeset>> backgroundWorker,
                                             ILog logger)
            : base(refreshNotifier, uiInvoker)
        {
            asyncClient = backgroundWorker;

            this.repository = repository;
            this.userRepository = userRepository;
            this.refreshNotifier = refreshNotifier;
            this.logger = logger;

            LoadData();
        }

        protected override void OnNotifiedToRefresh(object sender, RefreshEventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            if (!ViewModel.IsLoading)
            {
                ViewModel.IsLoading = true;

                asyncClient.RunAsyncVoid(() =>
                {
                    IEnumerable<User> allUsers = GetAllUsers(); //new List<User>(); // GetAllUsers();
                    IEnumerable<Changeset> latestChangesets = GetLatestChangesets(NUMBER_OF_CHANGESETS_TO_GET);
                    LoadChangesetIntoViewModel(latestChangesets);
                    LoadUserInfoIntoViewModel(allUsers);

                    if (latestChangesets.Count() == 0)
                    {
                        ViewModel.NoChangesets = true;
                    }

                    ViewModel.IsLoading = false;
                });
            }
        }

        private IEnumerable<Changeset> GetLatestChangesets(int count)
        {
            IEnumerable<Changeset> changeSets = null;
            try
            {
                changeSets = repository.Get(new ChangesetsAfterRevisionSpecification(lastRevision));
                ViewModel.HasConnectionProblems = false;
            }
            catch(Exception e)
            {
                logger.WriteEntry(new LogEntry()
                {
                    Message = e.ToString(),
                    Source = this.GetType().ToString(),
                    TimeStamp = DateTime.Now
                });
                ViewModel.HasConnectionProblems = true;
            }

            if (changeSets == null)
            {
                throw new ImplementationViolationException(
                    "Violation of IChangesetRepository. Does not accept a null reference as a return value.");
            }

            return changeSets.Take(count).ToList();
        }

        private void LoadChangesetIntoViewModel(IEnumerable<Changeset> changesets)
        {
            if (changesets.Count() > 0)
            {
                lastRevision = changesets.Max(c => c.Revision);
            }

            uiInvoker.Invoke(() =>
            {
                ChangesetViewModel newChangeset;

                foreach (Changeset changeset in changesets.Reverse())
                {
                    newChangeset = new ChangesetViewModel(ViewModel.Invoker)
                    {
                        Message = changeset.Comment,
                        Date = changeset.Time.ToLocalTime(),
                        Revision = changeset.Revision,
                        CommentIsBad = !changeset.IsValidComment(),
                    };

                    newChangeset.Developer.Username = changeset.Author.Username;

                    //check if newChangeset is in list already
                    bool newChangesetExistsInList =
                        ViewModel.Changesets.Any(c => c.Revision.Equals(newChangeset.Revision));

                    if (!newChangesetExistsInList)
                    {
                        ViewModel.Changesets.Insert(0, newChangeset);

                        if (ViewModel.Changesets.Count > NUMBER_OF_CHANGESETS_TO_DISPLAY)
                        {
                            ViewModel.Changesets.RemoveAt(ViewModel.Changesets.Count - 1);
                        }
                    }
                }
            });
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