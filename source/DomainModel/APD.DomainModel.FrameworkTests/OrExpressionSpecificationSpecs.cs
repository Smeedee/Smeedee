using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APD.DomainModel.FrameworkTests.SharedContext;
using APD.DomainModel.SourceControl;

using NUnit.Framework;

using TinyBDD.Specification.NUnit;
using System.Linq.Expressions;


namespace APD.DomainModel.FrameworkTests.OrExpressionSpecificationSpecs
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
                And(Right_Specification_eval_false).
                And(OrExpressionSpec_is_created);
            When("eval expression");
            Then(() =>
                 orExpressionSpec.IsSatisfiedBy(changeset).ShouldBeTrue());
        }

        [Test]
        public void assure_it_evals_false()
        {
            Given(Left_Specification_eval_false).
                And(Right_Specification_eval_false).
                And(OrExpressionSpec_is_created);
            When("eval expression");
            Then(() =>
                orExpressionSpec.IsSatisfiedBy(changeset).ShouldBeFalse());
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
                And(OrExpressionSpec_is_created);
            When("interpret expression");
        }

        [Test]
        public void assure_LinqExpression_Node_is_Or()
        {
            Then(() =>
                 orExpressionSpec.IsSatisfiedByExpression().Body.NodeType.ShouldBe(ExpressionType.Or));
        }

        [Test]
        [Ignore]
        public void assure_Left_Specification_Expression_is_lifted()
        {
            Then(() =>
            {
                var linqExpression = orExpressionSpec.IsSatisfiedByExpression().Body as BinaryExpression;
                (linqExpression.Left == leftSpecification.IsSatisfiedByExpression().Body ).ShouldBeTrue();
            });
        }

        [Test]
        [Ignore]
        public void assure_Right_Specification_Expression_is_lifted()
        {
            Then(() =>
            {
                var linqExpression = orExpressionSpec.IsSatisfiedByExpression().Body as BinaryExpression;
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
            Scenario("When combine two complex Specifications");
            Given("left Specification is created", () =>
                leftSpecification = new ChangesetsAfterRevisionSpecification(100));
            And("right Specification is created", () =>
                rightSpecification = new ChangesetsForUserSpecification("goeran"));
            And(OrExpressionSpec_is_created);
            When("compile expression");
        }

        [Test]
        public void assure_Expression_compile()
        {
            Then(() =>
                 orExpressionSpec.IsSatisfiedByExpression().Compile());
        }
    }

}
