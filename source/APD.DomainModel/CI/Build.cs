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
// /copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.Runtime.Serialization;


namespace APD.DomainModel.CI
{
    /// <summary>
    /// Don't change the numbers!
    /// 
    /// <para>
    /// If you change this enumeration, you must change the referred
    /// enumeration as well.
    /// </para>
    /// <seealso cref="APD.Client.Widget.CI.BuildStatus"/>
    /// </summary>
    public enum BuildStatus
    {
        Unknown = 0,
        Building = 1,
        FinishedWithFailure = 2,
        FinishedSuccefully = 3
    }

    [DataContract]
    public class Build : IComparable<Build>
    {
        [DataMember]
        public virtual CIProject Project { get; set; }
        
        [DataMember]
        public virtual Trigger Trigger { get; set; }
        
        [DataMember]
        public virtual BuildStatus Status { get; set; }
        
        [DataMember]
        public virtual DateTime StartTime { get; set; }
        
        [DataMember]
        public virtual DateTime FinishedTime { get; set; }
        
        [DataMember]
        public virtual string SystemId { get; set; }

        public virtual TimeSpan Duration
        {
            get
            {
                if (Status == BuildStatus.FinishedSuccefully || 
                    Status == BuildStatus.FinishedWithFailure)
                {
                    return FinishedTime - StartTime;
                }
                else if (Status == BuildStatus.Building)
                {
                    return DateTime.Now - StartTime;
                }
                else
                {
                    return new TimeSpan();
                }
            }
        }

        public Build()
        {
            Trigger = new UnknownTrigger();
        }

        public override bool Equals(object obj)
        {
            return this.GetHashCode().Equals(obj.GetHashCode());
        }

        public override int GetHashCode()
        {
            return string.Format("{0}.{1}", SystemId,
                (Project != null) ? Project.SystemId : string.Empty).GetHashCode();
        }


        #region IComparable<Build> Members

        public virtual int CompareTo(Build other)
        {
            return StartTime.CompareTo(other.StartTime);
        }

        #endregion
    }
}