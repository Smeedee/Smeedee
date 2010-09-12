using System;
using System.Linq;
using NUnit.Framework;
using Smeedee.DomainModel.TeamPicture;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Client.Web.Tests.Services.Integration
{
    [TestFixture][Category("IntegrationTest")]
    public class TeamPictureRepositoryServiceSpecs
    {
        private const int OneMegaByte = 1024*1024*1;

        [Test]
        public void Assure_webservice_can_save_and_retreive_teampicture()
        {
            var webservice = new TeamPictureService.TeamPictureRepositoryWebserviceClient();
            var alreadySavedCount = webservice.Get(new CurrentTeamPictureSpecification()).Count();

            TeamPicture teamPicture = new TeamPicture();
            var guid = Guid.NewGuid();
            teamPicture.Message = guid.ToString();
            teamPicture.Picture = guid.ToByteArray();
            teamPicture.PictureHeight = 480;
            teamPicture.PictureWidth = 640;

            webservice.Save(new [] {teamPicture});

            var returnedTeamPictures = webservice.Get(new CurrentTeamPictureSpecification());

            returnedTeamPictures.Count().ShouldBe(1);
            TeamPicture returnedTeamPicture = returnedTeamPictures.Last();

            returnedTeamPicture.Message.ShouldBe(teamPicture.Message);

            for (int i = 0; i < teamPicture.Picture.Length; i++)
            {
                Assert.AreEqual(teamPicture.Picture[i], returnedTeamPicture.Picture[i]);
            }

            returnedTeamPicture.Picture.ShouldBe(teamPicture.Picture);
            returnedTeamPicture.PictureHeight.ShouldBe(480);
            returnedTeamPicture.PictureWidth.ShouldBe(640);
        }

        [Test]
        public void Assure_webservice_can_handle_large_messages()
        {
            var webservice = new TeamPictureService.TeamPictureRepositoryWebserviceClient();
            var alreadySavedCount = webservice.Get(new CurrentTeamPictureSpecification()).Count();

            TeamPicture teamPicture = new TeamPicture();
            var guid = Guid.NewGuid();
            teamPicture.Message = guid.ToString();
            teamPicture.Picture = new byte[OneMegaByte];
            teamPicture.PictureHeight = 480;
            teamPicture.PictureWidth = 640;

            webservice.Save(new[] { teamPicture });

            var returnedTeamPictures = webservice.Get(new CurrentTeamPictureSpecification());

            returnedTeamPictures.Count().ShouldBe(1);
            returnedTeamPictures.Last().Message.ShouldBe(teamPicture.Message);
            returnedTeamPictures.Last().Picture.Length.ShouldBe(OneMegaByte);
            returnedTeamPictures.Last().PictureHeight.ShouldBe(480);
            returnedTeamPictures.Last().PictureWidth.ShouldBe(640);
        }
    }
}
