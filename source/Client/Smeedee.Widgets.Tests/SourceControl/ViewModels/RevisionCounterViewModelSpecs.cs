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
    public class Shared : ScenarioClass
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
            Given(object_is_created_with_topCommitersViewModel);
            When("always");
            Then("topCommitersViewModel should not be null", 
                  () => revisionCounterViewModel.topCommitersViewModel.ShouldNotBeNull());
        }

        [Test]
        public void should_have_default_value()
        {
            Given(object_is_created_without_topCommitersViewModel);
            When("revision count is requested");
            Then("the revision count should be zero", 
                () => revisionCounterViewModel.RevisionCount.ShouldBe(0));
        }
    }

    [TestFixture]
    public class When_notfied : Shared
    {
        [Test]
        public void should_reflect_changes_in_the_developers_collection()
        {
            Given(object_is_created_with_topCommitersViewModel);
            When("a developer is added to the topCommutersViewModel");
            Then("the revision count should reflect the changes", () =>
            {
                var codeCommiterViewModel = new CodeCommiterViewModel();
                codeCommiterViewModel.NumberOfCommits = 10;
                topCommitersViewModel.Developers.Add(codeCommiterViewModel);

                revisionCounterViewModel.RevisionCount.ShouldBe(10);
            });
        }

        [Test]
        public void should_reflect_changes_in_a_developers_number_of_commits()
        {
            Given(object_is_created_with_topCommitersViewModel);
            When("a developers number of comits changes");
            Then("the revision count should reflect the changes", () =>
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
        }
        
    }
}
