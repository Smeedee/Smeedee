using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;
using Smeedee.Tests;
using Smeedee.Widget.Admin.Users.ViewModels;
using TinyBDD.Specification.NUnit;
using TinyBDD.Dsl.GivenWhenThen;

namespace Smeedee.Widget.Admin.Users.Tests.ViewModels
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
                 () => { viewModel.Users.ShouldNotBeNull(); });
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
                 () => { viewModel.Users.Add(new UserViewModel()); });
            Then("the TeamMember collection in the viewModel should contain one entry",
                 () => { viewModel.Users.Count.ShouldBe(1); });
        }

        [Test]
        public void Assure_observer_is_notified_when_the_teamMember_collection_is_set()
        {
            Given(the_viewmodel_has_been_created);
            When("the collection of teamMembers is set",
                 () => { viewModel.Users = new ObservableCollection<UserViewModel>(); });
            Then("the observer should be notified",
                 () => { changeViewModelRecorder.ChangedProperties.Count.ShouldNotBe(0); });
        }

        [Test]
        public void Assure_observer_is_notified_when_a_user_is_added_to_the_teamMember_collection()
        {
            Given(the_viewmodel_has_been_created);
            When("a user is added to the collection of teamMembers", () => { viewModel.Users.Add(new UserViewModel()); });
            Then("the observer should be notified",
                 () => { changeTeamMemberRecorder.ChangedProperties.Count.ShouldNotBe(0); });
        }

        [Test]
        public void Assure_the_length_of_the_teamMember_collection_is_one_less_when_one_entry_is_removed()
        {
            Given(the_viewmodel_has_been_created)
                .And("the collection of teamMembers is set",
                     () => { viewModel.Users = new ObservableCollection<UserViewModel>(); })
                .And("the collection of teamMembers has one user in it",
                     () => { AddValidUser("Haldis"); }
                );
            When("the user is deleted from teamMembers", () =>
            {
                var userToRemove = new UserViewModel();
                userToRemove.Username = "Haldis";
                viewModel.Users.Remove(userToRemove);
            });
            Then("the length of the teamMembers collection should be 0",
                 () => { viewModel.Users.Count.ShouldBe(0); });
        }

        [Test]
        public void Assure_Delete_removes_selectedUser_from_users()
        {
            Given(the_viewmodel_has_been_created).
                And("a user new user is added and selected", () =>
                {
                    AddValidUser("TestUser");
                    var UserToDelete = new UserViewModel();
                    UserToDelete.Username = "TestUser";
                    viewModel.SelectedUser = UserToDelete;
                });
            When("the DeleteSelectedUser is run", () => { viewModel.DeleteSelectedUser.Execute(null); });
            Then("the selected user should be deleted", () => { viewModel.Users.Count.ShouldBe(0); });
        }

        [Test]
        public void Assure_OpenUserEditWindow_resets_userToAdd()
        {
            Given(the_viewmodel_has_been_created).And("a user has previously been edited", ()=>
                {
                    viewModel.EditedUser.Username = "frank";                                                                                  
                });
            When("new user window is created",
                 () => { viewModel.OpenUserEditWindow.Execute(null); });
            Then("edited user should be reset",
                 () =>
                 { viewModel.EditedUser.Username.ShouldBe(""); });
        }

        [Test]
        public void Assure_new_users_are_added()
        {
            Given(the_viewmodel_has_been_created).
                And("there is a new user to add",
                 () => { viewModel.EditedUser = CreateValidUser("Haldis"); });
            When("new user is added",
                 () => { viewModel.AddUser.Execute(null); });
            Then("new user should be added to TeamMembers collection",
                 () => { viewModel.Users.Count.ShouldBe(1); });
        }

        [Test]
        public void Should_only_add_new_users_if_they_have_userName()
        {
            Given(the_viewmodel_has_been_created);

            When("new User with no userName is added",
                 () =>
                 {
                     viewModel.EditedUser = new UserViewModel();
                     viewModel.AddUser.Execute(null);
                 });
            Then("User should not be added",
                 () => { viewModel.Users.Count.ShouldBe(0); });
        }

        [Test]
        public void Assure_only_adds_new_users_if_userName_is_unique_and_valid()
        {
            Given(the_viewmodel_has_been_created).
                And("there is a User which is unique and valid",
                 () =>
                 { viewModel.EditedUser = CreateValidUser("Haldis"); });
            When("new User with the same userName is added",
                 () =>
                 {
                     var newUser = new UserViewModel();
                     newUser.Username = "Haldis";
                     viewModel.AddUser.Execute(null);
                 });
            Then("User should not be added in TeamMembers",
                 () => { viewModel.Users.Count.ShouldBe(1); });
        }

        [Test]
        public void Assure_that_edited_user_is_copied_to_the_same_location_in_users_collection()
        {
            Given(the_viewmodel_has_been_created)
                .And(there_are_TeamMembers)
                .And(user_is_selected)
                .And(editing_is_initalized);

            When("user is edited and saved", () =>
            {
                viewModel.EditedUser.Username = "a";
                viewModel.AddUser.Execute(null);
            });

            Then("selected user should be the same as before", () =>
                viewModel.Users.First().Username.Equals("a").ShouldBeTrue());
        }

        [Test]
        public void Should_be_able_to_edit_user()
        {
            Given(the_viewmodel_has_been_created)
                .And(there_are_TeamMembers)
                .And(user_is_selected)
                .And(editing_is_initalized);

            When("user is edited and saved", () =>
            {
                viewModel.EditedUser.Username = "a";
                viewModel.AddUser.Execute(null);
            });

            Then("selected user should be edited", () =>
                viewModel.Users.Contains(new UserViewModel() { Username = "a" }));
        }

        [Test]
        public void Assure_edited_users_username_can_be_changed_to_and_from_same_username()
        {
            Given(the_viewmodel_has_been_created)
                .And(there_are_TeamMembers)
                .And(user_is_selected)
                .And(editing_is_initalized);
            When("username is set to another users username and then back again", () =>
            {
                viewModel.EditedUser.Username = "haldis2";
                viewModel.AddUser.Execute(null);
                viewModel.EditedUser.Username = "haldis1";
                viewModel.AddUser.Execute(null);
            });
            Then("user should not be invalid anymore", () =>
                viewModel.EditedUser.HasInvalidUsername.ShouldBeFalse());
        }

        [Test]
        public void Assure_user_cannot_be_edited_to_existing_users_username()
        {
            Given(the_viewmodel_has_been_created)
                .And(there_are_TeamMembers)
                .And(user_is_selected)
                .And(editing_is_initalized);

            When("username is set to another users username", () =>
            {
                viewModel.EditedUser.Username = "haldis2";
                viewModel.AddUser.Execute(null);
            });

            Then("selected user should not be updated", () =>
            {
                viewModel.Users.Contains(new UserViewModel() { Username = "haldis1" }).ShouldBeTrue();
                viewModel.Users.Contains(new UserViewModel() { Username = "haldis2" }).ShouldBeTrue();
            });
        }

        [Test]
        public void Assure_users_are_marked_if_they_are_invalid()
        {
            Given(the_viewmodel_has_been_created)
                .And(there_are_TeamMembers);

            When("new user is to be added", () =>
            {
                viewModel.OpenUserEditWindow.Execute(null);
                viewModel.EditedUser.Username = "haldis2";
                viewModel.AddUser.Execute(null);
            });

            Then("user should be marked as invalid", () =>
            {
                viewModel.EditedUser.HasInvalidUsername.ShouldBeTrue();
            });
        }
    }
    
    public class Shared : ScenarioClass
    {
        protected static UsersViewModel viewModel;
        protected static PropertyChangedRecorder changeViewModelRecorder;
        protected static PropertyChangedRecorder changeTeamMemberRecorder;

        protected Context the_viewmodel_has_been_created = () =>
        {
            viewModel = new UsersViewModel();
            changeViewModelRecorder = new PropertyChangedRecorder(viewModel);
            changeTeamMemberRecorder = new PropertyChangedRecorder(viewModel.Users);
        };

        protected Context there_are_TeamMembers = () =>
        {
            AddValidUser("haldis1");
            AddValidUser("haldis2");
        };

        protected Context user_is_selected = () => { viewModel.SelectedUser = viewModel.Users.First(); };
        protected Context editing_is_initalized = () => { viewModel.EditUser.Execute(null); };


        protected static void AddValidUser(string userName)
        {
            var newUser = CreateValidUser(userName);
            viewModel.Users.Add(newUser);
        }

        protected static UserViewModel CreateValidUser(string userName)
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
