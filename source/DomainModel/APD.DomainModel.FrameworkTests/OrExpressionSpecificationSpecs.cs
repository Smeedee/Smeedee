using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APD.DomainModel.FrameworkTests.SharedContext;

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
        public void assure_Left_Specification_Expression_is_lifted()
        {
            Then(() =>
            {
                var linqExpression = orExpressionSpec.IsSatisfiedByExpression().Body as BinaryExpression;
                (linqExpression.Left == leftSpecification.IsSatisfiedByExpression().Body ).ShouldBeTrue();
            });
        }

        [Test]
        public void assure_Right_Specification_Expression_is_lifted()
        {
            Then(() =>
            {
                var linqExpression = orExpressionSpec.IsSatisfiedByExpression().Body as BinaryExpression;
                (linqExpression.Right == rightSpecification.IsSatisfiedByExpression().Body).ShouldBeTrue();
            });
        }
    }
}
