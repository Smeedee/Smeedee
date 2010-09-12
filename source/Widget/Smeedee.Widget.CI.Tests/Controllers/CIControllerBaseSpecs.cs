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
// The project webpage is located at http://www.smeedee.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.Users;
using Moq;
using NUnit.Framework;
using Smeedee.Widget.CI.Controllers;
using Smeedee.Widget.CI.Tests.Controllers;
using Smeedee.Widget.CI.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using Smeedee.DomainModel.Framework;
using TinyMVVM.Framework.Services;


namespace Smeedee.Client.Widget.CI.Tests.CIControllerSpecs
{
    public class TestCIController : CIControllerBase<TestCIViewModel>
    {
        public TestCIController(TestCIViewModel viewModel, IRepository<CIServer> ciProjectRepository, IRepository<User> userRepository, IInvokeBackgroundWorker<IEnumerable<CIProject>> asyncClient, ITimer timer, IUIInvoker uiInvoke, ILog logger, IProgressbar progressbar)
            : base(viewModel, ciProjectRepository, userRepository, asyncClient, timer, uiInvoke, logger, progressbar)
        {
            
        }

        protected override void LoadDataIntoViewModel(IEnumerable<CIProject> projects)
        {
        }
    }

    public class TestCIViewModel : AbstractViewModel
    {
        public TestCIViewModel()
        {
            
        }
    }

    public class CIControllerBaseSpecsShared : Shared
    {
        protected Context controller_is_created = CreateController;

        protected When controller_is_spawned = CreateController;

        private static TestCIController Controller;

        private static void CreateController()
        {
            var viewModel = new TestCIViewModel();
            Controller = new TestCIController(viewModel, RepositoryMock.Object,
                                          UserRepoMock.Object,
                                          new NoBackgroundWorkerInvocation<IEnumerable<CIProject>>(),
                                          new Mock<ITimer>().Object,
                                          new NoUIInvokation(),
                                          new Mock<ILog>().Object,
                                          progressbarMock.Object);
        }
    }


    [TestFixture]
    public class When_spawned : CIControllerBaseSpecsShared
    {
        [Test]
        public void should_query_for_projects()
        {
                Given(there_are_active_projects_in_CI_tool)
                    .And(user_exist_in_userdb);

                When(controller_is_spawned);

                Then("assure it query for all projects latest build", 
                    () => RepositoryMock.Verify(c => c.Get(It.IsAny<AllSpecification<CIServer>>())));
        }
    }
}