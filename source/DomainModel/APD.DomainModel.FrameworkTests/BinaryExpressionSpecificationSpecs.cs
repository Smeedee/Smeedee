using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.DomainModel.Framework;

using TinyBDD.Dsl.GivenWhenThen;
using NUnit.Framework;

using TinyBDD.Specification.NUnit;
using APD.DomainModel.SourceControl;


namespace APD.DomainModel.FrameworkTests.BinaryExpressionSpecificationSpecs
{
    public class Shared : ScenarioClass
    {
        protected static BinaryExpressionSpecification<Changeset> binaryExpressionSpec;
        protected static Specification<Changeset> leftSpecification;
        protected static Specification<Changeset> rightSpecification;

        protected Context ExpressionSpecification_is_spawned = () =>
        {
            binaryExpressionSpec = new AndExpressionSpecification<Changeset>(leftSpecification, rightSpecification);
        };

        protected GivenSemantics Left_and_Right_Specification_is_created()
        {
            return Given(Left_Specification_is_created).
                And(Right_Specification_is_created);
        }

        protected Context Left_Specification_is_created = () =>
        {
            leftSpecification = new AllSpecification<Changeset>();
        };

        protected Context Right_Specification_is_created = () =>
        {
            rightSpecification = new AllSpecification<Changeset>();
        };

        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }
    }

    [TestFixture]
    public class When_spawning : Shared
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
    public class When_inspecting_structure : Shared
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
