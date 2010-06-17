using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;

namespace Smeedee.Integration.Database.DomainModel.Repositories
{
    public class NHibernateFactory
    {
        private static object lockObject = new object();

        public static ISessionFactory AssembleSessionFactory(string databaseFile)
        {
            lock (lockObject)
            {
                return NHibConfig.CreateConfiguration(databaseFile).BuildSessionFactory();
            }
        }
    }
}
