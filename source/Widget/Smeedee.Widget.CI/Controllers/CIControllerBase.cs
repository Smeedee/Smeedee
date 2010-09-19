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
// The project webpage is located at http://www.smeedee.org
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Smeedee.Client.Framework.Controller;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.Users;
using Smeedee.Widget.CI.ViewModels;
using TinyMVVM.Framework;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widget.CI.Controllers
{
    public enum CIProjectBuildStatusChangeEvents
    {
        ABuildFailed,
        AllBuildsSuccessful,
    }

    public abstract class CIControllerBase<T> : ControllerBase<T>
        where T : AbstractViewModel
    {
        private const int INACTIVE_PROJECT_THRESHOLD = 90;
        private const int REFREASH_INTERVAL = 3 * 60 * 1000;
        private readonly IRepository<CIServer> ciProjectRepository;
        protected readonly IRepository<User> userRepository;


        private IInvokeBackgroundWorker<IEnumerable<CIProject>> asyncClient;
        private readonly ILog logger;

        
        private bool isFrozen = false;
        private bool isLoading = false;


        public CIControllerBase(T viewModel, IRepository<CIServer> ciProjectRepository, IRepository<User> userRepository, IInvokeBackgroundWorker<IEnumerable<CIProject>> asyncClient, ITimer timer, IUIInvoker uiInvoke, ILog logger, IProgressbar loadingNotifier)
            : base(viewModel, timer, uiInvoke, loadingNotifier)
        {
            this.ciProjectRepository = ciProjectRepository;
            this.asyncClient = asyncClient;
            this.userRepository = userRepository;
            this.logger = logger;

            refreshNotifier.Start(REFREASH_INTERVAL);

            LoadData();
        }

        private void LoadData()
        {
            asyncClient.RunAsyncVoid(() =>
            {
                SetIsLoadingData();
                IEnumerable<CIProject> projects = TryGetProjects();
                bool gotProjects = projects != null;
                if (gotProjects)
                {
                    LoadDataIntoViewModel(projects);
                    SetIsNotLoadingData();
                }
                SetIsNotLoadingData();
            });
        }

        protected override void OnNotifiedToRefresh(object sender, EventArgs e)
        {
            if (!ViewModel.IsLoading)
                LoadData();
        }

        private IEnumerable<CIProject> TryGetProjects()
        {
            IEnumerable<CIProject> projects = null;

            try
            {
                var qServers = ciProjectRepository.Get(new AllSpecification<CIServer>());
                if (qServers.Count() > 0)
                    projects = qServers.First().Projects;
                ViewModel.HasConnectionProblems = false;

                if (projects != null)
                {
                    var activeProjects = RemoveInactiveProjects(projects);
                    projects = SortProjects(activeProjects);
                }

            }
            catch (Exception exception)
            {
                ViewModel.HasConnectionProblems = true;
                logger.WriteEntry(new ErrorLogEntry()
                {
                    Message = exception.ToString(),
                    Source = this.GetType().ToString(),
                    TimeStamp = DateTime.Now
                });
            }
            return projects;
        }

        private IOrderedEnumerable<CIProject> SortProjects(IEnumerable<CIProject> activeProjects)
        {
            return from c in activeProjects
                   orderby c
                   select c;
        }

        private IEnumerable<CIProject> RemoveInactiveProjects(IEnumerable<CIProject> projects)
        {
            return projects.Where(
                p => p.LatestBuild.StartTime > DateTime.Now.AddDays(-INACTIVE_PROJECT_THRESHOLD));
        }

        protected abstract void LoadDataIntoViewModel(IEnumerable<CIProject> projects);
    }   
}