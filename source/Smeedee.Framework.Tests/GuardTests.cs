using System;
using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Framework.Tests
{
    class GuardTests : Shared
    {
        [Test]
        public void assure_exception_is_thrown_when_null_is_given_as_input()
        {
            Given("always");

            When("always");

            Then("we should recieve a ArgumentNullException",
                () => this.ShouldThrowException<ArgumentException>(
                    () => Guard.ThrowExceptionIfNull(null, "object"),
                    exception =>
                    exception.Message.ShouldBe("The object argument cannot be null")));
        }

        [Test]
        public void Assure_exception_is_not_thrown_on_real_input()
        {
            Given("always");

            When(ThrowExceptionIfNull_is_called_with_a_not_null_object);

            Then("Nothing should happend, especially there should be no exception thrown");
        }

        [Test]
        public void should_throw_null_if_any_argument_is_null()
        {
            Given("anything");

            When("ThrowIfAnyNull is called");

            Then("exceptions should be thrown only when there are null arguments",
                    () =>
                        {
                            ThrowsException(() => Guard.ThrowIfNull<ArgumentException>("S", null, 0)).ShouldBeTrue();
                            ThrowsException(() => Guard.ThrowIfNull<ArgumentException>(null)).ShouldBeTrue();
                            ThrowsException(() => Guard.ThrowIfNull<ArgumentException>(null, null, 0)).ShouldBeTrue();
                            ThrowsException(() => Guard.ThrowIfNull<ArgumentException>("S", 4, 0, null)).ShouldBeTrue();

                            ThrowsException(() => Guard.ThrowIfNull<ArgumentException>("S", 4, 0)).ShouldBeFalse();
                            ThrowsException(() => Guard.ThrowIfNull<ArgumentException>("ll")).ShouldBeFalse();
                            ThrowsException(() => Guard.ThrowIfNull<ArgumentException>("S", 4)).ShouldBeFalse();
                            ThrowsException(() => Guard.ThrowIfNull<ArgumentException>("S", 4, 0, new string[] { null })).ShouldBeFalse();
                        });
        }
    }

    public class Shared : ScenarioClass
    {
        protected When ThrowExceptionIfNull_is_called_with_a_not_null_object =
            () => Guard.ThrowExceptionIfNull(new object(), "object");

        [SetUp]
        public void Setup()
        {
            Scenario("testing guard class");
        }
        
        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }

        public bool ThrowsException(Action fn)
        {
            try
            {
                fn.Invoke();
            } catch
            {
                return true;
            }
            return false;
        }
    }
}
