using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Smeedee.DomainModel.Corkboard;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.Integration.Database.DomainModel.Repositories;

namespace Smeedee.Client.Web.Services
{
    [ServiceContract(Namespace = "http://smeedee.org")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceKnownType(typeof(AllSpecification<RetrospectiveNote>))]
    [ServiceKnownType(typeof(RetrospectiveNoteByDescriptionSpecification))]
    [ServiceKnownType(typeof(RetrospectiveNoteByIdSpecification))]
    [ServiceKnownType(typeof(RetrospectiveNegativeNoteSpecification))]
    [ServiceKnownType(typeof(RetrospectivePositiveNoteSpecification))]
    [ServiceKnownType(typeof(Specification<RetrospectiveNote>))]

    public class RetrospectiveNoteRepositoryService
    {
        private readonly RetrospectiveNoteDatabaseRepository retrospectiveNoteRepository;

        public RetrospectiveNoteRepositoryService()
        {
            retrospectiveNoteRepository = new RetrospectiveNoteDatabaseRepository(DefaultSessionFactory.Instance);
        }

        [OperationContract]
        public IEnumerable<RetrospectiveNote> Get(Specification<RetrospectiveNote> specification)
        {
            IEnumerable<RetrospectiveNote> result = new List<RetrospectiveNote>();

            try
            {
                result = retrospectiveNoteRepository.Get(specification);
            }
            catch (Exception exception)
            {
                ILog logger = new Logger(new LogEntryDatabaseRepository(DefaultSessionFactory.Instance));
                logger.WriteEntry(new ErrorLogEntry(GetType().ToString(), exception.ToString()));
            }

            return result;
        }

        [OperationContract]
        public void Save(IEnumerable<RetrospectiveNote> retrospectiveNote)
        {
            try
            {
                retrospectiveNoteRepository.Save(retrospectiveNote);
            }
            catch (Exception exception)
            {
                ILog logger = new Logger(new LogEntryDatabaseRepository(DefaultSessionFactory.Instance));
                logger.WriteEntry(new ErrorLogEntry(GetType().ToString(), exception.ToString()));
            }
        }

        [OperationContract]
        public void Delete(Specification<RetrospectiveNote> specification)
        {
            try
            {
                retrospectiveNoteRepository.Delete(specification);
            }
            catch (Exception exception)
            {
                ILog logger = new Logger(new LogEntryDatabaseRepository(DefaultSessionFactory.Instance));
                logger.WriteEntry(new ErrorLogEntry(GetType().ToString(), exception.ToString()));
            }
        }
    }
}