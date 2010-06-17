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
// The project webpage is located at http://smeedee.org/
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Jan Fredrik Drabløs</author>
// <email>jaffe88@hotmail.com</email>
// <date>2009-08-13</date>

#endregion

using System;


namespace Smeedee.DomainModel.ProjectInfo
{
    public class WorkEffortHistoryItem : IComparable<WorkEffortHistoryItem>
    {
        public virtual Task Task { get; set; }
        public virtual float RemainingWorkEffort { get; set; }
        public virtual DateTime TimeStampForUpdate { get; set; }

        public WorkEffortHistoryItem() { }

        public WorkEffortHistoryItem(float remainingWorkEffort, DateTime timeStampForUpdate)
        {
            RemainingWorkEffort = remainingWorkEffort;
            TimeStampForUpdate = timeStampForUpdate;
        }

        public virtual int CompareTo(WorkEffortHistoryItem other)
        {
            return TimeStampForUpdate.CompareTo(other.TimeStampForUpdate);
        }

        public override bool Equals(object obj)
        {   
            return GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return string.Format("{0}|{1}", TimeStampForUpdate, Task.SystemId).GetHashCode();
        }
    }
}