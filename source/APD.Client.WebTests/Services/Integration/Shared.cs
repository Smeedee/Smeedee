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
using System.IO;
using APD.DomainModel.NHMapping.Entities;
using APD.Integration.Database.DomainModel.Repositories;
using TinyBDD.Dsl.GivenWhenThen;
using NHibernate;
using APD.DomainModel.Users;


namespace APD.Client.WebTests.Services.Integration
{
    public class Shared
    {
        protected const string DATE_FORMAT = "dd.MM.yyyy hh:mm:ss";

        private static ISessionFactory sessionfactory;
        protected static ISession databaseSession;

        protected Context Database_is_created = () =>
        {
            if (File.Exists(GenericDatabaseRepository<User>.DatabaseFilePath))
                File.Delete(GenericDatabaseRepository<User>.DatabaseFilePath);

            sessionfactory = NHibernateFactory.AssembleSessionFactory(GenericDatabaseRepository<User>.DatabaseFilePath);

            databaseSession = sessionfactory.OpenSession();
        };
    }
}
