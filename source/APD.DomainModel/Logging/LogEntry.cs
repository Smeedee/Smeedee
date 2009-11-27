using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APD.DomainModel.Logging
{
    public abstract class LogEntry
    {
        public virtual string Source { get; protected set; }
        public virtual string Message { get; protected set; }
        public virtual int Severity { get; protected set; }
        public virtual DateTime TimeStamp { get; set; }

        protected LogEntry() { }

        public LogEntry(string source, string message)
        {
            this.Source = source;
            this.Message = message;
            this.TimeStamp = DateTime.Now;
        }
        public LogEntry(string source, string message, DateTime timeStamp)
        {
            this.Source = source;
            this.Message = message;
            this.TimeStamp = timeStamp;
        }

        public virtual new bool Equals(object obj)
        {
            return this.GetHashCode().Equals(obj.GetHashCode());
        }

        public virtual new int GetHashCode()
        {
            return string.Format("{0}.{1}", TimeStamp, Message).GetHashCode();
        }
    }

    #region Log classes
    public class InfoLogEntry : LogEntry
    {
        public InfoLogEntry() { }

        public InfoLogEntry(string source, string message)
            : base(source, message)
        {
            this.Severity = 2;
        }
    }

    public class WarningLogEntry : LogEntry
    {
        public WarningLogEntry() { }

        public WarningLogEntry(string source, string message)
            : base(source, message)
        {
            this.Severity = 1;
        }
    }

    public class ErrorLogEntry : LogEntry
    {
        public ErrorLogEntry() { }
        public ErrorLogEntry(string source, string message)
            : base(source, message)
        {
            this.Severity = 0;
        }
    }
    #endregion
}
