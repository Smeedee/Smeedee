using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Users;
using Smeedee.Widget.CI.Controllers;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Services;

namespace Smeedee.Widget.CI.Tests.Controllers
{
    public class Shared
    {
        protected static Mock<IRepository<CIServer>> RepositoryMock;
        protected static Mock<IRepository<User>> UserRepoMock;

        protected static DateTime MockStartTime = DateTime.Now;
        protected static DateTime MockEndTime = DateTime.Now.AddMinutes(30);


        protected static IInvokeBackgroundWorker<IEnumerable<CIProject>> BackgroundWorkerInvoker =
            new NoBackgroundWorkerInvocation<IEnumerable<CIProject>>();



        protected Context there_are_active_projects_in_CI_tool = () =>
        {
            RepositoryMock = new Mock<IRepository<CIServer>>();

            var normalBuild = CreateBuild(DomainModel.CI.BuildStatus.FinishedSuccefully,
                                          new CodeModifiedTrigger("tuxbear"));

            var systemBuild = CreateBuild(DomainModel.CI.BuildStatus.FinishedSuccefully,
                                          new EventTrigger("Trigger test"));

            var unknownBuild = CreateBuild(DomainModel.CI.BuildStatus.FinishedSuccefully,
                                           new UnknownTrigger());

            var nonExistingUserBuild = CreateBuild(DomainModel.CI.BuildStatus.FinishedSuccefully,
                                                   new CodeModifiedTrigger("I_DONT_EXIST"));

            var projects = new List<CIProject>();

            projects.Add(new CIProject("Project 1") { Builds = new List<Build> { normalBuild } });
            projects.Add(new CIProject("Project 2") { Builds = new List<Build> { systemBuild } });
            projects.Add(new CIProject("Project 3") { Builds = new List<Build> { unknownBuild } });
            projects.Add(new CIProject("Project 4") { Builds = new List<Build> { nonExistingUserBuild } });

            var servers = new List<CIServer>();
            servers.Add(new CIServer("What the fuck", "http://www.whatthefuck.com"));
            projects.ForEach(p =>
                servers.First().AddProject(p));

            RepositoryMock.Setup(r => r.Get(It.IsAny<AllSpecification<CIServer>>())).Returns(servers);
        };

        protected Context there_are_active_projects_in_CI_tool_with_different_BuildStatuses = () =>
        {
            RepositoryMock = new Mock<IRepository<CIServer>>();

            var normalBuild1 = CreateBuild(DomainModel.CI.BuildStatus.FinishedSuccefully,
                                          new CodeModifiedTrigger("tuxbear"));

            var normalBuild2 = CreateBuild(DomainModel.CI.BuildStatus.FinishedWithFailure,
                              new CodeModifiedTrigger("tuxbear"));

            var normalBuild3 = CreateBuild(DomainModel.CI.BuildStatus.FinishedSuccefully,
                              new CodeModifiedTrigger("tuxbear"));

            var systemBuild = CreateBuild(DomainModel.CI.BuildStatus.FinishedWithFailure,
                                          new EventTrigger("Trigger test"));

            var unknownBuild = CreateBuild(DomainModel.CI.BuildStatus.Building,
                                           new UnknownTrigger());

            var nonExistingUserBuild = CreateBuild(DomainModel.CI.BuildStatus.Unknown,
                                                   new CodeModifiedTrigger("I_DONT_EXIST"));

            var projects = new List<CIProject>();

            projects.Add(new CIProject("Project 1") { Builds = new List<Build> { normalBuild1 } });
            projects.Add(new CIProject("Project 2") { Builds = new List<Build> { systemBuild } });
            projects.Add(new CIProject("Project 3") { Builds = new List<Build> { unknownBuild } });
            projects.Add(new CIProject("Project 4") { Builds = new List<Build> { nonExistingUserBuild } });
            projects.Add(new CIProject("Project 5") { Builds = new List<Build> { normalBuild2 } });
            projects.Add(new CIProject("Project 6") { Builds = new List<Build> { normalBuild3 } });

            var servers = new List<CIServer>();
            servers.Add(new CIServer("What the fuck", "http://www.whatthefuck.com"));
            projects.ForEach(p =>
                servers.First().AddProject(p));

            RepositoryMock.Setup(r => r.Get(It.IsAny<AllSpecification<CIServer>>())).Returns(servers);
        };

        protected Context there_are_active_and_inactive_Projects_in_CI_tool = () =>
        {
            RepositoryMock = new Mock<IRepository<CIServer>>();

            var projects = new List<CIProject>();
            var normalBuild = CreateBuild(DomainModel.CI.BuildStatus.FinishedSuccefully,
                              new CodeModifiedTrigger("tuxbear"));
            projects.Add(new CIProject("Project 1") { Builds = new List<Build> { normalBuild } });

            var inactiveBUild = new Build()
            {
                StartTime = DateTime.Now.AddDays(-91),
                FinishedTime = DateTime.Now.AddDays(-91).AddMinutes(10),
                Status = Smeedee.DomainModel.CI.BuildStatus.FinishedWithFailure,
                Trigger = new CodeModifiedTrigger("goeran")
            };
            projects.Add(new CIProject("Project 1 main trunk") { Builds = new List<Build> { inactiveBUild } });

            var servers = new List<CIServer>();
            servers.Add(new CIServer("What the fuck", "http://www.whatthefuck.com"));
            projects.ForEach(p =>
                servers.First().AddProject(p));

            RepositoryMock.Setup(r => r.Get(It.IsAny<AllSpecification<CIServer>>())).Returns(servers);
        };

        protected Context user_exist_in_userdb = () =>
        {
            UserRepoMock = new Mock<IRepository<User>>();

            var normalUser = new User()
            {
                Email = "tuxbear@start.no",
                ImageUrl = "http://www.blah.com",
                Firstname = "Ole Andre",
                Username = "tuxbear"
            };

            var unknownUser = new User()
            {
                Firstname = "Unknown",
                Surname = "User",
                Username = "unknown",
                Email = "noemail",
                ImageUrl = "nourl"
            };

            var systemUser = new User()
            {
                Firstname = "System user",
                Username = "system",
                Email = "noemail",
                ImageUrl = "nourl"
            };

            UserRepoMock.Setup(r => r.Get(It.Is<UserByUsername>(s => s.Username.Equals("tuxbear")))).Returns(
                new List<User>() { normalUser });
            UserRepoMock.Setup(r => r.Get(It.Is<UserByUsername>(s => s.Username.Equals("unknown")))).Returns(
                new List<User>() { unknownUser });
            UserRepoMock.Setup(r => r.Get(It.Is<UserByUsername>(s => s.Username.Equals("system")))).Returns(
                new List<User>() { systemUser });
        };


        protected static Build CreateBuild(DomainModel.CI.BuildStatus status, Trigger trigger)
        {
            return new Build()
            {
                StartTime = MockStartTime,
                FinishedTime = MockEndTime,
                Status = status,
                Trigger = trigger
            };
        }
    }
}
