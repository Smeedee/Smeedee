using System;
using System.Collections.Generic;
using System.Linq;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.SourceControl;
using Smeedee.DomainModel.TaskInstanceConfiguration;
using Smeedee.DomainModel.Users;
using Smeedee.Tasks.Framework;
using Smeedee.Tasks.Framework.TaskAttributes;

namespace Smeedee.Tasks.SourceControl
{
    [Task("Sourcecontrol users to UserDb",
        Author = "Smeedee Team",
        Description = "This tasks takes all the users that have commited to the source control and puts them into the UserDb (username only)",
        Version = 1,
        Webpage = "http://smeedee.org")]
    [TaskSetting(1, USERDB_SETTING_NAME, typeof(string), "default")]
    public class SourceControlUserToUserDbTask : TaskBase
    {
        private const string USERDB_TO_USE = "Default";
        private const string USERDB_SETTING_NAME = "Name of user database";
        private readonly IRepository<Changeset> _changesetRepository;
        private readonly IRepository<Userdb> _userDbRepository;
        private readonly IPersistDomainModels<Userdb> _userDbPersister;
        private readonly TaskConfiguration taskConfiguration;
      

        public override string Name { get { return "Sync Changeset Users with UserDB users"; } }

        public SourceControlUserToUserDbTask(IRepository<Changeset> changesetRepository, IRepository<Userdb> userDbRepository, IPersistDomainModels<Userdb> userDbPersister, TaskConfiguration taskConfiguration)

        {
            _changesetRepository = changesetRepository;
            _userDbRepository = userDbRepository;
            _userDbPersister = userDbPersister;
            this.taskConfiguration = taskConfiguration;
            Interval = TimeSpan.FromMilliseconds(taskConfiguration.DispatchInterval);
        }

        public override void Execute()
        {
            var allUsernamesFromChangesets = GetAllUsernamesFromChangesets();
            var userdb = GetUserdb();
            SyncUsersFromVCSToUserDb(allUsernamesFromChangesets, userdb);
            _userDbPersister.Save(userdb);
        }
        
        private IEnumerable<string> GetAllUsernamesFromChangesets()
        {
            var enumerable = _changesetRepository.Get(new AllSpecification<Changeset>());
            return enumerable
                .Where(c =>  c.Author != null)
                .Select(c => c.Author.Username)
                .Distinct(StringComparer.Ordinal);
        }

        private Userdb GetUserdb()
        {
			string userDbToSyncTo = "default";
			if (taskConfiguration.EntryExists(USERDB_SETTING_NAME))
			{
				userDbToSyncTo = taskConfiguration.ReadEntryValue(USERDB_SETTING_NAME).ToString();
			}
            
            return GetOrCreateUserDb(userDbToSyncTo);
        }

        private Userdb GetOrCreateUserDb(string userDbToSyncTo)
        {
            var userDbResult = _userDbRepository.Get(new UserdbNameSpecification(userDbToSyncTo));
            if (userDbResult == null || userDbResult.Count() == 0)
            {
                var userDb = new Userdb("default");
                _userDbPersister.Save(userDb);
                return userDb;
            }
            return userDbResult.First();

        }
    
        private void SyncUsersFromVCSToUserDb(IEnumerable<string> allUsernamesFromChangesets, Userdb userdb)
        {
            foreach (var userNameFromChangeset in allUsernamesFromChangesets)
            {
                if( !userdb.Users.Any( u => u.Username.Equals(userNameFromChangeset)))
                {
                    userdb.AddUser(new User(userNameFromChangeset));
                }
            }
        }
    }
}
