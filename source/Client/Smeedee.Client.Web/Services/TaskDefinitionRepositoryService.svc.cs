using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.TaskDefinition;
using Smeedee.Integration.Database.DomainModel.Repositories;
using Smeedee.Tasks.Framework.TaskDefinitions;

namespace Smeedee.Client.Web.Services
{
    [ServiceContract(Namespace = "http://smeedee.org")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class TaskDefinitionRepositoryService
    {
        private ILog logger;

        public TaskDefinitionRepositoryService()
        {
            logger = new Logger(new LogEntryDatabaseRepository(DefaultSessionFactory.Instance));
        }

        /* NOTE: This returns the directory that contains all the task dlls. 
         * Currently, we have all of them in Smeedee.Tasks.dll
         * This code returns the dir of the executing assembly, Smeedee.Client.Web.dll
         * In production, all our dlls are in the same folder.
         * In testing, they are spread out, one folder per project, 
         * but Smeedee.Tasks.dll is copied over to this project's bin folder, since we reference it.
         * Thus, this gives expected results in production and in test.
         */
        private string TaskDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        [OperationContract]
        [ServiceKnownType(typeof(Specification<TaskDefinition>))]
        [ServiceKnownType(typeof(AllSpecification<TaskDefinition>))]
        public IEnumerable<TaskDefinition> Get(Specification<TaskDefinition> specification)
        {
            IEnumerable<TaskDefinition> result = new List<TaskDefinition>();
            var taskDefinitionLoader = new TaskDefinitionLoader(logger);

            try
            {
                result = taskDefinitionLoader.LoadFromFolder(TaskDirectory);
            }
            catch (Exception exception)
            {
                logger.WriteEntry(new ErrorLogEntry(GetType().ToString(), exception.ToString()));
            }

            return result.ToList();
        }
    }
}