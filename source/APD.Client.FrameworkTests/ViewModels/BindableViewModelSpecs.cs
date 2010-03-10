using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using APD.Client.Framework;
using APD.Client.Framework.ViewModels;
using APD.Tests;

using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace APD.Client.Framework.Tests.ViewModels
{
    public class Shared
    {
        protected static BindableViewModel<int> viewModel;

        protected Context the_viewmodel_is_created = () =>
        {

            viewModel = new BindableViewModel<int>(new NoUIInvocation());
        };
    }

    [TestFixture]
    public class when_spawned :Shared
    {


        [Test]
        public void should_contain_Data_ObservableCollection_of_correct_type(){
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_viewmodel_is_created);
                scenario.When("Data property is accessed");
                scenario.Then("an ObservableCollection of correct type is returned", () => 
                    viewModel.Data.ShouldBeInstanceOfType<ObservableCollection<int>>());
            });
        }

        [Test]
        public void assure_Data_property_is_empty()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_viewmodel_is_created);
                scenario.When("Data property is accessed");
                scenario.Then("there are no elements in Data", () => 
                    viewModel.Data.Count.ShouldBe(0));
            });
        }

        [Test]
        public void assure_contain_SinceDate_property()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_viewmodel_is_created);
                scenario.When("SinceDate property is accessed");
                scenario.Then("Should return a datetime object", ()=>
                {
                    viewModel.SinceDate.ShouldNotBeNull();
                });
            });
        }
    }

    [TestFixture]
    public class on_changes : Shared
    {
        [Test]
        public void should_be_able_to_add_projects()
        {
            Scenario.StartNew(this, scenario =>
            {
                int initialCount=0;

                scenario.Given(the_viewmodel_is_created).
                    And("the number of elements is recorded", () =>
                    initialCount = viewModel.Data.Count);

                scenario.When("an element is added", ()=>
                    viewModel.Data.Add(42));

                scenario.Then("the number of elements increase by one", () =>
                    viewModel.Data.Count.ShouldBe(initialCount + 1));
            });

        }
    }
}
