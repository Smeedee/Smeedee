using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Smeedee.DomainModel.Framework;

using TinyBDD.Dsl.GivenWhenThen;
using NUnit.Framework;

using TinyBDD.Specification.NUnit;
using Smeedee.DomainModel.SourceControl;
using Smeedee.DomainModel.FrameworkTests.SharedContext;


namespace Smeedee.DomainModel.FrameworkTests.BinaryExpressionSpecificationSpecs
{
    [TestFixture]
    public class When_spawning : SharedExpressionScenarioClass
    {
        [SetUp]
        public void Setup()
        {
            Scenario("When spawning");
            Given(Left_and_Right_Specification_is_created()).
                And("Expression is not spawned");
            When("spawning");
        }

        [Test]
        public void Assure_Left_Specification_is_validated()
        {
            Then(() =>
                this.ShouldThrowException<ArgumentNullException>(() =>
                    new AndExpressionSpecification<Changeset>(null, rightSpecification)));
        }

        [Test]
        public void Assure_Right_Specification_is_validated()
        {
            Then(() =>
                this.ShouldThrowException<ArgumentNullException>(() =>
                    new AndExpressionSpecification<Changeset>(leftSpecification, null)));
        }
    }

    [TestFixture]
    public class When_inspecting_structure : SharedExpressionScenarioClass
    {
        [SetUp]
        public void Setup()
        {
            Scenario("When inspecting structure");
            Given(Left_and_Right_Specification_is_created()).
                And(ExpressionSpecification_is_spawned);
            When("inspecting structure");
        }

        [Test]
        public void Assure_it_is_a_Specification()
        {
            Then(() =>
                (binaryExpressionSpec is Specification<Changeset>).ShouldBeTrue());
        }

        [Test]
        public void Assure_it_has_a_Left_side()
        {
            Then(() =>
                binaryExpressionSpec.Left.ShouldNotBeNull());
        }

        [Test]
        public void Assure_right_has_Right_side()
        {
            Then(() =>
                binaryExpressionSpec.Right.ShouldNotBeNull());
        }
    }

}
