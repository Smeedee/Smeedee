using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using NUnit.Framework;
using Smeedee.Tests;
using Smeedee.Widgets.TeamMembers.ViewModels;
using TinyBDD.Specification.NUnit;
using TinyBDD.Dsl.GivenWhenThen;

namespace Smeedee.Widgets.Tests.TeamMembers.ViewModels
{
    [TestFixture]
    public class When_spawned : Shared
    {
        [Test]
        public void Assure_viewmodel_has_collection_of_teamMembers()
        {
            Given(the_viewmodel_has_been_created);
            When("");
            Then("the collection of team members should have been created",
                 () => { viewModel.TeamMembers.ShouldNotBeNull(); });
        }
    }
    
    [TestFixture]
    public class When_viewModel_data_is_changed : Shared
    {
        [Test]
        public void Assure_the_teamMember_collection_contains_one_entry_when_one_entry_is_added()
        {
            Given(the_viewmodel_has_been_created);
            When("one user is added to the TeamMember collection in the viewModel",
                 () => { viewModel.TeamMembers.Add(new UserViewModel()); });
            Then("the TeamMember collection in the viewModel should contain one entry",
                 () => { viewModel.TeamMembers.Count.ShouldBe(1); });
        }

        [Test]
        public void Assure_observer_is_notified_when_the_teamMember_collection_is_set()
        {
            Given(the_viewmodel_has_been_created);
            When("the collection of teamMembers is set",
                 () => { viewModel.TeamMembers = new ObservableCollection<UserViewModel>(); });
            Then("the observer should be notified",
                 () => { changeViewModelRecorder.ChangedProperties.Count.ShouldNotBe(0); });
        }

        [Test]
        public void Assure_observer_is_notified_when_a_user_is_added_to_the_teamMember_collection()
        {
            Given(the_viewmodel_has_been_created);
            When("a user is added to the collection of teamMembers", () => { viewModel.TeamMembers.Add(new UserViewModel()); });
            Then("the observer should be notified",
                 () => { changeTeamMemberRecorder.ChangedProperties.Count.ShouldNotBe(0); });
        }

        [Test]
        public void Assure_the_length_of_the_teamMember_collection_is_one_less_when_one_entry_is_removed()
        {
            Given(the_viewmodel_has_been_created)
                .And("the collection of teamMembers is set",
                     () => { viewModel.TeamMembers = new ObservableCollection<UserViewModel>(); })
                .And("the collection of teamMembers has one user in it",
                     () => { AddValidUserToTeamMembers("Haldis"); }
                );
            When("the user is deleted from teamMembers", () =>
                {
                    var userToRemove = new UserViewModel();
                    userToRemove.Username = "Haldis";
                    viewModel.TeamMembers.Remove(userToRemove);
                });
            Then("the length of the teamMembers collection should be 0",
                 () => { viewModel.TeamMembers.Count.ShouldBe(0); });
        }

        [Test]
        public void Should_update_visibility_on_UserViewModels_when_boxes_are_checked()
        {
            Given(the_viewmodel_has_been_created).
                And("there are theamMembers in viewModel", ()=>
                {
                    AddValidUserToTeamMembers("haldis");                                                                             
                });
            When("boxes are checked", ()=>
            {
                viewModel.UsernameIsChecked = true;
                viewModel.SurnameIsChecked = true;
                viewModel.FirstnameIsChecked = true;
                viewModel.MiddlenameIsChecked = true;
            });
            Then("teamMembers should be updated", () =>
            {
                var user = viewModel.TeamMembers.First();
                user.MiddlenameIsVisible.ShouldBe(Visibility.Visible);
                user.UsernameIsVisible.ShouldBe(Visibility.Visible);
                user.SurnameIsVisible.ShouldBe(Visibility.Visible);
                user.FirstnameIsVisible.ShouldBe(Visibility.Visible);
            });
        }
    }

    public class Shared : ScenarioClass
    {
        protected static TeamMembersViewModel viewModel;
        protected static PropertyChangedRecorder changeViewModelRecorder;
        protected static PropertyChangedRecorder changeTeamMemberRecorder;

        protected Context the_viewmodel_has_been_created = () =>
        {
            viewModel = new TeamMembersViewModel();
            changeViewModelRecorder = new PropertyChangedRecorder(viewModel);
            changeTeamMemberRecorder = new PropertyChangedRecorder(viewModel.TeamMembers);
        };

        protected Context there_are_TeamMembers = () =>
        {
            AddValidUserToTeamMembers("haldis1");
            AddValidUserToTeamMembers("haldis2");
        };

        protected static void AddValidUserToTeamMembers(string userName)
        {
            var newUser = new UserViewModel
            {
                Username = userName,
                Firstname = "Haldis",
                ImageUrl = "http://haldis.com"
            };
            
            viewModel.TeamMembers.Add(newUser);
        }

        protected static UserViewModel CreateValidUserToTeamMembers(string userName)
        {
            var validUser = new UserViewModel
            {
                Username = userName,
                Firstname = "Haldis",
                ImageUrl = "http://haldis.com"
            };
            return validUser;
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