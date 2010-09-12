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

//TODO: use tinyBDD if class gets any bigger

using System;
using NUnit.Framework;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.Client.Framework.Tests;
using Smeedee.Client.Tests;
using Smeedee.Tests;
using Smeedee.Widget.CI.ViewModels;
using TinyBDD.Specification.NUnit;
using TinyMVVM.Framework;
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework.Services.Impl;

namespace Smeedee.Client.Widget.CI.Tests.ProjectInfoViewModelSpecs
{
    public class Shared
    {
        protected ProjectInfoViewModel viewModel;
        public Shared()
        {
        }
    }

    [TestFixture]
    public class when_spawned : Shared
    {
        [SetUp]
        public void SetUp()
        {
            viewModel = new ProjectInfoViewModel();
        }

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
        [SetUp]
        public void SetUp()
        {
            viewModel = new ProjectInfoViewModel();
        }

        [Test]
        public void assure_listeners_are_notified_when_latest_build_changes()
        {
            PropertyTester.TestChange<ProjectInfoViewModel>(
                viewModel, vm => vm.LatestBuild, new BuildViewModel());
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
