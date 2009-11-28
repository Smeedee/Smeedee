using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APD.DomainModel.FrameworkTests.SharedContext;

using NUnit.Framework;

using TinyBDD.Specification.NUnit;


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
}
