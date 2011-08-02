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
using System.Linq;
using Smeedee.Client.Framework.Controller;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.SourceControl;
using Smeedee.Framework;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widgets.SourceControl.Controllers
{
    public abstract class ChangesetControllerBase<T> : ControllerBase<BindableViewModel<T>>
        where T : AbstractViewModel
    {
        protected IAsyncRepository<Changeset> changesetRepository;
        protected ILog logger;
        protected bool configIsChanged;

        protected ChangesetControllerBase(
            BindableViewModel<T> viewModel, 
            IAsyncRepository<Changeset> changesetRepo,
            ITimer timer, 
            IUIInvoker uiInvoke,
            ILog logger,
            IProgressbar loadingNotifier)
            : base(viewModel, timer, uiInvoke, loadingNotifier)
        {
            Guard.Requires<ArgumentException>(logger != null, "logger");
            Guard.Requires<ArgumentException>(changesetRepo != null, "changesetRepo");
            
            this.logger = logger;
            this.changesetRepository = changesetRepo;
            this.changesetRepository.GetCompleted += OnGetCompleted;
        }

        protected void LoadData()
        {
            LoadData(new AllChangesetsSpecification());
        }

        protected void LoadData(Specification<Changeset> specification)
        {
            if (!ViewModel.IsLoading)
            {
                SetIsLoadingData();
                QueryChangesets(specification);
            }
        }
        
        protected void QueryChangesets(Specification<Changeset> specification)
        {
            changesetRepository.BeginGet(specification);
        }

        private void OnGetCompleted(object sender, GetCompletedEventArgs<Changeset> e)
        {
            if (e.Error != null)
            {
                LogErrorMsg(e.Error);
                ViewModel.HasConnectionProblems = true;
            } else
            {
                ViewModel.HasConnectionProblems = false;
                
                if (e.Result == null)
                {
                    throw new Exception(
                        "Violation of IChangesetRepository. Does not accept a null reference as a return value.");
                }

                AfterQueryAllChangesets();
                try
                {
                    LoadDataIntoViewModel(e.Result);
                }
                catch (Exception exception)
                {
                    LogErrorMsg(exception);
                    ViewModel.HasConnectionProblems = true;
                }
            }
            SetIsNotLoadingData();
        }     

        protected void UpdateRevision(IEnumerable<Changeset> changesets)
        {
            if (changesets == null || changesets.Count() == 0) return;

            var newMaxRevision = changesets.Max(c => c.Revision);
            if (newMaxRevision > ViewModel.CurrentRevision || configIsChanged)
            {
                ViewModel.CurrentRevision = newMaxRevision;
            }
        }

        protected void LogErrorMsg(Exception exception)
        {
            logger.WriteEntry(ErrorLogEntry.Create(this, exception.ToString()));
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