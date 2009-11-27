using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using APD.DomainModel.Framework.Logging;


namespace APD.DomainModel.Framework.SL.Logging
{
    public class LogEntryPersister : IPersistDomainModels<LogEntry>
    {

        #region IPersistDomainModels<LogEntryPersister> Members

        public void Save(LogEntry domainModel)
        {

        }

        public void Save(System.Collections.Generic.IEnumerable<LogEntry> domainModels)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
