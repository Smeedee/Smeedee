using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.SourceControl;
using Smeedee.DomainModel.TaskInstanceConfiguration;
using Smeedee.DomainModel.Users;
using Smeedee.Tasks.Framework;
using Smeedee.Tasks.SourceControl;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Tasks.Tests.SourceControl
{
    [TestFixture]
    public class when_users_exists_in_the_sourceControl_that_is_not_in_the_user_db : Shared
    {
        [Test]
        public void Assure_new_users_in_userDB_are_created()
        {
            Userdb savedUserDb = null;
            _userDbPersisterMock.Setup(r => r.Save(It.IsAny<Userdb>())).Callback((Userdb udb) => savedUserDb = udb);

            _task.Execute();

            savedUserDb.Users.Count().ShouldBe(2);
            savedUserDb.Users.Any(u => u.Username.Equals("tuxbear")).ShouldBeTrue();
            savedUserDb.Users.Any(u => u.Username.Equals("goeran")).ShouldBeTrue();
        }

        [Test]
        public void assure_handles_changeset_with_no_author()
        {
            _changesetRepositoryMock.Setup(r => r.Get(It.IsAny<Specification<Changeset>>())).Returns(
                new List<Changeset>
                    {
                        new Changeset() {Author = new Author("tuxbear"), Comment = "I ROX", Revision = 1},
                        new Changeset() {Author = new Author("tuxbear"), Comment = "I ROX2", Revision = 2},
                        new Changeset() {Author = new Author("goeran"), Comment = "I ROX 3 and 4", Revision = 3},
                        new Changeset() {Author = null, Comment = "I ROX 3 and 4", Revision = 4}
                    });
            Userdb savedUserDb = null;
            _userDbPersisterMock.Setup(r => r.Save(It.IsAny<Userdb>())).Callback((Userdb udb) => savedUserDb = udb);

            try
            {
                _task.Execute();
            } catch (NullReferenceException e)
            {
                Assert.Fail();
            }
        }
    }

    [TestFixture]
    public class when_there_is_no_userDb : Shared
    {
        [Test]
        public void Assure_a_userDb_is_created()
        {
            _userDbRepositoryMock.Setup(r => r.Get(It.IsAny<Specification<Userdb>>())).Returns(new List<Userdb>());
            _task.Execute();
            _userDbPersisterMock.Verify(r => r.Save(It.IsAny<Userdb>()), Times.Exactly(2));
        }
    }

    public class Shared
    {
        protected Mock<IRepository<Changeset>> _changesetRepositoryMock;
        protected Mock<IRepository<Userdb>> _userDbRepositoryMock;
        protected SourceControlUserToUserDbTask _task;
        protected Mock<IPersistDomainModels<Userdb>> _userDbPersisterMock;

        [SetUp]
        public void Setup()
        {
            _userDbPersisterMock = new Mock<IPersistDomainModels<Userdb>>();
            _userDbRepositoryMock = new Mock<IRepository<Userdb>>();
            _changesetRepositoryMock = new Mock<IRepository<Changeset>>();

            _changesetRepositoryMock.Setup(r => r.Get(It.IsAny<Specification<Changeset>>())).Returns(
                new List<Changeset>
                    {
                        new Changeset() {Author = new Author("tuxbear"), Comment = "I ROX", Revision = 1},
                        new Changeset() {Author = new Author("tuxbear"), Comment = "I ROX2", Revision = 2},
                        new Changeset() {Author = new Author("goeran"), Comment = "I ROX 3 and 4", Revision = 3}
                    });

            _userDbRepositoryMock.Setup(r => r.Get(It.IsAny<Specification<Userdb>>())).Returns(new List<Userdb> { new Userdb("default") });

            _task = new SourceControlUserToUserDbTask(_changesetRepositoryMock.Object, _userDbRepositoryMock.Object, _userDbPersisterMock.Object, new TaskConfiguration());

        }
    }

}
