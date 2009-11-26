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
using APD.DomainModel.NHMapping.Entities;
using APD.DomainModel.SourceControl;
using FluentNHibernate.Cfg.Db;
using NUnit.Framework;
using TinyBDD.Specification.NUnit;
using NHibernate;
using APD.Integration.Database.DomainModel.Repositories;


namespace APD.IntegrationTests.Database.DomainModel.Repositories.ChangesetPersisterSpecs
{
    [TestFixture]
    public class When_saving : Shared
    {
        protected const string TEST_USERNAME = "Kim Bjarne me Musa";
        protected const string TEST_COMMENT = "What what in the butt butt";
        protected const int TEST_REVISION_NUMBER = 1001;
        protected static readonly DateTime TEST_TIME = DateTime.Today;

        [SetUp]
        public void Setup()
        {
            DeleteDatabaseIfExists();
            RecreateSessionFactory();
        }
        
        [Test]
        public void Assure_Changeset_is_saved()
        {
            
            var changesetPersister = new ChangesetPersister(sessionFactory);

            var changeset = new Changeset
                            {
                                Author = new Author {Username = TEST_USERNAME},
                                Comment = TEST_COMMENT,
                                Revision = TEST_REVISION_NUMBER,
                                Time = TEST_TIME
                            };

            changesetPersister.Save(changeset);

            ISessionFactory newSessionFactory = NHibernateFactory.AssembleSessionFactory(DATABASE_TEST_FILE);

            var dbResults = DatabaseRetriever.GetDatamodelFromDatabase<Changeset>(newSessionFactory);

            dbResults[0].Author.Username.ShouldBe(TEST_USERNAME);
            dbResults[0].Comment.ShouldBe(TEST_COMMENT);
            dbResults[0].Revision.ShouldBe(TEST_REVISION_NUMBER);
            dbResults[0].Time.ShouldBe(TEST_TIME);
        }


    }
}
