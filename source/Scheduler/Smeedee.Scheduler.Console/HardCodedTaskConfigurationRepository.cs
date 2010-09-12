using System;
using System.Collections.Generic;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.TaskInstanceConfiguration;

namespace Smeedee.Scheduler.Console
{
    public class HardCodedTaskConfigurationRepository : IRepository<TaskConfiguration>
    {
        public IEnumerable<TaskConfiguration> Get(Specification<TaskConfiguration> specification)
        {
            // Subversion for google code:
            var svnGoogle = new TaskConfiguration
                                {
                                    Name = "Google SVN for Smeedee",
                                    TaskName = "SVN Changeset Harvester",
                                    DispatchInterval = 1000 * 60 * 1
                                };
            svnGoogle.Entries = new List<TaskConfigurationEntry>
            {
                NewConfigEntry("url", "http://smeedee.googlecode.com/svn/", typeof(string), svnGoogle),
                NewConfigEntry("username", "guest", typeof(string), svnGoogle),
                NewConfigEntry("password", "", typeof(string), svnGoogle),
                NewConfigEntry("provider", "svn", typeof(string), svnGoogle)
            };

            // Team city
            var teamCity = new TaskConfiguration
            {
                Name = "TeamCity Continuous - SoC Branch",
                TaskName = "Team City Continuous Integration Task",
                DispatchInterval = 1000 * 60 * 5
            };
            teamCity.Entries = new List<TaskConfigurationEntry>
            {
                NewConfigEntry("url", "http://smeedee.com:8111", typeof (string), teamCity),
                NewConfigEntry("username", "guest", typeof (string), teamCity),
                NewConfigEntry("password", "", typeof (string), teamCity),
                NewConfigEntry("project", "", typeof (string), teamCity)
            };

            // Burndown chart
            var burndownChart = new TaskConfiguration
            {
                Name = "Burndown Chart - ProjectInfo",
                TaskName = "TFS Project Info Harvester",
                DispatchInterval = 1000 * 60 * 5
                                        
            };
            burndownChart.Entries = new List<TaskConfigurationEntry>
            {
                NewConfigEntry("url", "http://smeedee.org", typeof (Uri), burndownChart),
                NewConfigEntry("username", "guest", typeof (string), burndownChart),
                NewConfigEntry("password", "", typeof (string), burndownChart),
                NewConfigEntry("provider", "conchango-tfs", typeof (string), burndownChart),
                NewConfigEntry("project", "", typeof (string), burndownChart)
            };

            return new List<TaskConfiguration> {svnGoogle, teamCity, burndownChart};
        }

        private TaskConfigurationEntry NewConfigEntry(string name, object value, Type type, TaskConfiguration config)
        {
            return new TaskConfigurationEntry
                       {
                           Name = name,
                           TaskConfiguration = config,
                           Type = type,
                           Value = value
                       };
        }
    }
}