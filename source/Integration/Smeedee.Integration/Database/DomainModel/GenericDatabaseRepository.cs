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

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;

using APD.DomainModel.Framework;

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

using Environment=System.Environment;
using APD.DomainModel.SourceControl;


namespace APD.Integration.Database.DomainModel.Repositories
{
    public class GenericDatabaseRepository<TDomainModelType> : IRepository<TDomainModelType>, IPersistDomainModels<TDomainModelType>
    {
        public static readonly string DatabaseFilePath =
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "smeedeeDB.db");

        protected readonly ISessionFactory sessionFactory;

        public GenericDatabaseRepository() :
            this(NHibernateFactory.AssembleSessionFactory(DatabaseFilePath))
        {
            
        }

        public GenericDatabaseRepository(ISessionFactory sessionFactory)
        {
            if (sessionFactory == null)
                throw new ArgumentNullException("sessionFactory");

            this.sessionFactory = sessionFactory;
        }

        public virtual IEnumerable<TDomainModelType> Get(Specification<TDomainModelType> specification)
        {
            using (var session = sessionFactory.OpenSession())
            {
                return session.CreateCriteria(typeof(TDomainModelType)).List<TDomainModelType>().
                    Where(cs => specification.IsSatisfiedBy(cs));
            }

        }

        public virtual void Save(IEnumerable<TDomainModelType> domainModels)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (session.BeginTransaction())
                {
                    foreach (var domainModel in domainModels)
                        session.SaveOrUpdate(domainModel);

                    session.Transaction.Commit();
                    session.Flush();
                }
            }
        }

        public virtual void Save(TDomainModelType domainModel)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (session.BeginTransaction())
                {
                    session.SaveOrUpdate(domainModel);
                    session.Transaction.Commit();
                    session.Flush();
                }
            }
        }
    }
}