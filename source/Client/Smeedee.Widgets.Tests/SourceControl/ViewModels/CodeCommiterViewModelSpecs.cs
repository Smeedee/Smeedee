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
// The project webpage is located at http://smeedee.org/
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System.Linq;
using NUnit.Framework;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Widget.SourceControl.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using Smeedee.Tests;


namespace Smeedee.Client.Widget.SourceControlTests.ViewModels.CodeCommiterViewModelSpecs
{
    public class Shared : ScenarioClass
    {
        protected static CodeCommiterViewModel viewModel;
        protected static PropertyChangedRecorder changeRecorder;

        protected Context the_ViewModel_is_created = () =>
        {
            viewModel = new CodeCommiterViewModel();
            changeRecorder = new PropertyChangedRecorder(viewModel);
        };
    }

    [TestFixture]
    public class When_ViewModel_is_spawned : Shared
    {
        [SetUp]
        public void Setup()
        {
            viewModel = new CodeCommiterViewModel();
        }

        [Test]
        public void Assure_ViewModel_is_a_PersonViewModel()
        {
            (viewModel is Person).ShouldBeTrue();
        }
    }

    [TestFixture]
    public class When_properties_change : Shared
    {
        [Test]
        public void Assure_observers_are_notified_when_NumberOfCommits_changes()
        {
            Given(the_ViewModel_is_created);

            When("the NumberOfCommits property is changed", () =>
                viewModel.NumberOfCommits = 10);

            Then("assure observers are notified", () =>
                changeRecorder.ChangedProperties.Any(p => p == "NumberOfCommits").ShouldBeTrue()).
            And("assure the value is changed", () =>
                viewModel.NumberOfCommits.ShouldBe(10));
        }

    }

}
