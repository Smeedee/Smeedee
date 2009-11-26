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

using APD.Client.Framework;
using APD.Client.Widget.CI.ViewModels;
using APD.DomainModel.CI;
using APD.Tests;

using NUnit.Framework;

using TinyBDD.Specification.NUnit;
//TODO: use tinyBDD if class gets any bigger

namespace APD.Client.Widget.CI.Tests.ProjectInfoViewModelSpecs
{
    public class Shared
    {
        protected ProjectInfoViewModel viewModel;
        public Shared()
        {
            viewModel = new ProjectInfoViewModel(new NoUIInvocation());
        }
    }

    [TestFixture]
    public class when_spawned : Shared
    {
        [Test]
        public void should_have_IsLoading_property()
        {
            viewModel.IsLoading.ShouldNotBeNull();
        }

        [Test]
        public void should_have_ProjectName_property()
        {
            viewModel.ProjectName.ShouldBeNull();
        }

        [Test]
        public void should_have_latest_build_view_model_property()
        {
            viewModel.LatestBuild.ShouldNotBeNull();
        }

        [Test]
        public void should_have_IsSoundEnabled_property()
        {
            viewModel.IsSoundEnabled.ShouldNotBeNull();
        }
    }

    [TestFixture]
    public class when_properties_change : Shared
    {
        [Test]
        public void assure_listeners_are_notified_when_latest_build_changes()
        {
            PropertyTester.TestChange<ProjectInfoViewModel>(
                viewModel, vm => vm.LatestBuild, new BuildViewModel(new NoUIInvocation()));
            Assert.IsTrue(PropertyTester.WasNotified);
        }

        [Test]
        public void assure_listeners_are_notified_when_name_changes()
        {
            PropertyTester.TestChange<ProjectInfoViewModel>(
                viewModel, vm => vm.ProjectName);
            Assert.IsTrue(PropertyTester.WasNotified);
        }
    }
}
