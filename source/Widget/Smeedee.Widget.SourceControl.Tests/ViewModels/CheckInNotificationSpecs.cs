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

using System;
using System.Collections.ObjectModel;
using System.Linq;
using Moq;
using Smeedee.Client.Framework;
using Smeedee.Client.Framework.Tests;
using Smeedee.Tests;
using NUnit.Framework;
using Smeedee.Widget.SourceControl.ViewModels;
using TinyBDD.Specification.NUnit;
using TinyBDD.Dsl.GivenWhenThen;


namespace Smeedee.Client.Widget.SourceControlTests.ViewModels.CheckInNotificationSpecs
{
    public class Shared : ScenarioClass
    {
        protected static LatestCommitsViewModel viewModel;
        protected static PropertyChangedRecorder changeRecorder;
        protected static ChangesetViewModel changeViewModel;

        protected Context the_object_is_created = () =>
                    {
                        ViewModelBootstrapperForTests.Initialize();
                        viewModel = new LatestCommitsViewModel(new Client.Framework.ViewModel.Widget());
                        changeRecorder = new PropertyChangedRecorder(viewModel);
                        changeViewModel = new ChangesetViewModel();
                    };

        protected When numberOfCommits_field_in_view_is_set_to_5 = () => viewModel.NumberOfCommits = 5;

        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }
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

        [Test]
        public void Should_have_KeywordList_Property()
        {
            viewModel.KeywordList.ShouldNotBeNull();
            viewModel.KeywordList.Count().ShouldBe(0);
            (viewModel.KeywordList is ObservableCollection<KeywordColorPairViewModel>).ShouldBeTrue();
        }

    }

    [TestFixture]
    public class When_properties_change : Shared
    {
        [Test]
        public void Should_notify_observers_when_Loading_changes()
        {
            Given(the_object_is_created);
            When("IsLoading property changes", () => viewModel.IsLoading = true);
            Then("the observers should be notified about the change", () =>
                changeRecorder.ChangedProperties.Any(p => p == "IsLoading").ShouldBeTrue());
            And("the value should be updated", () => 
                viewModel.IsLoading.ShouldBeTrue());
        }
    }

    [TestFixture]
    public class When_Settings_change : Shared

    {
        [Test]
        public void assure_number_of_commits_is_changed_when_the_field_in_the_viewModel_is_set()
        {
            Given(the_object_is_created);
            When(numberOfCommits_field_in_view_is_set_to_5);
            Then(() => changeRecorder.ChangedProperties.Count.ShouldBe(1));
        }

    }
}
