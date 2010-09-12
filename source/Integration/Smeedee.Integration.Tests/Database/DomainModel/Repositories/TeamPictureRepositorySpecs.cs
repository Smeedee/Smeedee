using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Smeedee.DomainModel.TeamPicture;
using Smeedee.Integration.Database.DomainModel.Repositories;
using Smeedee.IntegrationTests.Database.DomainModel.Repositories;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Integration.Tests.Database.DomainModel.Repositories
{
    [TestFixture][Category("IntegrationTest")]
    public class TeamPictureRepositorySpecs : Shared
    {
        private const int WIDTH = 640;
        private const int HEIGHT = 480;
        private TeamPictureDatabaseRepository dbRepo;
        private TeamPicture firstTeamPicture;
        private Guid _firstGuid;

        [SetUp]
        public void Setup()
        {
            DeleteDatabaseIfExists();
            RecreateSessionFactory();
            this.dbRepo = new TeamPictureDatabaseRepository(sessionFactory);
            
            firstTeamPicture = new TeamPicture();
            _firstGuid = Guid.NewGuid();
            setFieldsFromGuid(_firstGuid, firstTeamPicture);
        }

        private void setFieldsFromGuid(Guid guid, TeamPicture teamPicture)
        {
            teamPicture.Message = guid.ToString();
            teamPicture.Picture = guid.ToByteArray();
            teamPicture.PictureHeight = HEIGHT;
            teamPicture.PictureWidth = WIDTH;
        }

        [Test]
        public void Assure_teamPicture_can_be_Saved()
        {
            dbRepo.Save(firstTeamPicture);

            RecreateSessionFactory();
            var newDbRepo = new TeamPictureDatabaseRepository(sessionFactory);
            var teamPictures = newDbRepo.Get(new CurrentTeamPictureSpecification());

            Assert_single_result(teamPictures, firstTeamPicture);
        }

        private void Assert_single_result(IEnumerable<TeamPicture> teamPictures, TeamPicture testData)
        {
            teamPictures.Count().ShouldBe(1);
            teamPictures.First().Message.ShouldBe(testData.Message);
            teamPictures.First().Picture.ShouldBe(testData.Picture);
            teamPictures.First().PictureHeight.ShouldBe(HEIGHT);
            teamPictures.First().PictureWidth.ShouldBe(WIDTH);
        }

        [Test]
        public void Assure_old_picture_is_overwritten_when_new_is_saved()
        {
            var secondGuid = Guid.NewGuid();
            var secondTeamPicture = new TeamPicture();
            setFieldsFromGuid(secondGuid, secondTeamPicture);

            dbRepo.Save(firstTeamPicture);
            dbRepo.Save(secondTeamPicture);

            RecreateSessionFactory();
            var newDbRepo = new TeamPictureDatabaseRepository(sessionFactory);

            var teamPictures = newDbRepo.Get(new CurrentTeamPictureSpecification());

            Assert_single_result(teamPictures, secondTeamPicture);
        }

    }
}