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

    public class CIController : ControllerBase<CIViewModel>
    {
        private const int INACTIVE_PROJECT_THRESHOLD = 90;
        private readonly IRepository<CIServer> ciProjectRepository;
        private readonly IRepository<User> userRepository;

        private IInvokeBackgroundWorker<IEnumerable<CIProject>> asyncClient;
        private readonly ILog logger;

        private bool isSoundEnabled = false;
        private bool isFrozen = false;
        private bool isLoading = false;

        #region Constructor

        public CIController(CIViewModel viewModel, IRepository<CIServer> ciProjectRepository, IRepository<User> userRepository, IInvokeBackgroundWorker<IEnumerable<CIProject>> asyncClient, ITimer timer, IUIInvoker uiInvoke, ILog logger)
            : base(viewModel, timer, uiInvoke)
        {
            this.ciProjectRepository = ciProjectRepository;
            this.asyncClient = asyncClient;
            this.userRepository = userRepository;
            this.logger = logger;

            refreshNotifier.Start(10000);

            LoadData();
        }

        #endregion

        private bool IsLoading()
        {
            return isLoading;
        }

        private void FlagIsLoading()
        {
            isLoading = true;
        }

        private void UnflagIsLoading()
        {
            isLoading = false;
        }

        private void LoadData()
        {
            asyncClient.RunAsyncVoid(() =>
            {
                FlagIsLoading();
                ViewModel.IsLoading = true;
                IEnumerable<CIProject> projects = TryGetProjects();
                bool gotProjects = projects != null;
                if (gotProjects)
                {
                    LoadDataIntoViewModel(projects);
                    ViewModel.IsLoading = false;
                    //CheckProjectBuildStatusAndNotify(projects);
                }
                UnflagIsLoading();
            });
        }

        protected override void OnNotifiedToRefresh(object sender, EventArgs e)
        {
            if (!IsLoading())
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
            catch( Exception exception )
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

        private void LoadDataIntoViewModel(IEnumerable<CIProject> projects)
        {
            uiInvoker.Invoke(() =>
            {
                foreach (var projectInfo in projects)
                    LoadProjectInfoIntoView(projectInfo);
            });
        }

        //private void CheckProjectBuildStatusAndNotify(IEnumerable<CIProject> projects)
        //{
        //    var projectsWithBrokenBuild =
        //        projects.Where(pi => pi.LatestBuild.Status == DomainModel.CI.BuildStatus.FinishedWithFailure);

        //    bool foundBrokenBuild = projectsWithBrokenBuild.Count() > 0;

        //    if (foundBrokenBuild && !isFrozen)
        //    {
        //        isFrozen = true;
        //        freezeViewCmdPublisher.Notify();
        //    }
        //    else if (!foundBrokenBuild && isFrozen)
        //    {
        //        Thread.Sleep(3000);
        //        isFrozen = false;
        //        unFreezeViewCmdPublisher.Notify();
        //    }
        //}

        private void LoadProjectInfoIntoView(CIProject CIProject)
        {
            ProjectInfoViewModel existingProjectInfoViewModel = TryFindProjectInView(CIProject.ProjectName);
            bool projectExistsInView = existingProjectInfoViewModel != null;

            if (projectExistsInView)
            {
                UpdateExistingProject(existingProjectInfoViewModel, CIProject);
            }
            else
            {
                AddNewProject(CIProject);
            }
        }

        private void AddNewProject(CIProject CIProject)
        {
            var model = new ProjectInfoViewModel();
            UpdateExistingProject(model, CIProject);
            ViewModel.Data.Add(model);
        }

        private void UpdateExistingProject(ProjectInfoViewModel model, CIProject CIProject)
        {
            BuildViewModel buildModel = model.LatestBuild;

            model.ProjectName = CIProject.ProjectName;
            model.IsSoundEnabled = isSoundEnabled;
            buildModel.StartTime = CIProject.LatestBuild.StartTime;
            buildModel.FinishedTime = CIProject.LatestBuild.FinishedTime;
            buildModel.Status = (BuildStatus) CIProject.LatestBuild.Status;
            SetBuildTrigger(CIProject.LatestBuild.Trigger, ref buildModel);
        }

        private ProjectInfoViewModel TryFindProjectInView(string projectName)
        {
            foreach (ProjectInfoViewModel exisitingProjects in ViewModel.Data)
            {
                if (exisitingProjects.ProjectName == projectName)
                {
                    return exisitingProjects;
                }
            }

            return null;
        }

        private void SetBuildTrigger(Trigger trigger, ref BuildViewModel buildViewModel)
        {
            buildViewModel.TriggeredBy = TryGetPerson(trigger.InvokedBy);
            buildViewModel.TriggerCause = trigger.Cause;
        }

        private Person TryGetPerson(string username)
        {
            Person returnPerson = new UnknownPerson();

            try
            {
                var userList = userRepository.Get(new UserByUsername(username));

                if (userList.Count() > 0)
                {
                    var user = userList.First();
                    returnPerson = new Person()
                                   {
                                       Username = username,
                                       Firstname = user.Firstname,
                                       Middlename = user.Middlename,
                                       Surname = user.Surname,
                                       Email = user.Email,
                                       ImageUrl = user.ImageUrl,
                                   };
                }
                else
                {
                    returnPerson = new Person()
                    {
                        Username = username,
                        ImageUrl = User.unknownUser.ImageUrl
                    };
                }
            }
            catch (Exception)
            {
                returnPerson = new Person()
                {
                    Username = "unknown"
                };

            }

            return returnPerson;
        }

    }
}