using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Smeedee.Widget.SourceControl.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace Smeedee.Client.Widget.SourceControlTests.ViewModels.RevisionCounterViewModelSpecs
{
    public class Shared
    {
        static protected RevisionCounterViewModel revisionCounterViewModel;
        static protected TopCommitersViewModel topCommitersViewModel;

        protected Context object_is_created_without_topCommitersViewModel = () =>
        {
            revisionCounterViewModel = new RevisionCounterViewModel();
        };

        protected Context object_is_created_with_topCommitersViewModel = () =>
        {
            topCommitersViewModel = new TopCommitersViewModel();
            revisionCounterViewModel = new RevisionCounterViewModel(topCommitersViewModel);
        };
    }

    [TestFixture]
    public class When_spawned : Shared
    {
        [Test]
        public void should_have_a_topComittersViewModel_reference()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(object_is_created_with_topCommitersViewModel);
                scenario.When("always");
                scenario.Then("topCommitersViewModel should not be null", () =>
                {
                    revisionCounterViewModel.topCommitersViewModel.ShouldNotBeNull();
                });
            });
        }

        [Test]
        public void should_have_default_value()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(object_is_created_without_topCommitersViewModel);
                scenario.When("revision count is requested");
                scenario.Then("the revision count should be zero", () =>
                {
                    revisionCounterViewModel.RevisionCount.ShouldBe(0);
                });
            });
        }
    }

    [TestFixture]
    public class When_notfied : Shared
    {
        [Test]
        public void should_reflect_changes_in_the_developers_collection()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(object_is_created_with_topCommitersViewModel);
                scenario.When("a developer is added to the topCommutersViewModel");
                scenario.Then("the revision count should reflect the changes", () =>
                {
                    var codeCommiterViewModel = new CodeCommiterViewModel();
                    codeCommiterViewModel.NumberOfCommits = 10;
                    topCommitersViewModel.Developers.Add(codeCommiterViewModel);

                    revisionCounterViewModel.RevisionCount.ShouldBe(10);
                });
            });
        }

        [Test]
        public void should_reflect_changes_in_a_developers_number_of_commits()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(object_is_created_with_topCommitersViewModel);
                scenario.When("a developers number of comits changes");
                scenario.Then("the revision count should reflect the changes", () =>
                {
                    var codeCommiterViewModel1 = new CodeCommiterViewModel();
                    var codeCommiterViewModel2 = new CodeCommiterViewModel();
                    codeCommiterViewModel1.NumberOfCommits = 10;
                    codeCommiterViewModel2.NumberOfCommits = 5;
                    topCommitersViewModel.Developers.Add(codeCommiterViewModel1);
                    topCommitersViewModel.Developers.Add(codeCommiterViewModel2);

                    revisionCounterViewModel.RevisionCount.ShouldBe(15);

                    codeCommiterViewModel1.NumberOfCommits = 1;
                    codeCommiterViewModel2.NumberOfCommits = 7;

                    revisionCounterViewModel.RevisionCount.ShouldBe(8);
                });
            });
        }
        
    }
}
