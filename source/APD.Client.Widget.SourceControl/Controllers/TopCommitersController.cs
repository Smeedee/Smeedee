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

using System.Collections.Generic;
using System.Linq;
using APD.Client.Framework;
using APD.Client.Widget.SourceControl.ViewModels;
using APD.DomainModel.Framework.Logging;
using APD.DomainModel.SourceControl;
using APD.DomainModel.Framework;
using APD.DomainModel.Users;


namespace APD.Client.Widget.SourceControl.Controllers
{
    public class TopCommitersController : ChangesetStandAloneController<CodeCommiterViewModel>
    {
        private IEnumerable<User> allUsers;
        private IRepository<User> userRepository;

        public TopCommitersController(INotifyWhenToRefresh refreshNotifier,
                                      IRepository<Changeset> changesetRepository, 
                                      IRepository<User> userRepository,
                                      IInvokeUI uiInvoker,
                                      IInvokeBackgroundWorker<IEnumerable<Changeset>> backgroundWorkerInvoker,
                                      ILog logger)
            : base(refreshNotifier, changesetRepository, uiInvoker, backgroundWorkerInvoker, logger)
        {
            this.userRepository = userRepository;
            LoadData();
        }


        protected override void LoadDataIntoViewModel(IEnumerable<Changeset> qAllChangesets)
        {
            var committers = ( from changeset in qAllChangesets
                      group changeset by changeset.Author.Username
                      into g
                          select new CodeCommiterViewModel(ViewModel.Invoker)
                                 {
                                     Username = g.Key, 
                                     Firstname = g.Key,
                                     NumberOfCommits = g.Count()
                                 }).OrderByDescending(r => r.NumberOfCommits);

            if (HasUpdatedChangesets(committers))
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
            else
            {
                UpdateNumberOfCommits(committers);
            }
        }

        protected void UpdateNumberOfCommits(IOrderedEnumerable<CodeCommiterViewModel> committers)
        {

            var committersArray = committers.ToArray();

            for (int i = 0; i < ViewModel.Data.Count; i++)
                ViewModel.Data[i].NumberOfCommits = committersArray[i].NumberOfCommits;

        }

        protected override void OnNotifiedToRefresh(object sender, RefreshEventArgs e)
        {
            LoadData();
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
            //allUsers = new List<User>();
        }
    }
}