using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;
using DataServicesJSONP;
using Smeedee.Client.Web.Services.DTO.SourceControl;
using Smeedee.Client.Web.Services.DTO.NoSql;
using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.DSL.Specifications;
using Smeedee.DomainModel.NoSql;
using Smeedee.DomainModel.SourceControl;
using Smeedee.Integration.Database.DomainModel.Repositories;

namespace Smeedee.Client.Web.Services   
{
    [JSONPSupportBehavior]
    public class rest : DataService<SmeedeeREST>
    {
        // This method is called only once to initialize service-wide policies.
        public static void InitializeService(DataServiceConfiguration config)
        {
            // TODO: set rules to indicate which entity sets and service operations are visible, updatable, etc.
            // Examples:
            config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
            // config.SetServiceOperationAccessRule("MyServiceOperation", ServiceOperationRights.All);
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V2;
        }
    }

    public class SmeedeeREST
    {
        private static IRepository<Changeset> changesetRepository = new ChangesetDatabaseRepository();
        private static IRepository<NoSqlDatabase> noSqlDatabaseRepository = new SqliteNoSqlDatabaseRepository();

        public IQueryable<ChangesetDTO> Changesets
        {
            get
            {
                return (from changeset in changesetRepository.Get(All.ItemsOf<Changeset>())
                        select new ChangesetDTO()
                        {
                            Revision = changeset.Revision.ToString(),
                            Author = new AuthorDTO { Username = changeset.Author.Username },
                            Time = changeset.Time,
                            Comment = changeset.Comment
                        }).AsQueryable();
            }
        }

        public IQueryable<NoSqlDatabaseDTO> NoSqlDatabases
        {
            get
            {
                var databases = noSqlDatabaseRepository.Get(All.ItemsOf<NoSqlDatabase>());
                return (from database in databases
                        select new NoSqlDatabaseDTO()
                        {
                            Name = database.Name,
                            Collections = NoSqlCollections.Where(d => d.Database.Name == database.Name)
                        }).AsQueryable();
            }
        }

        public IQueryable<NoSqlCollectionDTO> NoSqlCollections
        {
            get
            {
                var databases = noSqlDatabaseRepository.Get(All.ItemsOf<NoSqlDatabase>());
                var collections = databases.SelectMany(d => d.Collections).Select(a => a.Value);

                return (from collection in collections
                        select new NoSqlCollectionDTO()
                        {
                            Name = collection.Name,
                            Database = new NoSqlDatabaseDTO()
                            {
                                Name = collection.NoSqlDatabase.Name
                            },
                            Documents = NoSqlDocuments.Where(d => d.Collection.Name == collection.Name &&
                                d.Collection.Database.Name == collection.NoSqlDatabase.Name)
                        }).AsQueryable();
            }
        }

        public IQueryable<NoSqlDocumentDTO> NoSqlDocuments
        {
            get
            {
                var databases = noSqlDatabaseRepository.Get(All.ItemsOf<NoSqlDatabase>());
                var documents =
                    databases.SelectMany(d => d.Collections).SelectMany(c => c.Value.Documents).Select(d => d);

                return (from document in documents
                        select new NoSqlDocumentDTO()
                        {
                            Id = document.Id,
                            JSON = document.JSON,
                            Collection = new NoSqlCollectionDTO()
                            {
                                Name = document.Collection.Name,
                                Database = new NoSqlDatabaseDTO()
                                {
                                    Name = document.Collection.NoSqlDatabase.Name
                                }
                            }
                        }).AsQueryable();
            }
        }
    }
}
