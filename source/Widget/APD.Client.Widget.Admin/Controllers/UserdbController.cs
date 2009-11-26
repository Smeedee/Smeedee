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
// </copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.Linq;
using APD.Client.Framework;
using APD.DomainModel.Users;
using APD.Client.Widget.Admin.ViewModels;
using APD.DomainModel.Framework;
using APD.Client.Framework.Controllers;
using System.Collections;
using System.Collections.Generic;

namespace APD.Client.Widget.Admin.Controllers
{
    public class UserdbController : ControllerBase<UserdbViewModel>
    {
        private UserdbViewModel viewModel;
        private IRepository<Userdb> userdbRepository;
        private IInvokeBackgroundWorker<IEnumerable<Userdb>> backgroundWorker;
        private INotifyWhenToRefresh refreshNotifier;
        private IPersistDomainModels<Userdb> userdbPersistanceRepository;
        private INotify<EventArgs> saveNotifier;
        private INotify<EventArgs> editNotifier;
        private IInvokeUI uiInvoker;
        private ITriggerEvent<ShowModalDialogEventArgs> showModalDialogEventTrigger;

        public UserdbController(UserdbViewModel viewModel, IRepository<Userdb> userdbRepository, 
            IInvokeBackgroundWorker<IEnumerable<Userdb>> backgroundWorker, 
            INotifyWhenToRefresh refreshNotifier, IPersistDomainModels<Userdb> userdbPersistanceRepository, 
            INotify<EventArgs> saveNotifier, INotify<EventArgs> editNotifier, 
            ITriggerEvent<ShowModalDialogEventArgs> showModalDialogEventTrigger, 
            IInvokeUI uiInvoker)
            : base(refreshNotifier, uiInvoker, viewModel)
        {
            this.viewModel = viewModel;
            this.userdbRepository = userdbRepository;
            this.backgroundWorker = backgroundWorker;
            this.refreshNotifier = refreshNotifier;
            this.userdbPersistanceRepository = userdbPersistanceRepository;
            this.saveNotifier = saveNotifier;
            this.uiInvoker = uiInvoker;
            this.editNotifier = editNotifier;
            this.showModalDialogEventTrigger = showModalDialogEventTrigger;


            this.refreshNotifier.Refresh += new EventHandler<RefreshEventArgs>(refreshNotifier_NewNotification);
            this.saveNotifier.NewNotification += new EventHandler<EventArgs>(saveNotifier_NewNotification);
            this.editNotifier.NewNotification += new EventHandler<EventArgs>(editNotifier_NewNotification);

            LoadData();
        }

        void editNotifier_NewNotification(object sender, EventArgs e)
        {
            if (showModalDialogEventTrigger != null)
                showModalDialogEventTrigger.NewEvent(new ShowModalDialogEventArgs()
                {
                    ViewModel = viewModel
                });
        }

        void saveNotifier_NewNotification(object sender, EventArgs e)
        {
            SaveData();
        }

        private void SaveData()
        {
            var data = ExtractDomainModelFromViewModel();

            viewModel.IsLoading = true;
            backgroundWorker.RunAsyncVoid(() =>
            {
                PersistDomainModel(data);
                viewModel.IsLoading = false;
            });
        }

        private void PersistDomainModel(Userdb data)
        {
            userdbPersistanceRepository.Save(data);
        }

        private Userdb ExtractDomainModelFromViewModel() 
        {
            var data = new Userdb("default");
            data.Name = viewModel.Name;

            var users = from u in viewModel.Data
                        select new User()
                               {
                                   Email = u.Email,
                                   ImageUrl = u.ImageUrl,
                                   Firstname = u.Firstname,
                                   Middlename = u.Middlename,
                                   Surname = u.Surname,
                                   Username = u.Username
                               };
            foreach (User user in users)
                data.AddUser(user);
            return data;
        }

        void refreshNotifier_NewNotification(object sender, EventArgs e)
        {
            LoadData();
        }

        private bool loadingFlag;

        private void LoadData()
        {
            if (!loadingFlag)
            {
                viewModel.IsLoading = true;
                backgroundWorker.RunAsyncVoid(() =>
                {
                    loadingFlag = true;
                    var data = LoadDomainModelFromRepository();
                    uiInvoker.Invoke(() =>
                    {
                        ExtractViewModelFromDomainModel(data);
                    });
                    loadingFlag = false;
                    viewModel.IsLoading = false;
                });

            }
        }

        private void ExtractViewModelFromDomainModel(Userdb userdb)
        {
            viewModel.Data.Clear();

            if(userdb == null)
            {
                return;
            }
                
            
            viewModel.Name = userdb.Name;

            var users = from u in userdb.Users
                        select new UserViewModel(uiInvoker)
                               {
                                   Email = u.Email,
                                   ImageUrl = u.ImageUrl,
                                   Firstname = u.Firstname,
                                   Middlename = u.Middlename,
                                   Surname = u.Surname,
                                   Username = u.Username
                               };

            foreach (UserViewModel uservm in users)
                viewModel.Data.Add(uservm);
        }

        private Userdb LoadDomainModelFromRepository()
        {
            //Todo: Refactor. This is a work-around since IRepository API doesnt support single resultset

            var userdbs = userdbRepository.Get(new AllSpecification<Userdb>());

            if (userdbs.Count() > 0)
                return userdbs.First();

            return new Userdb("default");
        }

        protected override void OnNotifiedToRefresh(object sender, RefreshEventArgs e)
        {
        }
    }
}