using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace APD.DomainModel.Framework.Logging
{
    public class LogEntry
    {
        private int id;
        public virtual string Source { get; set; }
        public virtual string Message { get; set; }
        public virtual int Severity { get; set; }
        public virtual DateTime TimeStamp { get; set; }

        public LogEntry() { }

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

    public class InfoLogEntry : LogEntry
    {
        public InfoLogEntry() 
        {
            Severity = 2;
        }

        public InfoLogEntry(string source, string message)
            : base(source, message)
        {
            this.Severity = 2;
        }
    }

    public class WarningLogEntry : LogEntry
    {
        public WarningLogEntry()
        {
            this.Severity = 1;
        }

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
}