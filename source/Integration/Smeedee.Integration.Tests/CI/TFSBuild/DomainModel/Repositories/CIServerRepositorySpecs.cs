//#region File header

//// <copyright>
//// This library is free software; you can redistribute it and/or
//// modify it under the terms of the GNU Lesser General Public
//// License as published by the Free Software Foundation; either
//// version 2.1 of the License, or (at your option) any later version.
//// 
//// This library is distributed in the hope that it will be useful,
//// but WITHOUT ANY WARRANTY; without even the implied warranty of
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//// Lesser General Public License for more details.
//// 
//// You should have received a copy of the GNU Lesser General Public
//// License along with this library; if not, write to the Free Software
//// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//// /copyright> 
//// 
//// <contactinfo>
//// The project webpage is located at http://agileprojectdashboard.org/
//// which contains all the neccessary information.
//// </contactinfo>

//#endregion


//using System.Collections.Generic;
//using System.Linq;

//using Smeedee.DomainModel.CI;
//using Smeedee.Plugin.TFSBuild.DomainModel.Repositories;

//using NUnit.Framework;

//using TinyBDD.Dsl.GivenWhenThen;
//using TinyBDD.Specification.NUnit;

//using MSBuildStatus = Microsoft.TeamFoundation.Build.Client.BuildStatus;
//using BuildStatus = Smeedee.DomainModel.CI.BuildStatus;


//namespace Smeedee.Plugin.TFSBuildTests.DomainModel.Repositories.CIProjectRepositorySpecs
//{
    

//    [TestFixture][Category("IntegrationTest")]
//    public class CIProjectRepositoryUnitTests
//    {
//        [Test]
//        public void Assure_correct_conversion_from_in_progress_builds()
//        {
//            Scenario.StartNew(this, scenario =>
//            {
//                BuildStatus status = BuildStatus.Unknown;
//                scenario.Given("we want to convert in progress ms build status to ci build status");

//                scenario.When("we try to convert build status of in progress", () =>
//                    status = CIServerRepository.ConvertTFSStatusToBuildStatus(MSBuildStatus.InProgress));
//                scenario.Then("it should be returned as building", () => status.ShouldBe(BuildStatus.Building));
//            });
//        }

//        [Test]
//        public void Assure_correct_conversion_from_successfull_builds()
//        {
//            Scenario.StartNew(this, scenario =>
//            {
//                BuildStatus status = BuildStatus.Unknown;
//                scenario.Given("we want to convert successfull, and partially successfull ms build statuses to ci build status");

//                scenario.When("we try to convert build status of successfull and partially successfull", () =>
//                    status = CIServerRepository.ConvertTFSStatusToBuildStatus(MSBuildStatus.Succeeded));
//                scenario.Then("it should be returned as successfull", () =>
//                    status.ShouldBe(BuildStatus.FinishedSuccefully));

//                scenario.When("we try to convert build statusof partially successfull", () =>
//                    status = CIServerRepository.ConvertTFSStatusToBuildStatus(MSBuildStatus.PartiallySucceeded));
//                scenario.Then("it should be returned as successfull", () =>
//                    status.ShouldBe(BuildStatus.FinishedSuccefully));
//            });
//        }

//        [Test]
//        public void Assure_correct_conversion_from_unsuccessfull_builds()
//        {
//            Scenario.StartNew(this, scenario =>
//            {
//                BuildStatus status = BuildStatus.Unknown;
//                scenario.Given("we want to convert unsuccessfull ms build status to ci build status");

//                scenario.When("we try to convert build status of failed", () =>
//                    status = CIServerRepository.ConvertTFSStatusToBuildStatus(MSBuildStatus.Failed));
//                scenario.Then("it should be returned as finished with failure", () =>
//                    status.ShouldBe(BuildStatus.FinishedWithFailure));
//            });
//        }

//        [Test]
//        public void Assure_correct_conversion_from_all_unknown_builds()
//        {
//            Scenario.StartNew(this, scenario =>
//            {
//                BuildStatus status = BuildStatus.Unknown;
//                scenario.Given("we want to convert the other ms build statuses to ci build statuses");

//                scenario.When("we try to convert build status of not started", () =>
//                    status = CIServerRepository.ConvertTFSStatusToBuildStatus(MSBuildStatus.NotStarted));
//                scenario.Then("it should be returned as unknown", () =>
//                    status.ShouldBe(BuildStatus.Unknown));

//                scenario.When("we try to convert build status of stopped", () =>
//                    status = CIServerRepository.ConvertTFSStatusToBuildStatus(MSBuildStatus.Stopped));
//                scenario.Then("it should be returned as unknown", () => status.ShouldBe(BuildStatus.Unknown));
//            });
//        }
//    }
//}