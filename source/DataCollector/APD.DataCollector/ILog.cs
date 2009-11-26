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

using System;


namespace APD.DataCollector
{
    public abstract class LogEntry
    {
        public virtual string Source { get; protected set; }
        public virtual string Message { get; protected set; }
        public virtual int Severity { get; protected set; }
        public virtual DateTime TimeStamp { get; set; }

        protected LogEntry(){}

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
    public class InfoLogEntry :LogEntry
    {
        public InfoLogEntry(){}

        public InfoLogEntry(string source, string message) : base(source,message)
        {
            this.Severity = 2;
        }
    }

    public class WarningLogEntry :LogEntry
    {
        public WarningLogEntry() {}

        public WarningLogEntry(string source, string message) : base(source,message)
        {
            this.Severity = 1;
        }
    }

    public class ErrorLogEntry :LogEntry
    {
        public ErrorLogEntry(){}
        public ErrorLogEntry(string source, string message) : base(source,message)
        {
            this.Severity = 0;
        }
    }
    #endregion

    public interface ILog
    {
        void WriteEntry(LogEntry entry);

        int VerbosityLevel { get; set; }
        
    }

}
