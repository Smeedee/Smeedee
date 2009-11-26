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
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using APD.Client.Framework;
using APD.Tests;
using NUnit.Framework;
using TinyBDD.Specification.NUnit;
using TinyBDD.Dsl.GivenWhenThen;
using APD.Client.Widget.SourceControl.ViewModels;


namespace APD.Client.Widget.SourceControlTests.ViewModels.CheckInNotificationSpecs
{
    public class Shared
    {
        protected static CheckInNotificationViewModel viewModel;
        protected static PropertyChangedRecorder changeRecorder;

        protected Context the_object_is_created = () =>
        {
            viewModel = new CheckInNotificationViewModel(new NoUIInvocation());
            changeRecorder = new PropertyChangedRecorder(viewModel);
        };
    }

    [TestFixture]
    public class When_ViewModel_is_spawned : Shared
    {
        [SetUp]
        public void Setup()
        {
            the_object_is_created();
        }

        [Test]
        public void Should_have_a_NoChangesets_property()
        {
            viewModel.NoChangesets.ShouldBe(false);
        }

        [Test]
        public void Should_have_a_Loading_property()
        {
            viewModel.IsLoading.ShouldBeFalse();
        }

        [Test]
        public void Should_have_Changesets_Property()
        {
            viewModel.Changesets.ShouldNotBeNull();
            viewModel.Changesets.Count.ShouldBe(0);
        }
        
    }

    [TestFixture]
    public class When_properties_change : Shared
    {
        [Test]
        public void Should_notify_observers_when_NoChangesets_changes()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_object_is_created);

                scenario.When("NoChangesets property changes", () =>
                    viewModel.NoChangesets = true);

                scenario.
                    Then("the observers should be notified about the change", () =>
                        changeRecorder.ChangedProperties.ShouldContain("NoChangesets")).
                    And("the value should be updated", () =>
                        viewModel.NoChangesets.ShouldBeTrue());
            });
        }

        [Test]
        public void Should_notify_observers_when_Loading_changes()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_object_is_created);

                scenario.When("IsLoading property changes", () =>
                    viewModel.IsLoading = true);

                scenario.
                    Then("the observers should be notified about the change", () =>
                        changeRecorder.ChangedProperties.ShouldContain("IsLoading")).
                    And("the value should be updated", () =>
                        viewModel.IsLoading.ShouldBeTrue());

            });
        }
    }    
}
