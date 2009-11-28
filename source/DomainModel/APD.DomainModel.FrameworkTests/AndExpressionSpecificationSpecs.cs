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
}
