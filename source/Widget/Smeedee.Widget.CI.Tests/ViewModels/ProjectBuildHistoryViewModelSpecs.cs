using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Smeedee.Client.Framework;
using Smeedee.Tests;

using NUnit.Framework;
using Smeedee.Widget.CI.ViewModels;
using TinyBDD.Specification.NUnit;


namespace Smeedee.Client.Widget.CI.Tests.ViewModels.ProjectBuildHistoryViewModelSpecs
{
    [TestFixture]
    public class when_changing
    {
        [Test]
        public void Assure_ProjectName_fires_Propertychanged()
        {
            PropertyTester.TestChange<ProjectBuildHistoryViewModel>(
                new ProjectBuildHistoryViewModel(), bhvm => bhvm.ProjectName );

            PropertyTester.WasNotified.ShouldBeTrue();
        }

        [Test]
        public void Assure_SuccessfulBuildsByDate_fires_propertyChanged()
        {
            var newValue = new Dictionary<DateTime, int>();
            PropertyTester.TestChange<ProjectBuildHistoryViewModel>(
                new ProjectBuildHistoryViewModel(), bhvm => bhvm.SuccessfullBuildsByDate, newValue );

            PropertyTester.WasNotified.ShouldBeTrue();
        }


        [Test]
        public void Assure_FailedBuildsByDate_fires_propertyChanged()
        {
            var newValue = new Dictionary<DateTime, int>();
            PropertyTester.TestChange<ProjectBuildHistoryViewModel>(
                new ProjectBuildHistoryViewModel(), bhvm => bhvm.FailedBuildsByDate, newValue);

            PropertyTester.WasNotified.ShouldBeTrue();
        }
    }
}
