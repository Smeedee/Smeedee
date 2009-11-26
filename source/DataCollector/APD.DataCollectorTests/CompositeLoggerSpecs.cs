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
using APD.DataCollector;
using Moq;
using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace APD.DataCollectorTests.CompositeLoggerSpecs
{
    [TestFixture]
    public class CompositeLoggerSpecs
    {
        private static CompositeLogger logger;
        private static Mock<ILog> mockLogger1;
        private static Mock<ILog> mockLogger2;
        private static LogEntry savedLog1;
        private static LogEntry savedLog2;
        private static LogEntry logMessage;

        protected Context a_CompositeLogger_is_created = () =>
        {
            logger = new CompositeLogger();
        };

        protected When a_log_message_is_written = () =>
        {
            logMessage = new InfoLogEntry("Test Framework", "Information Message");
            logger.WriteEntry(logMessage);
        };

        protected Then check_that_all_loggers_saved_the_message = () =>
        {
            logMessage.ShouldBe(savedLog1);
            logMessage.ShouldBe(savedLog2);
        };

        [SetUp]
        public void Setup()
        {
            mockLogger1 = new Mock<ILog>();
            mockLogger2 = new Mock<ILog>();
            savedLog1 = null;
            savedLog2 = null;
            logger = null;

            mockLogger1.Setup(l => l.WriteEntry(It.IsAny<LogEntry>())).Callback(
                (LogEntry logEntry) => savedLog1 = logEntry );
            mockLogger2.Setup(l => l.WriteEntry(It.IsAny<LogEntry>())).Callback(
                (LogEntry logEntry) => savedLog2 = logEntry);
        }

        [Test]
        public void assure_implements_ILog_interface()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_CompositeLogger_is_created);
                scenario.When("the instance is checked");
                scenario.Then("it implements ILog", () => logger.ShouldBeInstanceOfType<ILog>());
            });
            
        }

        [Test]
        public void assure_will_log_to_all_registed_loggers()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_CompositeLogger_is_created).
                    And("two loggers registers", () =>
                    {
                        logger.RegisterLogger(mockLogger1.Object);
                        logger.RegisterLogger(mockLogger2.Object);
                    });
                scenario.When(a_log_message_is_written);
                scenario.Then(check_that_all_loggers_saved_the_message);
            });
        }

        [Test]
        public void assure_will_register_loggers_via_constructor()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given("Two loggers register via the constructor", () =>
                {
                    logger = new CompositeLogger(new List<ILog>{ mockLogger1.Object, mockLogger2.Object});
                });
                scenario.When(a_log_message_is_written);
                scenario.Then(check_that_all_loggers_saved_the_message);
            });
        }

        [Test]
        public void assure_will_register_multiple_loggers_at_once()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_CompositeLogger_is_created).
                    And("Two loggers registers as a list", () =>
                        logger.RegisterLoggers(new List<ILog> { mockLogger1.Object, mockLogger2.Object })
                        );
                scenario.When(a_log_message_is_written);
                scenario.Then(check_that_all_loggers_saved_the_message);
            });
        }
    }
}