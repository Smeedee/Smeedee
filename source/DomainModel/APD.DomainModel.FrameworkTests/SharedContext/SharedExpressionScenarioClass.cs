using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.DomainModel.Framework;
using APD.DomainModel.SourceControl;

using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;

namespace APD.DomainModel.FrameworkTests.SharedContext
{
    public class SharedExpressionScenarioClass : ScenarioClass
    {
        protected static Changeset changeset = new Changeset();
        protected static AndExpressionSpecification<Changeset> andExpressionSpec;
        protected static OrExpressionSpecification<Changeset> orExpressionSpec;
        protected static BinaryExpressionSpecification<Changeset> binaryExpressionSpec;
        protected static Specification<Changeset> leftSpecification;
        protected static Specification<Changeset> rightSpecification;

        protected Context Left_Specification_eval_true = () =>
        {
            leftSpecification = new LinqSpecification<Changeset>(c => true);
        };

        protected Context Left_Specification_eval_false = () =>
        {
            leftSpecification = new LinqSpecification<Changeset>(c => false);
        };

        protected Context Right_specification_eval_true = () =>
        {
            rightSpecification = new LinqSpecification<Changeset>(c => true);
        };

        protected Context Right_Specification_eval_false = () =>
        {
            rightSpecification = new LinqSpecification<Changeset>(c => false);
        };

        protected Context Left_Specification_is_created = () =>
        {
            leftSpecification = new AllSpecification<Changeset>();
        };

        protected Context Right_Specification_is_created = () =>
        {
            rightSpecification = new AllSpecification<Changeset>();
        };

        protected Context AndExpressionSpec_is_created = () =>
        {
            andExpressionSpec = new AndExpressionSpecification<Changeset>(leftSpecification, rightSpecification);
        };

        protected Context OrExpressionSpec_is_created = () =>
        {
            orExpressionSpec = new OrExpressionSpecification<Changeset>(leftSpecification, rightSpecification);
        };

        protected Context ExpressionSpecification_is_spawned = () =>
        {
            binaryExpressionSpec = new AndExpressionSpecification<Changeset>(leftSpecification, rightSpecification);
        };

        protected GivenSemantics Left_and_Right_Specification_is_created()
        {
            return Given(Left_Specification_is_created).
                And(Right_Specification_is_created);
        }

        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }
    }
}
