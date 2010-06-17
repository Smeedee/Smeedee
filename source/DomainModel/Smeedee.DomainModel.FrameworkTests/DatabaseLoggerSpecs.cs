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
// The project webpage is located at http://smeedee.org/
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System.Collections.Generic;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;

using Moq;
using NUnit.Framework;
using TinyBDD.Specification.NUnit;


namespace Smeedee.IntegrationTests.Database.DomainModel.Repositories.DatabaseLoggerSpecs
{

    [TestFixture]
    public class DatabaseLoggerSpecs
    {
        private Mock<IPersistDomainModels<LogEntry>> mockPersister;
        private List<LogEntry> savedLogs;

        [SetUp]
        public void Setup()
        {

            savedLogs = new List<LogEntry>();

            mockPersister = new Mock<IPersistDomainModels<LogEntry>>();
            mockPersister.Setup(p => p.Save(It.IsAny<LogEntry>())).Callback(
                (LogEntry logEntry) => savedLogs.Add(logEntry));
        }


        [Test]
        public void Should_save_all_logs()
        {
            var logger = new Logger(mockPersister.Object);
            logger.VerbosityLevel = int.MaxValue;

            LogEntry infoLog = new InfoLogEntry("TestFramework", "Information message");
            LogEntry warningLog = new WarningLogEntry("TestFramework", "Warning message");
            LogEntry errorLog = new ErrorLogEntry("TestFramework", "Error message");

            logger.WriteEntry(infoLog);
            logger.WriteEntry(warningLog);
            logger.WriteEntry(errorLog);

            savedLogs.Contains(infoLog).ShouldBeTrue();
            savedLogs.Contains(warningLog).ShouldBeTrue();
            savedLogs.Contains(errorLog).ShouldBeTrue();
        }
    }
}