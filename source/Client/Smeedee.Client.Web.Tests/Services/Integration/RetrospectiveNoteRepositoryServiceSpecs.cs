using NUnit.Framework;
using Smeedee.Client.Web.Tests.RetrospectiveNoteRepositoryService;
using Smeedee.DomainModel.Corkboard;
using Smeedee.DomainModel.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace Smeedee.Client.Web.Tests.Services.Integration
{
    //Ignoring these integration specific learning tests
    public class RetrospectiveNoteRepositoryServiceSpecs
    {

        [Ignore]
        [TestFixture][Category("IntegrationTest")]
        public class When_retrospective_note_repository_webservice_is_running : ScenarioClass
        {

            private RetrospectiveNoteRepositoryServiceClient client;
            private RetrospectiveNote retrospectiveNote;
            private RetrospectiveNote[] result;

            [SetUp]
            public void SetUp()
            {
                client = new RetrospectiveNoteRepositoryServiceClient();
            }

            
            [Test]
            public void Assure_a_retrospective_note_can_be_saved()
            {
                Given("Given a retrospective note is created", () =>
                      retrospectiveNote = new RetrospectiveNote());

                When("The retrospective note is saved by the webservice", () =>
                    client.Save(new RetrospectiveNote[] { retrospectiveNote }));

                Then("Then there should be one more retrospective note in the repository", () =>
                    client.Get(new AllSpecification<RetrospectiveNote>()).ShouldNotBeNull());
            }

            [Test]
            public void Assure_a_retrospective_note_should_be_sent_through_the_webservice_and_retrieved_afterwards()
            {
                Given("A Retrospective Note with description 'Andreas' is created", () =>
                       retrospectiveNote = new RetrospectiveNote { Description = "Andreas" })
                       .And("The repository is empty", () => 
                       client.Delete(new AllSpecification<RetrospectiveNote>()))
                       .And("The note is saved using the webservice client", () =>
                       client.Save(new RetrospectiveNote[] { retrospectiveNote }));

                When("When the note is retrieved using the same webservice", () =>
                       result = client.Get(new AllSpecification<RetrospectiveNote>()));

                Then("The note description should be Andreas", () =>
                     result[0].Description.ShouldBe("Andreas"));
            }

            [Test]
            public void Assure_a_retrospective_note_can_be_deleted_from_the_repository_using_the_webservice()
            {
                retrospectiveNote = new RetrospectiveNote {Description = "test"};

                client.Save(new RetrospectiveNote[] {retrospectiveNote});

                var resultSize = client.Get(new AllSpecification<RetrospectiveNote>()).Length;

                client.Delete(new RetrospectiveNoteByDescriptionSpecification("test"));

                Assert.AreEqual(resultSize-1, client.Get(new AllSpecification<RetrospectiveNote>()).Length);
            }

            [Test]
            public void Assure_a_retrospective_note_can_be_deleted_by_using_delete_all_positive_notes_specification()
            {
                client.Delete(new AllSpecification<RetrospectiveNote>());

                for (int i = 0; i < 5; i++)
                {
                    retrospectiveNote = new RetrospectiveNote {Description = i.ToString(), Type = NoteType.Positive };
                    client.Save(new RetrospectiveNote[] { retrospectiveNote });
                }

                client.Delete(new RetrospectivePositiveNoteSpecification());
                Assert.AreEqual(0, client.Get(new AllSpecification<RetrospectiveNote>()).Length);
            }

            [Test]
            public void Assure_a_retrospective_note_can_be_deleted_by_using_delete_all_negative_notes_specification()
            {
                client.Delete(new AllSpecification<RetrospectiveNote>());

                for (int i = 0; i < 5; i++)
                {
                    retrospectiveNote = new RetrospectiveNote { Description = i.ToString(), Type = NoteType.Negative };
                    client.Save(new RetrospectiveNote[] { retrospectiveNote });
                }

                client.Delete(new RetrospectiveNegativeNoteSpecification());
                Assert.AreEqual(0, client.Get(new AllSpecification<RetrospectiveNote>()).Length);
            }

            [TearDown]
            public void TearDown()
            {
                StartScenario();
            }
        }
    }
}
