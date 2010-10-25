using System;
using System.Windows;
using NUnit.Framework;
using Smeedee.Widgets.TeamMembers.ViewModels;
using TinyBDD.Specification.NUnit;
using TinyBDD.Dsl.GivenWhenThen;

namespace Smeedee.Widgets.Tests.TeamMembers.ViewModels
{
    [TestFixture]
    public class UserViewModelSpecs : ScenarioClass
    {
        public static UserViewModel viewModel;

        private Context ViewModel_is_created = () =>
        {
            viewModel = new UserViewModel
            {
                Username = "haldis",
                Surname = "haldis",
                ImageUrl = "haldi.jpg",
                Firstname = "hal",
                Middlename = "dis",
                Email = "haldis@foxylady.com",
                FirstnameIsVisible = Visibility.Visible,
                MiddlenameIsVisible = Visibility.Visible,
                SurnameIsVisible = Visibility.Visible,
                UsernameIsVisible = Visibility.Visible
            };
        };

        [Test]
        public void Should_have_these_properties()
        {
            Given(ViewModel_is_created);
            When("all properties are set");
            Then("all properites should not be null", () =>
            {
                viewModel.Username.ShouldNotBeNull(); 
                viewModel.Surname.ShouldNotBeNull();
                viewModel.ImageUrl.ShouldNotBeNull();
                viewModel.Firstname.ShouldNotBeNull();
                viewModel.Middlename.ShouldNotBeNull();
                viewModel.Email.ShouldNotBeNull();
                viewModel.FirstnameIsVisible.ShouldNotBeNull();
                viewModel.MiddlenameIsVisible.ShouldNotBeNull();
                viewModel.SurnameIsVisible.ShouldNotBeNull();
                viewModel.UsernameIsVisible.ShouldNotBeNull(); 
            });
        }

        [Test]
        public void Should_have_default_imageUrl_if_no_imageUrl_is_set()
        {
            Given(ViewModel_is_created);
            When("imageUrl is not set", ()=>
            {
                viewModel.ImageUrl = null;
                viewModel.DefaultPictureUri = "www.haldis.com";
            });
            Then("getting ImageUrl should return default image", () =>
            {
                viewModel.ImageUrl.ShouldNotBeNull();
            });
        }

        [Test]
        public void Should_be_able_to_compare_UserViewModels_by_userName()
        {
            Given(ViewModel_is_created);
            When("to ViewModels are compared");
            Then("ViewModels should only be equal if userNames are equal", () =>
            {
                var otherUserViewModel = new UserViewModel {Username = "haldis"};
                viewModel.Equals(otherUserViewModel).ShouldBeTrue();
                otherUserViewModel.Username = "frank";
                viewModel.Equals(otherUserViewModel).ShouldBeFalse();
                viewModel.Equals(null).ShouldBeFalse();
            });
        }

        [SetUp]
        public void SetUp()
        {
            Scenario("");
        }

        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }
    }
}