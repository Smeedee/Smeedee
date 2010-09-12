using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;


namespace Smeedee.DomainModel.Framework.Logging
{
    [DataContract]
    public class LogEntry
    {
        private int id;

        [DataMember]
        public virtual string Source { get; set; }

        [DataMember]
        public virtual string Message { get; set; }

        [DataMember]
        public virtual int Severity { get; set; }

        [DataMember]
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

    [DataContract]
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

    [DataContract]
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

    [DataContract]
    public class ErrorLogEntry : LogEntry
    {
        public ErrorLogEntry() { }
        public ErrorLogEntry(string source, string message)
            : base(source, message)
        {
            this.Severity = 0;
        }

        public static ErrorLogEntry Create(string source, string message)
        {
            return new ErrorLogEntry()
            {
                Source = source,
                TimeStamp = DateTime.Now,
                Message = message
            };
        }

        public static ErrorLogEntry Create(object source, string message)
        {
            return Create(source.GetType().ToString(), message);
        }
    }
}