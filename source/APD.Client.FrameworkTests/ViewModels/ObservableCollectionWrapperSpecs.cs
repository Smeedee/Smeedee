using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.Client.Framework.ViewModels;

using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using NUnit.Framework;

namespace APD.Client.Framework.Tests.ViewModels.ObservableCollectionWrapperSpecs
{
    public class CustomWrapper : ItemViewModelWrapper<CustomViewModel>
    {
        public string Name
        {
            set { InternalViewModel.Name = value; }
            get { return InternalViewModel.Name; }
        }

        public CustomWrapper(CustomViewModel viewModel)
            : base(viewModel)
        {
            
        }
    }

    public class CustomViewModel : AbstractViewModel
    {
        public string Name { get; set; }

        public CustomViewModel() :
            base(new NoUIInvocation())
        {
            
        }
    }

    public class Shared
    {
        protected static BindableViewModel<CustomViewModel> viewModel;
        protected static ObservableCollectionWrapper<CustomViewModel, CustomWrapper> Wrapper;

        protected Context viewModel_exist = () =>
        {
            viewModel = new BindableViewModel<CustomViewModel>(new NoUIInvocation());
        };

        protected Context wrapper_is_created = () =>
        {
            Wrapper = new ObservableCollectionWrapper<CustomViewModel, CustomWrapper>(viewModel.Data);
        };

        protected Context wrapper_contains_data = () =>
        {
            Wrapper.Add(new CustomWrapper(new CustomViewModel() { Name = "Gøran" }));
            Wrapper.Add(new CustomWrapper(new CustomViewModel() { Name = "Torstein"}));
        };

        protected Context viewModel_contains_data = () =>
        {
            viewModel.Data.Add(new CustomViewModel() { Name = "Gøran"});
            viewModel.Data.Add(new CustomViewModel() { Name = "Torstein"});
        };
    }

    [TestFixture]
    public class When_spawned : Shared
    {
        [Test]
        public void Assure_BindableViewModel_is_specified()
        {
            this.ShouldThrowException<ArgumentNullException>(() =>
                new ObservableCollectionWrapper<CustomViewModel, CustomWrapper>(null), exception =>
                    exception.Message.ShouldBe("Value cannot be null.\r\nParameter name: dataSource"));
        }

        [Test]
        public void Assure_items_in_ViewModel_is_reflected_to_the_wrapper()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(viewModel_exist).
                    And(viewModel_contains_data);
                scenario.When("Wrapper is created", () =>
                    Wrapper = new ObservableCollectionWrapper<CustomViewModel, CustomWrapper>(viewModel.Data));
                scenario.Then("assure items in ViewModel is reflected to Wrapper", () =>
                    Wrapper.ShouldHave(2));
            });
        }
    }

    [TestFixture]
    public class When_editing_ViewModel : Shared
    {
        [Test]
        public void Assure_added_item_is_reflected_to_wrapper()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(viewModel_exist).
                    And(wrapper_is_created);

                scenario.When("an item is added to the ViewModel", () =>
                    viewModel.Data.Add(new CustomViewModel() { Name = "Gøran Hansen"}));

                scenario.Then("assure added item is refleected to Wrapper", () =>
                {
                    Wrapper.ShouldHave(1);
                    Wrapper.First().Name.ShouldBe("Gøran Hansen");
                });
            });
        }

        [Test]
        public void Assure_removed_item_is_reflected_to_wrapper()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(viewModel_exist).
                    And(wrapper_is_created).
                    And(viewModel_contains_data);

                scenario.When("an item is removed from the ViewModel", () =>
                    viewModel.Data.RemoveAt(0));

                scenario.Then("assure removed item is reflected to Wrapper", () =>
                                                                              Wrapper.ShouldHave(1));
            });
        }
    }

    [TestFixture]
    public class When_Clearing_viewModel : Shared
    {
        [Test]
        public void Assure_wrapper_has_been_cleared()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(viewModel_exist).
                    And(wrapper_is_created).
                    And(viewModel_contains_data);
                
                scenario.When("ViewModel is cleared", () =>
                    viewModel.Data.Clear());
                scenario.Then("assure wrapper has been cleared", () =>
                    Wrapper.Count.ShouldBe(0));
            });   
        }
    }

    [TestFixture]
    public class When_editing_wrapper : Shared
    {
        [Test]
        public void Assure_added_item_is_mediated_to_ViewModel()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(viewModel_exist).
                    And(wrapper_is_created);
                scenario.When("item is added to Wrapper", () =>
                    Wrapper.Add(new CustomWrapper(new CustomViewModel())));
                scenario.Then("assure Wrapper adds the item to ViewModel", () =>
                    viewModel.Data.ShouldHave(1));
            });
        }

        [Test]
        public void Assure_removed_item_is_mediated_to_ViewModel()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(viewModel_exist).
                    And(wrapper_is_created).
                    And(wrapper_contains_data);
                scenario.When("item is removed from Wrapper", () =>
                    Wrapper.RemoveAt(0));
                scenario.Then("assure Wrapper removes the item from ViewModel", () =>
                {
                    viewModel.Data.ShouldHave(1);
                });
            });
        }
    }



    [TestFixture]
    public class When_editing_wrapper_items : Shared
    {
        [Test]
        public void Assure_change_in_item_is_mediated_to_ViewModel_item()
        {
            Scenario.StartNew(this, scenario =>
            {
                CustomViewModel itemViewModel = new CustomViewModel();
                CustomWrapper itemWrapper = new CustomWrapper(itemViewModel);

                scenario.Given(viewModel_exist).
                    And(wrapper_is_created).
                    And("an item is added to the Wrapper", () =>
                                                            Wrapper.Add(itemWrapper));

                scenario.When("Wrapper item is edited", () =>
                    itemWrapper.Name = "Gøran Hansen");

                scenario.Then("assure change in item is mediated to ViewModel item", () =>
                {
                    viewModel.Data.ShouldHave(1);
                    viewModel.Data.First().Name.ShouldBe("Gøran Hansen");
                });
            });
        }

    }

}
