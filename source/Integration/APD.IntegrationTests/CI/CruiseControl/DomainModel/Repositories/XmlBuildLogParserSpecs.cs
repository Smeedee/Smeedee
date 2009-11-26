#region File header

// <copyright>
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
// </copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using APD.DomainModel.CI;
using APD.Integration.CI.CruiseControl.DomainModel.Repositories;
using APD.IntegrationTests.CI.CruiseControl.Tests;

using NUnit.Framework;
using TinyBDD.Specification.NUnit;


namespace APD.IntegrationTests.CI.CruiseControl.DomainModel.Repositories.XmlBuildLogParserSpecs
{
    public class Shared
    {
        protected XmlBuildLogParser parser;
        
        public Shared()
        {
            parser = new XmlBuildLogParser();
        }
    }

    [TestFixture]
    public class when_input_is_malformed : Shared
    {
        [Test]
        [ExpectedException(typeof(CruiseControlRepositoryException))]
        public void should_fail_with_CruiseControlRepositoryException()
        {
            Build build = parser.Parse(Resource.MalformedXml);
        }
    }

    [TestFixture]
    public class when_input_is_invalid : Shared
    {
        [Test]
        [ExpectedException(typeof(CruiseControlRepositoryException))]
        public void should_fail_with_CruiseControlRepositoryException()
        {
            Build build = parser.Parse(Resource.empty_buildlog);
        }
    }

    [TestFixture]
    public class when_build_is_forced_and_failed : Shared
    {
        [Test]
        public void should_have_eventtrigger_and_finishedwithfailure_status()
        {
            Build build = parser.Parse(Resource.forced_failed);
            Assert.IsTrue(build.Status == BuildStatus.FinishedWithFailure);
            build.Trigger.ShouldBeInstanceOfType<EventTrigger>();
        }
    }

    [TestFixture]
    public class when_build_is_forced_and_successful : Shared
    {
        [Test]
        public void should_have_eventtrigger_and_finishedsuccessfully_status()
        {
            Build build = parser.Parse(Resource.forced_successfull);
            Assert.IsTrue(build.Status == BuildStatus.FinishedSuccefully);
            build.Trigger.ShouldBeInstanceOfType<EventTrigger>();
        }
    }

    [TestFixture]
    public class when_build_is_forced_and_has_unknown_result : Shared
    {
        [Test]
        public void should_have_eventtrigger_and_unknown_status()
        {
            Build build = parser.Parse(Resource.forced_exception);
            Assert.IsTrue(build.Status == BuildStatus.Unknown);
            build.Trigger.ShouldBeInstanceOfType<EventTrigger>();
        }
    }

    [TestFixture]
    public class when_build_is_modified_and_failed : Shared
    {
        [Test]
        public void should_have_codemodifiedtrigger_and_finishedwithfailure_status()
        {
            Build build = parser.Parse(Resource.modified_failed);
            Assert.IsTrue(build.Status == BuildStatus.FinishedWithFailure);
            build.Trigger.ShouldBeInstanceOfType<CodeModifiedTrigger>();
            CodeModifiedTrigger trigger = build.Trigger as CodeModifiedTrigger;
            trigger.InvokedBy.ShouldBe("goeran");
        }
    }

    [TestFixture]
    public class when_build_is_modified_and_successful : Shared
    {
        [Test]
        public void should_return_build_with_codemodifiedtrigger_and_finishedsuccessfully_status()
        {
            Build build = parser.Parse(Resource.modified_success);
            Assert.IsTrue(build.Status == BuildStatus.FinishedSuccefully);
            build.Trigger.ShouldBeInstanceOfType<CodeModifiedTrigger>();
            CodeModifiedTrigger trigger = build.Trigger as CodeModifiedTrigger;
            trigger.InvokedBy.ShouldBe("jaffe");
        }
    }

    [TestFixture]
    public class when_build_is_unknown_and_successful : Shared
    {
        [Test]
        public void should_return_build_with_unknowntrigger_and_finishedsuccessfully_status()
        {
            Build build = parser.Parse(Resource.unknown_success);
            Assert.IsTrue(build.Status == BuildStatus.FinishedSuccefully);
            build.Trigger.ShouldBeInstanceOfType<UnknownTrigger>();
        }
    }



}