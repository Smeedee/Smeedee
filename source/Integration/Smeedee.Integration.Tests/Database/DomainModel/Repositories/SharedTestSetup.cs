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

using System.Collections.Generic;
using System.IO;
using Smeedee.DomainModel.Users;
using TinyBDD.Dsl.GivenWhenThen;
using NHibernate;
using Smeedee.Integration.Database.DomainModel.Repositories;


namespace Smeedee.IntegrationTests.Database.DomainModel.Repositories
{
    public class Shared
    {
        protected static ISessionFactory sessionFactory;

        protected static string DATABASE_TEST_FILE = "smeedeeDB_testing.db";
        protected const string TESTPROJECTNAME_TWO = "This is my project";
        protected const string TESTPROJECTNAME_ONE = "Testproject";

        protected Shared()
        {
            RecreateSessionFactory();
        }

        protected static void RecreateSessionFactory()
        {
            sessionFactory = NHibernateFactory.AssembleSessionFactory(DATABASE_TEST_FILE);
        }

        protected static void DeleteDatabaseIfExists()
        {
            File.Delete(DATABASE_TEST_FILE);
        }

        protected Context Database_exists = () =>
        {
            DeleteDatabaseIfExists();
            RecreateSessionFactory();
        };

        protected Context Database_contains_user_data = () =>
        {
            using (var session = sessionFactory.OpenSession())
            {
                var userdb = new Userdb("default");

                var users = new List<User>();
                users.Add(new User
                {
                    Email = "mail@goeran.no",
                    ImageUrl = "http://goeran.no/avatar.jpg",
                    Firstname = "Gøran",
                    Surname = "Hansen",
                    Username = "goeran"
                });

                users.Add(new User
                {
                    Email = "jonas@follesoe.no",
                    ImageUrl = "http://jonas.follesoe.no/avatar.jpg",
                    Firstname = "Jonas Follesø",
                    Username = "jonas"
                });

                userdb.Users = users;
                session.Save(userdb);
                session.Flush();
            }
        };
    }

    public static class DatabaseRetriever
    {
        public static IList<T> GetDatamodelFromDatabase<T>(ISessionFactory sessionFactory)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    return session.CreateCriteria(typeof (T)).List<T>();
                }
            }
        }
    }
}