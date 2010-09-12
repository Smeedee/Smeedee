using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Smeedee.Client.Framework.SL.LogEntryRepositoryService;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;

namespace Smeedee.Client.Framework.SL.Repositories
{
    [Export(typeof(IPersistDomainModels<LogEntry>))]
    [Export(typeof(IRepository<LogEntry>))]
    public class LogEntryWebserviceRepository : IPersistDomainModels<LogEntry>, IRepository<LogEntry>
    {

        private ManualResetEvent resetEvent = new ManualResetEvent(false);
        private List<LogEntry> logEntries = new List<LogEntry>();
        LogEntryRepositoryServiceClient client = new LogEntryRepositoryServiceClient();
        private Exception invocationException;

        public LogEntryWebserviceRepository()
        {
            client.Endpoint.Address = 
                WebserviceEndpointResolver.ResolveDynamicEndpointAddress(client.Endpoint.Address);
            client.GetCompleted += new EventHandler<GetCompletedEventArgs>(client_GetCompleted);
        }

        void client_GetCompleted(object sender, GetCompletedEventArgs e)
        {
            var resultingHolidays = e.Result;

            if (resultingHolidays != null)
            {
                this.logEntries = resultingHolidays;
            }

            invocationException = e.Error;

            resetEvent.Set();
        }
        
        #region IPersistDomainModels<LogEntryPersister> Members

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

        #endregion

        public IEnumerable<LogEntry> Get(Specification<LogEntry> specification)
        {
            client.GetAsync(specification);
            resetEvent.Reset();
            resetEvent.WaitOne();

            if (invocationException != null)
                throw invocationException;

            return logEntries;
        }
    }
}
