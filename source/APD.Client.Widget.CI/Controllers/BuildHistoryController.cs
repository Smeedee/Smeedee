using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using APD.Client.Framework;
using APD.Client.Framework.Controllers;
using APD.Client.Widget.CI.ViewModels;
using APD.DomainModel.CI;
using APD.DomainModel.Framework;


namespace APD.Client.Widget.CI.Controllers
{
    public class BuildHistoryController : ControllerBase<BuildHistoryViewModel>
    {
        private IRepository<CIServer> ciRepository;

        public BuildHistoryController(
            INotifyWhenToRefresh refreshNotifier, 
            IInvokeUI uiInvoker,
            IRepository<CIServer> ciRepository )
            : base(refreshNotifier, uiInvoker)
        {
            this.ciRepository = ciRepository;
        }

        protected override void OnNotifiedToRefresh(object sender, RefreshEventArgs e)
        {
            var ciServers = ciRepository.Get(new AllSpecification<CIServer>());

            var allProjects = from s in ciServers
                              from p in s.Projects
                              select new ProjectBuildHistoryViewModel(uiInvoker)
                              {
                                  ProjectName = p.ProjectName,
                                  FailedBuildsByDate = (from b in p.Builds
                                                        where b.Status == DomainModel.CI.BuildStatus.FinishedWithFailure
                                                        group b by b.FinishedTime into g
                                                        select new { Date = g.Key, Count = g.Count() }
                                                       ).ToDictionary(g => g.Date, g => g.Count),

                                   SuccessfullBuildsByDate = (from b in p.Builds
                                                       where b.Status == DomainModel.CI.BuildStatus.FinishedSuccefully
                                                       group b by b.FinishedTime into g
                                                       select new { Date = g.Key, Count = g.Count() }
                                                       ).ToDictionary(g => g.Date, g => g.Count)
                              };
                
                
            ViewModel.Data = new ObservableCollection<ProjectBuildHistoryViewModel>(allProjects);
        }
    }
}
