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
using Smeedee.Client.Framework;
using Smeedee.Client.Framework.Controller;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.SourceControl;
using TinyMVVM.Framework;
using TinyMVVM.Framework.Services;


namespace Smeedee.Widget.SourceControl.Controllers
{
    public abstract class ChangesetStandAloneController<T> : ControllerBase<BindableViewModel<T>>
        where T : AbstractViewModel
    {
        protected IRepository<Changeset> changesetRepository;
        protected IInvokeBackgroundWorker<IEnumerable<Changeset>> asyncClient;

        protected ILog logger;

        protected ChangesetStandAloneController(BindableViewModel<T> viewModel, IRepository<Changeset> changesetRepo, IInvokeBackgroundWorker<IEnumerable<Changeset>> asyncClient, ITimer timer, IUIInvoker uiInvoke, ILog logger)
            : base(viewModel, timer, uiInvoke)
        {
            this.logger = logger;
            this.changesetRepository = changesetRepo;
            this.asyncClient = asyncClient;
        }


        protected void LoadData()
        {
            LoadData(new AllChangesetsSpecification());
        }

        protected void LoadData(Specification<Changeset> specification)
        {
            if (!ViewModel.IsLoading)
            {
                ViewModel.IsLoading = true;
                asyncClient.RunAsyncVoid(() =>
                {
                    var changesets = QueryChangesets(specification);

                    uiInvoker.Invoke(() =>
                    {
                        try
                        {
                            LoadDataIntoViewModel(changesets);
                        }
                        catch (Exception e)
                        {
                            LogErrorMsg(e);
                            ViewModel.HasConnectionProblems = true;
                        }
                    });
                    ViewModel.IsLoading = false;
                });
            }
        }


        protected IEnumerable<Changeset> QueryChangesets(Specification<Changeset> specification)
        {
            IEnumerable<Changeset> changesets = null;
            try
            {
                changesets = changesetRepository.Get(specification);
                ViewModel.HasConnectionProblems = false;
                AfterQueryAllChangesets();
            }
            catch (Exception e)
            {
                LogErrorMsg(e);
                ViewModel.HasConnectionProblems = true;
            }

            if (changesets == null)
            {
                throw new Exception(
                    "Violation of IChangesetRepository. Does not accept a null reference as a return value.");
            }

            return changesets;

        }

        protected void LogErrorMsg(Exception exception)
        {
            logger.WriteEntry(new ErrorLogEntry()
            {
                Message = exception.ToString(),
                Source = this.GetType().ToString(),
                TimeStamp = DateTime.Now
            });
        }

        protected void LogWarningMsg (Exception exception)
        {
            logger.WriteEntry(new WarningLogEntry()
            {
                Message = exception.ToString(),
                Source = this.GetType().ToString(),
                TimeStamp = DateTime.Now
            });
        }
        

        protected abstract void LoadDataIntoViewModel(IEnumerable<Changeset> qChangesets);

        protected abstract override void OnNotifiedToRefresh(object sender, EventArgs e);

        protected virtual void AfterQueryAllChangesets()
        {
            
        }

        
    }
}