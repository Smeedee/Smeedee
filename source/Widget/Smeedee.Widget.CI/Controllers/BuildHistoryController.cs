using System;
using System.Collections.ObjectModel;
using System.Linq;
using Smeedee.Client.Framework.Controller;
using Smeedee.Client.Framework.Services;
using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Framework;
using Smeedee.Widget.CI.ViewModels;
using TinyMVVM.Framework;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widget.CI.Controllers
{
    public class BuildHistoryController : ControllerBase<BuildHistoryViewModel>
    {
        private IRepository<CIServer> ciRepository;

        public BuildHistoryController(BuildHistoryViewModel viewModel, ITimer timer, IUIInvoker uiInvoke, IRepository<CIServer> ciRepo)
            : base(viewModel, timer, uiInvoke)
        {
            this.ciRepository = ciRepo;
        }

        protected override void OnNotifiedToRefresh(object sender, EventArgs e)
        {
            var ciServers = ciRepository.Get(new AllSpecification<CIServer>());

            var allProjects = from s in ciServers
                              from p in s.Projects
                              select new ProjectBuildHistoryViewModel()
                              {
                                  ProjectName = p.ProjectName,
                                  FailedBuildsByDate = (from b in p.Builds
                                                        where b.Status == Smeedee.DomainModel.CI.BuildStatus.FinishedWithFailure
                                                        group b by b.FinishedTime into g
                                                        select new { Date = g.Key, Count = g.Count() }
                                                       ).ToDictionary(g => g.Date, g => g.Count),

                                   SuccessfullBuildsByDate = (from b in p.Builds
                                                       where b.Status == Smeedee.DomainModel.CI.BuildStatus.FinishedSuccefully
                                                       group b by b.FinishedTime into g
                                                       select new { Date = g.Key, Count = g.Count() }
                                                       ).ToDictionary(g => g.Date, g => g.Count)
                              };
                
                
            ViewModel.Data = new ObservableCollection<ProjectBuildHistoryViewModel>(allProjects);
        }
    }
}
