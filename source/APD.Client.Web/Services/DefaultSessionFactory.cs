using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using APD.Integration.Database.DomainModel.Repositories;

using NHibernate;


namespace APD.Client.Web.Services
{
    public class DefaultSessionFactory
    {
        private static ISessionFactory instance = null;

        private DefaultSessionFactory()
        {}

        public static ISessionFactory Instance 
        { 
            get
            {
                if (instance == null)
                    instance =
                        NHibernateFactory.AssembleSessionFactory(ChangesetDatabaseRepository.DatabaseFilePath);

                return instance;
            } 
        }
    }
}
