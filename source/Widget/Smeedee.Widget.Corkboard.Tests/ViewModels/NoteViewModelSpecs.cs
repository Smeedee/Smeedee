using NUnit.Framework;
using Smeedee.Client.Framework.Tests;
using Smeedee.DomainModel.Corkboard;
using Smeedee.Widget.Corkboard.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using TinyMVVM.Framework;

namespace Smeedee.Widget.Corkboard.Tests.ViewModels
{
    [TestFixture]
    public class When_move_command_is_triggered : SharedTestFixture
    {

        [Test]
        public void assure_move_event_is_triggered_when_moving_up()
        {
            Given(someone_subscribes_to_a_note);
            When("the note is asking to move up", () => note.MoveUp.ExecuteDelegate());
            Then("it should raise the Move event exactly once", () => raised.ShouldBe(1));
        }

        [Test]
        public void assure_move_event_is_triggered_when_moving_down()
        {
            Given(someone_subscribes_to_a_note);
            When("the note is asking to move down", () => note.MoveDown.ExecuteDelegate());
            Then("it should raise the Move event exactly once", () => raised.ShouldBe(1));
        }

        [Test]
        public void move_down_should_provide_correct_arguments()
        {
            Given(someone_subscribes_to_a_note);
            When("the note is asking to move down", () => note.MoveDown.ExecuteDelegate());
            Then("the move arguments should specify down as direction", () => moveargs.MoveDirection.ShouldBe(MoveArgs.Direction.DOWN));
        }

        [Test]
        public void move_up_should_provide_correct_arguments()
        {
            Given(someone_subscribes_to_a_note);
            When("the note is asking to move up", () => note.MoveUp.ExecuteDelegate());
            Then("the move arguments should specify up as direction", () => moveargs.MoveDirection.ShouldBe(MoveArgs.Direction.UP));
        }
    }


    [TestFixture]
    public class SharedTestFixture : ScenarioClass
    {
        protected static NoteViewModel note;
        protected static int raised;
        protected static MoveArgs moveargs;

        [SetUp]
        public void SetUp()
        {
            Scenario("");
            note = new NoteViewModel { Description = "positive note", Type = NoteType.Positive };
        }

        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }

        protected Context someone_subscribes_to_a_note = () =>
        {
            raised = 0;
            note.Move += MoveSubscriber;
        };

        protected static void MoveSubscriber(object sender, MoveArgs args)
        {
            raised++;
            moveargs = args;
        }
    }
}
