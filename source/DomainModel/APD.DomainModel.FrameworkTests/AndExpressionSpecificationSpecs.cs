using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.DomainModel.Framework;

using TinyBDD.Dsl.GivenWhenThen;
using NUnit.Framework;
using APD.DomainModel.SourceControl;

using TinyBDD.Specification.NUnit;
using APD.DomainModel.FrameworkTests.SharedContext;
using System.Linq.Expressions;


namespace APD.DomainModel.FrameworkTests.AndExpressionSpecificationSpecs
{
   [TestFixture]
    public class When_eval_expression : SharedExpressionScenarioClass
    {
        [SetUp]
        public void Setup()
        {
            Scenario("When eval expression");
        }

        [Test]
        public void assure_it_evals_true()
        {
            Given(Left_Specification_eval_true).
                And(Right_specification_eval_true).
                And(AndExpressionSpec_is_created);
            When("eval expression");
            Then(() =>
                 andExpressionSpec.IsSatisfiedBy(changeset).ShouldBeTrue());
        }

        [Test]
        public void Assure_it_evals_false()
        {
            Given(Left_Specification_eval_true).
                And(Right_Specification_eval_false).
                And(AndExpressionSpec_is_created);
            When("eval expression");
            Then(() =>
                andExpressionSpec.IsSatisfiedBy(changeset).ShouldBeFalse());
        }
    }

    [TestFixture]
    public class When_interpret_expression : SharedExpressionScenarioClass
    {
        [SetUp]
        public void Setup()
        {
            Scenario("Interpret expression");
            Given(Left_Specification_eval_true).
                And(Right_Specification_eval_false).
                And(AndExpressionSpec_is_created);
            When("interpret expression");
        }

        [Test]
        public void assure_LinqExpression_Node_is_And()
        {
            Then(() =>
            {
                var linqExpression = andExpressionSpec.IsSatisfiedByExpression().Body;
                linqExpression.NodeType.ShouldBe(ExpressionType.And);
            });
        }

        [Test]
        [Ignore]
        public void assure_Left_Specification_Expression_is_lifted()
        {
            Then(() =>
            {
                var linqExpression = andExpressionSpec.IsSatisfiedByExpression().Body as BinaryExpression;
                (linqExpression.Left == leftSpecification.IsSatisfiedByExpression().Body).ShouldBeTrue();
            });
        }

        [Test]
        [Ignore]
        public void assure_Right_Specification_Expression_is_lifted()
        {
            Then(() =>
            {
                var linqExpression = andExpressionSpec.IsSatisfiedByExpression().Body as BinaryExpression;
                (linqExpression.Right == rightSpecification.IsSatisfiedByExpression().Body).ShouldBeTrue();
            });
        }
    }

    [TestFixture]
    public class When_combining_two_complex_Specifications : SharedExpressionScenarioClass
    {
        [SetUp]
        public void Setup()
        {
            long h = 100;
            string name = "goeran";
            Scenario("When combine two complex Specifications");
            Given("left Specification is created", () =>
                leftSpecification = new ChangesetsAfterRevisionSpecification(h));
            And("right Specification is created", () =>
                rightSpecification = new ChangesetsForUserSpecification(name));
            And(AndExpressionSpec_is_created);
            When("compile expression");
        }

        [Test]
        public void assure_Expression_compile()
        {
            Then(() =>
                 andExpressionSpec.IsSatisfiedByExpression().Compile());
        }
    }
}
