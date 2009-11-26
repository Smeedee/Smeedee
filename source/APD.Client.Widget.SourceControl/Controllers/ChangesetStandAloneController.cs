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

using APD.Client.Framework;
using APD.Client.Framework.Controllers;
using APD.Client.Framework.ViewModels;
using APD.DomainModel.SourceControl;
using APD.DomainModel.Framework;


namespace APD.Client.Widget.SourceControl.Controllers
{
    public abstract class ChangesetStandAloneController<T> : ControllerBase<BindableViewModel<T>>
        where T : AbstractViewModel
    {
        protected IRepository<Changeset> changesetRepository;
        protected IInvokeBackgroundWorker<IEnumerable<Changeset>> asyncClient;

        protected ChangesetStandAloneController(INotifyWhenToRefresh refreshNotifier,
                                                IRepository<Changeset> changesetRepository,
                                                IInvokeUI uiInvoker,
                                                IInvokeBackgroundWorker<IEnumerable<Changeset>> backgroundWorkerInvoker)
            : base(refreshNotifier, uiInvoker)
        {
            this.changesetRepository = changesetRepository;

            asyncClient = backgroundWorkerInvoker;
        }


        protected void LoadData()
        {
            if (!ViewModel.IsLoading)
            {
                ViewModel.IsLoading = true;
                asyncClient.RunAsyncVoid(() =>
                {
                    var qAllChangesets = QueryAllChangesets();

                    uiInvoker.Invoke(() =>
                    {
                        try
                        {
                            LoadDataIntoViewModel(qAllChangesets);
                        }
                        catch
                        {
                            ViewModel.HasConnectionProblems = true;
                        }
                    });
                    ViewModel.IsLoading = false;
                });
            }
        }


        protected IEnumerable<Changeset> QueryAllChangesets()
        {
            IEnumerable<Changeset> changesets = null;
            try
            {
                changesets = changesetRepository.Get(new AllChangesetsSpecification());
                ViewModel.HasConnectionProblems = false;
                AfterQueryAllChangesets();
            }
            catch
            {
                ViewModel.HasConnectionProblems = true;
            }

            return changesets;
        }


        protected abstract void LoadDataIntoViewModel(IEnumerable<Changeset> qAllChangesets);

        protected abstract override void OnNotifiedToRefresh(object sender, RefreshEventArgs e);

        protected virtual void AfterQueryAllChangesets()
        {
            
        }
    }
}