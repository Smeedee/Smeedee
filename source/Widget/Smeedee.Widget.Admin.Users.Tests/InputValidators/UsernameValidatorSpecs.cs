using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Smeedee.Widget.Admin.Users.InputValidators;
using Smeedee.Widget.Admin.Users.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Widget.Admin.Users.Tests.InputValidators
{
    public class When_validating : ScenarioClass
    {
        public static UserViewModel viewModel = new UserViewModel();

        [Test]
        public void Assure_validator_returns_error_if_username_is_invalid()
        {
            Given("a viewModel has invalid username", () =>
                {
                    viewModel.HasInvalidUsername = true;
                });
            When("the viewModel is validated");

            Then("viewModel should not be successfully validated", () =>
            {
                UsernameValidator.ValideUserViewModel(
                    null, new ValidationContext(viewModel, null, null)).ErrorMessage.ShouldBe("Username must be unique!");

            });
        }

        [Test]
        public void Assure_validator_returns_success_if_username_is_valid()
        {
            Given("the user has a valid username", () =>
            {
                viewModel.HasInvalidUsername = false;
            });
            When("the viewModel is validated");

            Then("viewModel should be successfully validated", () =>
            {

                UsernameValidator.ValideUserViewModel(
                    null, new ValidationContext(viewModel, null, null)).ShouldBe(ValidationResult.Success);
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
