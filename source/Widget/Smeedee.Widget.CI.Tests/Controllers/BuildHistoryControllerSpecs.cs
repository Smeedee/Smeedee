using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Controller;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Framework;
using Smeedee.Widget.CI.Controllers;
using Smeedee.Widget.CI.ViewModels;
using TinyBDD.Specification.NUnit;
using TinyMVVM.Framework.Services;


namespace Smeedee.Client.Widget.CI.Tests.Controllers.BuildHistoryControllerSpecs
{
    public class shared
    {
        protected IUIInvoker invoker = new NoUIInvokation();
        protected Mock<ITimer> refresher = new Mock<ITimer>();
        protected Mock<IRepository<CIServer>> repository = new Mock<IRepository<CIServer>>();
        protected Mock<IProgressbar> progressbarMock = new Mock<IProgressbar>();
        protected DateTime firstDate;
        protected DateTime secondDate;
        protected Build failedBuild1;
        protected Build failedBuild2;
        protected Build failedBuild3;
        protected Build successBuild1;
        protected Build successBuild2;
        protected CIProject project1;
        protected CIProject project2;
        protected CIServer ciserver;

        public shared()
        {
            this.firstDate = new DateTime(2000, 10, 10);
            this.secondDate = new DateTime(2000, 10, 11);
            this.failedBuild1 = new Build() { FinishedTime = firstDate, Status = DomainModel.CI.BuildStatus.FinishedWithFailure };
            this.failedBuild2 = new Build() { FinishedTime = firstDate, Status = DomainModel.CI.BuildStatus.FinishedWithFailure };
            this.failedBuild3 = new Build() { FinishedTime = secondDate, Status = DomainModel.CI.BuildStatus.FinishedWithFailure };
            this.successBuild1 = new Build() { FinishedTime = firstDate, Status = DomainModel.CI.BuildStatus.FinishedSuccefully };
            this.successBuild2 = new Build() { FinishedTime = secondDate, Status = DomainModel.CI.BuildStatus.FinishedSuccefully };

            this.project1 = new CIProject("test one");
            project1.AddBuild(failedBuild1);
            project1.AddBuild(successBuild1);

            this.project2 = new CIProject("test two");
            project2.AddBuild(failedBuild2);
            project2.AddBuild(failedBuild3);
            project2.AddBuild(successBuild2);

            this.ciserver = new CIServer("server", "serverurl");
            ciserver.AddProject(project1);
            ciserver.AddProject(project2);

            repository.Setup(r => r.Get(It.IsAny<AllSpecification<CIServer>>()))
                .Returns(new List<CIServer>() { ciserver });
        }

    }

    [TestFixture]
    public class when_spawning : shared
    {
        [Test]
        public void Assure_controller_inherits_from_abstractController()
        {
            var viewModel = new BuildHistoryViewModel();
            var controller = new BuildHistoryController(viewModel, refresher.Object, invoker, repository.Object, progressbarMock.Object);
            var castedController = controller as ControllerBase<BuildHistoryViewModel>;
        }
    }

    [TestFixture]
    public class when_notified_to_refresh : shared
    {
        protected BuildHistoryController controller;

        [SetUp]
        public void Setup()
        {
            var viewModel = new BuildHistoryViewModel();
            controller = new BuildHistoryController(viewModel, refresher.Object, invoker, repository.Object, progressbarMock.Object);
        }

        [Test]
        public void Assure_controller_fills_viewmodel_with_data()
        {
            refresher.Raise( r => r.Elapsed += null, EventArgs.Empty);
            controller.ViewModel.Data.Count.ShouldBe(2);

            var firstProjectBuildHistory = controller.ViewModel.Data.Single( p => p.ProjectName == "test one");
            firstProjectBuildHistory.FailedBuildsByDate.Count.ShouldBe(1);
            firstProjectBuildHistory.FailedBuildsByDate[firstDate].ShouldBe(1);

            firstProjectBuildHistory.SuccessfullBuildsByDate.Count.ShouldBe(1);
            firstProjectBuildHistory.SuccessfullBuildsByDate[firstDate].ShouldBe(1);

            var secondProjectBuildHistorty = controller.ViewModel.Data.Single(p => p.ProjectName == "test two");
            secondProjectBuildHistorty.FailedBuildsByDate.Count.ShouldBe(2);
            secondProjectBuildHistorty.FailedBuildsByDate[firstDate].ShouldBe(1);
            secondProjectBuildHistorty.FailedBuildsByDate[secondDate].ShouldBe(1);

            secondProjectBuildHistorty.SuccessfullBuildsByDate.Count.ShouldBe(1);
            secondProjectBuildHistorty.SuccessfullBuildsByDate[secondDate].ShouldBe(1);
        }
    }
}
