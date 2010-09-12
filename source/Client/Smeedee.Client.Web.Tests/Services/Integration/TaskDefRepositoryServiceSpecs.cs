using System;
using System.Linq;
using NUnit.Framework;
using Smeedee.Client.Web.Tests.TaskDefRepositoryService;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.TaskDefinition;

namespace Smeedee.Client.WebTests.Services.Integration
{
    [TestFixture][Category("IntegrationTest")]
    public class TaskDefRepositoryServiceSpecs 
    {
        [Test]
        public void assure_data_can_be_serialized_using_the_webservice()
        {
            var client = new TaskDefinitionRepositoryServiceClient();

            var result = client.Get(new AllSpecification<TaskDefinition>());

            Assert.IsTrue(result.Count() > 0);
        }
    }
}