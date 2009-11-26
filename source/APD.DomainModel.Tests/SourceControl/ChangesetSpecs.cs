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
// /copyright> 
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
using APD.DomainModel.SourceControl;
using NUnit.Framework;


namespace APD.DomainModel.SourceControlTests
{
    [TestFixture]
    public class ChangesetSpecs
    {
        [Test]
        public void Assure_has_author()
        {
            var cs = new Changeset();
            Assert.IsNull(cs.Author);

            var user = new Author {Username = "dagolap"};
            cs.Author = user;
            Assert.AreEqual(cs.Author, user);
        }

        [Test]
        public void Assure_has_datetime()
        {
            var cs = new Changeset();
            Assert.IsNotNull(cs.Time);

            var date = new DateTime(1986, 04, 12);
            cs.Time = date;
            Assert.AreEqual(cs.Time, date);
        }

        [Test]
        public void Assure_has_message()
        {
            var cs = new Changeset();
            Assert.IsNull(cs.Comment);

            const string log = "This is a log message!";
            cs.Comment = log;
            Assert.AreEqual(cs.Comment, log);
        }

        [Test]
        public void Assure_has_revision()
        {
            var cs = new Changeset();
            Assert.AreEqual(cs.Revision, 0);

            cs.Revision = 1;
            Assert.AreEqual(cs.Revision, 1);
        }

        [Test]
        public void Assure_returns_bad_log_when_empty()
        {
            var cs = new Changeset();
            Assert.IsFalse(cs.IsValidComment());
        }

        [Test]
        public void Assure_returns_good_log_when_not_empty()
        {
            var cs = new Changeset
                     {
                         Comment = "Added tests for SC domain model."
                     };
            Assert.IsTrue(cs.IsValidComment());
        }
    }
}