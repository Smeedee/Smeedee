using System;

using APD.Tests;

using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace APD.Client.Framework.ViewModels.PersonSpecs
{
    public class Shared
    {
        protected static PropertyChangedRecorder changeRecorder;
        protected static Person viewModel;

        protected Context the_firstname_is_set = () => viewModel.Firstname = "First";
        protected Context the_middlename_is_set = () => viewModel.Middlename = "Middle";

        protected readonly Context user_has_no_first_name = () => { viewModel.Firstname = null; };
        protected readonly Context the_lastname_is_set = () => { viewModel.Surname = "Last"; };


        protected Context the_object_is_created = () =>
        {
            viewModel = new Person(new NoUIInvocation());
            changeRecorder = new PropertyChangedRecorder(viewModel);
            viewModel.Username = "username";
        };

        protected Context the_object_is_created_with_first_name = () =>
        {
            viewModel = new Person(new NoUIInvocation());
            viewModel.Firstname = "First";
            changeRecorder = new PropertyChangedRecorder(viewModel);
        };
    }

    [TestFixture]
    public class When_viewModel_is_spawned : Shared
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            the_object_is_created();
        }

        #endregion

        [Test]
        public void Should_have_a_Email_Property()
        {
            viewModel.Email.ShouldBeNull();
        }

        [Test]
        public void Should_have_a_Firstname_property()
        {
            viewModel.Firstname.ShouldBeNull();
        }

        [Test]
        public void Should_have_a_ImageUrl_Property()
        {
            PropertyTester.TestForExistence<Person>(p=>p.ImageUrl);
        }

        [Test]
        public void Should_have_a_Middlename_property()
        {
            viewModel.Middlename.ShouldBeNull();
        }

        [Test]
        public void Should_have_a_Name_property()
        {
            viewModel.Name.ShouldBe(viewModel.Username);
        }

        [Test]
        public void Should_have_a_Surname_property()
        {
            viewModel.Surname.ShouldBeNull();
        }

        [Test]
        public void Should_have_a_Username_property()
        {
            viewModel.Username.ShouldBe(viewModel.Username);
        }
    }

    [TestFixture]
    public class When_properties_change : Shared
    {
        [Test]
        public void Should_notify_oberservers_when_Firstname_property_changes()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_object_is_created);

                scenario.When("a value for the Firstname property is set", () =>
                                                                           viewModel.Firstname = "goeran");

                scenario.
                    Then("the observers should be notified about the change", () =>
                    {
                        changeRecorder.ChangedProperties.ShouldContain("Firstname");
                        changeRecorder.ChangedProperties.ShouldContain("Name");
                    })
                    .And("the firstname should be updated",
                         () => viewModel.Firstname.ShouldBe("goeran"))
                    .And("the name should be updated",
                         () => viewModel.Name.ShouldBe("goeran"));
            });
        }

        [Test]
        public void Should_notify_oberservers_when_Middlename_property_changes()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_object_is_created_with_first_name);

                scenario.When("a value for the Middlename property is set", () =>
                                                                            viewModel.Middlename = "Middle");

                scenario.
                    Then("the observers should be notified about the change", () =>
                    {
                        changeRecorder.ChangedProperties.ShouldContain("Middlename");
                        changeRecorder.ChangedProperties.ShouldContain("Name");
                    })
                    .And("the middlename should be updated",
                         () => viewModel.Middlename.ShouldBe("Middle"))
                    .And("the name should be updated",
                         () => viewModel.Name.ShouldBe("First Middle"));
            });
        }

        [Test]
        public void Should_notify_oberservers_when_Surname_property_changes()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_object_is_created_with_first_name)
                    .And(the_firstname_is_set);

                scenario.When("a value for the Surname property is set",
                              () => viewModel.Surname = "Last");

                scenario.
                    Then("the observers should be notified about the change", () =>
                    {
                        changeRecorder.ChangedProperties.ShouldContain("Surname");
                        changeRecorder.ChangedProperties.ShouldContain("Name");
                    })
                    .And("the surname should be updated",
                         () => viewModel.Surname.ShouldBe("Last"))
                    .And("the name should be updated",
                         () => viewModel.Name.ShouldBe("First Last"));
            });
        }

        [Test]
        public void Should_notify_observers_when_Email_changes()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_object_is_created);

                scenario.When("a value for the Email property is set", () =>
                                                                       viewModel.Email = "mail@goeran.no");

                scenario.
                    Then("the observers should be notified about the change",
                         () => changeRecorder.ChangedProperties.ShouldContain("Email"))
                    .And("the value should be updated", () =>
                                                        viewModel.Email.ShouldBe("mail@goeran.no"));
            });
        }

        [Test]
        public void Should_notify_observers_when_Username_changes()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_object_is_created);

                scenario.When("a value for the Username property is set", () =>
                                                                       viewModel.Username = "goeran");

                scenario.
                    Then("the observers should be notified about the change",
                         () => changeRecorder.ChangedProperties.ShouldContain("Username"))
                    .And("the value should be updated", () =>
                                                        viewModel.Username.ShouldBe("goeran"));
            });
        }

        [Test]
        public void Should_notify_observers_when_ImageUrl_changes()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_object_is_created);

                scenario.When("a value for the ImageUrl property is set", () =>
                                                                          viewModel.ImageUrl =
                                                                          "http://goeran.no/avatar.jpg");

                scenario.
                    Then("the observers should be notified about the change", () =>
                                                                              changeRecorder.ChangedProperties.
                                                                                  ShouldContain("ImageUrl")).
                    And("the value should be updated", () =>
                                                       viewModel.ImageUrl.ShouldBe(
                                                           "http://goeran.no/avatar.jpg"));
            });
        }
    }

    [TestFixture]
    public class when_user_has_only_first_name : Shared
    {
        [Test]
        public void assure_name_is_first_name()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario
                    .Given(the_object_is_created)
                    .And(the_firstname_is_set);

                scenario.When("accessing name property");

                scenario.Then("assure name equals first name",
                              () => viewModel.Name.ShouldBe("First"));
            });
        }
    }

    [TestFixture]
    public class when_user_has_firstname_and_surname : Shared
    {
        [Test]
        public void assure_name_is_firstname_and_surname()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario
                    .Given(the_object_is_created)
                    .And(the_firstname_is_set)
                    .And(the_lastname_is_set);

                scenario.When("accessing name property");

                scenario.Then("assure name equals first and surname",
                              () => viewModel.Name.ShouldBe("First Last"));
            });
        }
    }

    [TestFixture]
    public class when_user_has_firstname__and_middlename_and_surname : Shared
    {
        [Test]
        public void assure_name_is_firstname_and_middlename_and_surname()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario
                    .Given(the_object_is_created)
                    .And(the_firstname_is_set)
                    .And(the_middlename_is_set)
                    .And(the_lastname_is_set);

                scenario.When("accessing name property");

                scenario.Then("assure name equals first, middle and surname",
                              () => viewModel.Name.ShouldBe("First Middle Last"));
            });
        }
    }

    [TestFixture]
    public class when_user_has_only_username : Shared
    {
        [Test]
        public void assure_name_is_username()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario
                    .Given(the_object_is_created);

                scenario.When("accessing name property");

                scenario.Then("assure name equals username",
                              () => viewModel.Name.ShouldBe("username"));
            });
        }
    }

    [TestFixture]
    public class When_UnknownPerson_is_created
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            unknownPerson = new UnknownPerson(new NoUIInvocation());
        }

        #endregion

        protected Person unknownPerson;

        [Test]
        [ExpectedException(typeof (Exception))]
        public void Firstname_property_throws_exception_when_set()
        {
            unknownPerson.Firstname = "test";
        }

        [Test]
        public void Name_property_is_always_Unknown()
        {
            try
            {
                unknownPerson.Firstname = "test";
            }
            catch {}

            unknownPerson.Name.ShouldBe("Unknown");
        }

        [Test]
        public void Should_inherit_from_Person()
        {
            unknownPerson.ShouldBeInstanceOfType<Person>();
        }
    }

    [TestFixture]
    public class when_person_has_only_first_name : Shared
    {
        [Test]
        public void assure_name_is_first_name()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario
                    .Given(the_object_is_created)
                    .And(the_firstname_is_set);

                scenario.When("accessing name property");

                scenario.Then("assure name equals first name",
                              () => viewModel.Name.ShouldBe("First"));
            });
        }
    }

    [TestFixture]
    public class when_person_has_firstname_and_surname : Shared
    {
        [Test]
        public void assure_name_is_firstname_and_surname()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario
                    .Given(the_object_is_created)
                    .And(the_firstname_is_set)
                    .And(the_lastname_is_set);

                scenario.When("accessing name property");

                scenario.Then("assure name equals first and surname",
                              () => viewModel.Name.ShouldBe("First Last"));
            });
        }
    }


    [TestFixture]
    public class when_person_has_no_imageUrl_set : Shared
    {
        [Test]
        public void assure_default_image_is_returned()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario
                    .Given(the_object_is_created);

                scenario.When("accessing ImageUrl property");

                scenario.Then("assure the property returns a string",
                              () => viewModel.ImageUrl.ShouldNotBeNull());
            });
        }
    }

    [TestFixture]
    public class when_person_has_firstname__and_middlename_and_surname : Shared
    {
        [Test]
        public void assure_name_is_firstname_and_middlename_and_surname()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario
                    .Given(the_object_is_created)
                    .And(the_firstname_is_set)
                    .And(the_middlename_is_set)
                    .And(the_lastname_is_set);

                scenario.When("accessing name property");

                scenario.Then("assure name equals first, middle and surname",
                              () => viewModel.Name.ShouldBe("First Middle Last"));
            });
        }
    }
}