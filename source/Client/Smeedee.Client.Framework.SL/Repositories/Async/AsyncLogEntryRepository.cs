using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Smeedee.Client.Framework.SL.LogEntryRepositoryService;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;

namespace Smeedee.Client.Framework.SL.Repositories.Async
{
    public class AsyncLogEntryRepository : IAsyncRepository<LogEntry>, IPersistDomainModelsAsync<LogEntry>
    {
        private readonly LogEntryRepositoryServiceClient client;

        public AsyncLogEntryRepository()
        {
            client = new LogEntryRepositoryServiceClient();
            client.Endpoint.Address =
                WebserviceEndpointResolver.ResolveDynamicEndpointAddress(client.Endpoint.Address);
            
            client.GetCompleted += Client_GetCompleted;
            client.LogCompleted += Client_LogCompleted;
            
        }

        #region IAsyncRepository<LogEntry> Members
        private void Client_GetCompleted(object sender, GetCompletedEventArgs e)
        {
            if( GetCompleted != null )
            {
                var specificationUsed = e.UserState as Specification<LogEntry>;

                GetCompletedEventArgs<LogEntry> eventArgs = null;
                if (e.Error != null)
                    eventArgs = new GetCompletedEventArgs<LogEntry>(specificationUsed, e.Error);
                else
                    eventArgs = new GetCompletedEventArgs<LogEntry>(e.Result, specificationUsed);
                
                GetCompleted(this, eventArgs);
            }
        }

        public void BeginGet(Specification<LogEntry> specification)
        {
            client.GetAsync(specification, specification);
        }

        public event EventHandler<GetCompletedEventArgs<LogEntry>> GetCompleted;

        #endregion

        #region IPersistDomainModelsAsync<LogEntry> Members

        private void Client_LogCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (SaveCompleted != null)
            {
                var eventArgs = new SaveCompletedEventArgs(e.Error);
                SaveCompleted(this, eventArgs);
            }
        }

        public void Save(LogEntry domainModel)
        {
            client.LogAsync(domainModel);
        }
        
        public void Save(IEnumerable<LogEntry> domainModels)
        {
            //Have to convert to the base class LogEntry, because serializing subclasses fail (ErrorLogEntry, etc)
            var entries = from d in domainModels
                                 select new LogEntry()
                                            {
                                                Message = d.Message, 
                                                Severity = d.Severity, 
                                                Source = d.Source, 
                                                TimeStamp = d.TimeStamp
                                            };
            client.LogAllAsync(entries.ToList());
        }

        public event EventHandler<SaveCompletedEventArgs> SaveCompleted;

        #endregion
    }
}
