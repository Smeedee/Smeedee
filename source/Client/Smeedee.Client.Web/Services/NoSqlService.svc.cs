using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using Smeedee.Client.Web.Services.DTO.NoSql;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.DSL.Specifications;
using Smeedee.DomainModel.NoSql;
using Smeedee.Integration.Database.DomainModel.Repositories;

namespace Smeedee.Client.Web.Services
{
	[ServiceContract(Namespace="")]
	[AspNetCompatibilityRequirements(RequirementsMode=AspNetCompatibilityRequirementsMode.Allowed)]
	public class NoSqlService
	{
	    private IRepository<NoSqlDatabase> repository = new SqliteNoSqlDatabaseRepository();

        [OperationContract]
        [WebGet(UriTemplate = "/databases", RequestFormat = WebMessageFormat.Json)]
		public IEnumerable<NoSqlDatabaseDTO> GetAllDatabases()
        {
            return from database in repository.Get(All.ItemsOf<NoSqlDatabase>())
                   select new NoSqlDatabaseDTO()
                    {
                        Name = database.Name
                    };
        }

        [OperationContract]
        [WebGet(UriTemplate = "/databases/{database}", RequestFormat = WebMessageFormat.Json)]
        public IEnumerable<NoSqlCollectionDTO> GetCollections(string database)
        {
            return from collection in repository.Get(All.ItemsOf<NoSqlDatabase>()).SelectMany(d => d.Collections)
                   select new NoSqlCollectionDTO()
                   {
                       Name = collection.Key
                   };
        }

        [OperationContract]
        [WebGet(UriTemplate = "/databases/{database}/{collection}", RequestFormat = WebMessageFormat.Json)]
        public IEnumerable<NoSqlDocumentDTO> GetDocument(string database, string collection)
        {
            var allDocuments = repository.Get(All.ItemsOf<NoSqlDatabase>()).SelectMany(d => d.Collections).SelectMany(g => g.Value.Documents);
            return
                from document in allDocuments.Where(d => d.Collection.Name == collection && d.Collection.NoSqlDatabase.Name == database)
                select new NoSqlDocumentDTO
                {
                    Id = document.Id,
                    JSON = document.JSON,
                    Collection = new NoSqlCollectionDTO{ Name = document.Collection.Name }
                };
        }
            
        [OperationContract]
        [WebGet(UriTemplate = "/hello")]
        public string Hello()
        {
            return "hello world";
        }
		// Add more operations here and mark them with [OperationContract]
	}
}
