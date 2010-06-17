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
using System.Collections.Generic;
using System.Linq;


namespace Smeedee.DomainModel.ProjectInfo
{
    public class Task
    {
        public virtual string SystemId { get; set; }
        public virtual string Name { get; set; }
        public virtual string Status { get; set; }
        public virtual float WorkEffortEstimate { get; set; }
        public virtual Iteration Iteration { get; set; }

        private IList<WorkEffortHistoryItem> internalWorkEffortHistory;
        public virtual IEnumerable<WorkEffortHistoryItem> WorkEffortHistory
        {
            get
            {
                return internalWorkEffortHistory;
            }
            set
            {
                internalWorkEffortHistory = value.ToList();
                foreach (WorkEffortHistoryItem historyItem in value)
                {
                    historyItem.Task = this;
                }
            }
        }

        public Task()
        {
            WorkEffortHistory = new List<WorkEffortHistoryItem>();
            internalWorkEffortHistory = new List<WorkEffortHistoryItem>();
        }


        public virtual void AddWorkEffortHistoryItem(WorkEffortHistoryItem newHistoryItem)
        {
            newHistoryItem.Task = this;
            internalWorkEffortHistory.Add(newHistoryItem);
            WorkEffortHistory = internalWorkEffortHistory;
        }

        public override bool Equals(object obj)
        {
            return GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return string.Format("{0}.{1}", SystemId, (Iteration != null) ? Iteration.SystemId : string.Empty).GetHashCode();
        }
    }
}