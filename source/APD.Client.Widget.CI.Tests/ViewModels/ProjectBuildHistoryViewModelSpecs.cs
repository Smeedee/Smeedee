using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using APD.Client.Framework;
using APD.Client.Widget.CI.ViewModels;
using APD.Tests;

using NUnit.Framework;

using TinyBDD.Specification.NUnit;


namespace APD.Client.Widget.CI.Tests.ViewModels.ProjectBuildHistoryViewModelSpecs
{
    [TestFixture]
    public class when_changing
    {
        [Test]
        public void Assure_ProjectName_fires_Propertychanged()
        {
            PropertyTester.TestChange<ProjectBuildHistoryViewModel>(
                new ProjectBuildHistoryViewModel(new NoUIInvocation()), bhvm => bhvm.ProjectName );

            PropertyTester.WasNotified.ShouldBeTrue();
        }

        [Test]
        public void Assure_SuccessfulBuildsByDate_fires_propertyChanged()
        {
            var newValue = new Dictionary<DateTime, int>();
            PropertyTester.TestChange<ProjectBuildHistoryViewModel>(
                new ProjectBuildHistoryViewModel(new NoUIInvocation()), bhvm => bhvm.SuccessfullBuildsByDate, newValue );

            PropertyTester.WasNotified.ShouldBeTrue();
        }


        [Test]
        public void Assure_FailedBuildsByDate_fires_propertyChanged()
        {
            var newValue = new Dictionary<DateTime, int>();
            PropertyTester.TestChange<ProjectBuildHistoryViewModel>(
                new ProjectBuildHistoryViewModel(new NoUIInvocation()), bhvm => bhvm.FailedBuildsByDate, newValue);

            PropertyTester.WasNotified.ShouldBeTrue();
        }
    }
}
