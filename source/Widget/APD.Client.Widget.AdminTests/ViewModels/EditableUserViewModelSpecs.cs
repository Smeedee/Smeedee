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
// <author>Your name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System;
using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using APD.Client.Widget.Admin.ViewModels;
using APD.Client.Framework;
using System.ComponentModel.DataAnnotations;
using APD.DomainModel.Users;


namespace APD.Client.Widget.AdminTests.ViewModels.EditableUserViewModelSpecs
{
    public class Shared
    {
        protected static UserViewModel viewModel;
        protected static EditableUserViewModel wrapper;

        protected Context viewModel_exist = () =>
        {
            viewModel = new UserViewModel(new NoUIInvocation());
        };

        protected Context wrapper_is_created = () =>
        {
            wrapper = new EditableUserViewModel(viewModel);
        };
    }

    [TestFixture]
    public class When_spawining : Shared
    {
        [Test]
        public void Assure_UserViewModel_is_specified()
        {
            this.ShouldThrowException<ArgumentNullException>(() =>
                                                         new EditableUserViewModel(null), exception =>
                                                                                          exception.Message.
                                                                                              ShouldBe(
                                                                                              "Value cannot be null.\r\nParameter name: viewModel"));
        }
    }

    [TestFixture]
    public class When_edit : Shared
    {
        [Test]
        public void Assure_Username_is_set_on_ViewModel()
        {
            Scenario.StartNew(this, scenario =>
            {
                Given_ViewModel_is_created(scenario);
                scenario.When("Username is changed on wrapper", () =>
                    wrapper.Username = "goeran");
                scenario.Then("assure Username is changed on ViewModel", () =>
                    viewModel.Username.ShouldBe(wrapper.Username));
            });
        }

        private GivenSemantics Given_ViewModel_is_created(Semantics scenario)
        {
            return scenario.Given(viewModel_exist).
                And(wrapper_is_created);

        }

        [Test]
        public void Assure_Username_is_propegated_to_Email()
        {
            Scenario.StartNew(this, scenario =>
            {
                Given_ViewModel_is_created(scenario);
 
                scenario.When("Username is empty", () =>
                    wrapper.Username = string.Empty);
                scenario.Then("assure nothing is propegated to Email", () =>
                    wrapper.Email.ShouldBe(string.Empty));

                scenario.When("Username contains @", () =>
                    wrapper.Username = "mail@goeran.no");
                scenario.Then("assure Username content is propagated to Email", () =>
                    wrapper.Email.ShouldBe("mail@goeran.no"));

                scenario.When("Username contains chars", () =>
                    wrapper.Username = "goeran");
                scenario.Then("assure Username content is propagated to Email and prefixed with @", () =>
                    wrapper.Email.ShouldBe("goeran@"));
            });
        }

        [Test]
        public void Assure_Username_is_not_propagated_to_Email_when_Email_is_specified()
        {
            Scenario.StartNew(this, scenario =>
            {
                Given_ViewModel_is_created(scenario).
                    And("E-mail is specified", () =>
                        viewModel.Email = "mail@goeran.no");

                scenario.When("Username is specified", () =>
                    wrapper.Username = "goeran");

                scenario.Then("Assure username is not propagated to Email", () =>
                    wrapper.Email.ShouldBe("mail@goeran.no"));
            });
        }

        [Test]
        public void Assure_Firstname_is_set_on_ViewModel()
        {
            Scenario.StartNew(this, scenario =>
            {
                Given_ViewModel_is_created(scenario);
                scenario.When("Firstname is changed on wrapper", () =>
                    wrapper.Firstname = "Gøran Hansen");
                scenario.Then("assure Firstname is changed on ViewModel", () =>
                    viewModel.Name.ShouldBe("Gøran Hansen"));
            });
        }


        [Test]
        public void Assure_Middlename_is_set_on_ViewModel()
        {
            Scenario.StartNew(this, scenario =>
            {
                Given_ViewModel_is_created(scenario);

                scenario.When("Middlename is changed on wrapper", () =>
                    wrapper.Middlename = "Haldis");
                scenario.Then("assure Middlename is changed on ViewModel", () =>
                    viewModel.Middlename.ShouldBe("Haldis"));
            });   
        }

        [Test]
        public void Assure_Surname_is_set_on_ViewModel()
        {
            Scenario.StartNew(this, scenario =>
            {
                Given_ViewModel_is_created(scenario);

                scenario.When("Surname is changed on wrapper", () =>
                    wrapper.Surname = "Hansen");
                scenario.Then("assure Surname is changed on ViewModel", () =>
                    viewModel.Surname.ShouldBe("Hansen"));
            });
        }

        [Test]
        public void Assure_ImageUrl_is_set_on_ViewModel()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(viewModel_exist).
                    And(wrapper_is_created);
                scenario.When("ImageUrl is changed on wrapper", () =>
                    wrapper.ImageUrl = "http://goeran.no/avatar.jpg");
                scenario.Then("assure ImageUrl is changed on ViewModel", () =>
                    viewModel.ImageUrl.ShouldBe("http://goeran.no/avatar.jpg"));
            });
        }

        [Test]
        public void Assure_Email_is_set_on_ViewModel()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(viewModel_exist).
                    And(wrapper_is_created);
                scenario.When("Email is changed on wrapper", () =>
                    wrapper.Email = "mail@goeran.no");
                scenario.Then("assure Email is changed on the ViewModel", () =>
                    viewModel.Email.ShouldBe("mail@goeran.no"));
            });
        }

        [Test]
        public void Assure_ImageUrl_is_validated()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(viewModel_exist).
                    And(wrapper_is_created);
                
                scenario.When("ImageUrl is changed and a invalid url is specified");
                scenario.Then("assure ValidationException is thrown", () =>
                {
                    this.ShouldThrowException<ValidationException>(() =>
                        wrapper.ImageUrl = "NotAValidUrl", exception =>
                        exception.Message.ShouldBe("Not well formatted URL"));    
                });
                scenario.Then("assure UnkownUser image url is mediated to ViewModel", () =>
                    viewModel.ImageUrl.ShouldBe("~/" + User.unknownUser.ImageUrl));

                scenario.When("ImageUrl is changed and a valid url is specified", () =>
                    wrapper.ImageUrl = "http://goeran.no/default.htm");
                scenario.Then("assure value is mediated to ViewModel", () =>
                    viewModel.ImageUrl.ShouldBe("http://goeran.no/default.htm" ));
            });
        }
    }
}
