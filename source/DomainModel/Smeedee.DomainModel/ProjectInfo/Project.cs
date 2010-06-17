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
    public class Project
    {
        private DateTime lastCurrentIterationSetCall = DateTime.MinValue;
        private Iteration currentIteration;
        public virtual string Name { get; set; }
        public virtual string SystemId { get; set; }
        public virtual ProjectInfoServer Server { get; set; }

        private IList<Iteration> internalIterations;
        public virtual IEnumerable<Iteration> Iterations
        {
            get
            {
                return internalIterations;
            }
            set
            {
                internalIterations = value.ToList();
                foreach (Iteration iteration in value)
                {
                    iteration.Project = this;
                }
            }
        }

        public virtual Iteration CurrentIteration 
        { 
            get
            {
                if (currentIteration == null || DateTime.Now.Date != lastCurrentIterationSetCall)
                    SetCurrentIteration();

                return currentIteration;
            }
        }

        public Project()
        {
            Iterations = new List<Iteration>();
            internalIterations = new List<Iteration>();
        }

        public Project(string name)
            : this()
        {
            Name = name;
        }


        public virtual void AddIteration(Iteration newIteration)
        {
            newIteration.Project = this;
            internalIterations.Add(newIteration);
            Iterations = internalIterations;
        }

        public virtual int GetIterationCount()
        {
            return internalIterations.Count;
        }
        
        private void SetCurrentIteration()
        {
            foreach (Iteration iteration in Iterations)
            {
                bool afterStartDate = iteration.StartDate.CompareTo(DateTime.Now) < 0;
                bool beforeEndDate = (iteration.EndDate.CompareTo(DateTime.Now) > 0);

                if (afterStartDate && beforeEndDate)
                    currentIteration = iteration;
            }
            if (currentIteration == null && internalIterations.Count > 0)
                currentIteration = GetPreviousIteration();

            lastCurrentIterationSetCall = DateTime.Now.Date;
        }

        private Iteration GetPreviousIteration()
        {
            Iteration lastIteration = internalIterations[0];
            bool noPreviousIterations = true;
            foreach (Iteration iteration in Iterations)
            {
                if (iteration.EndDate.CompareTo(DateTime.Now) < 0)
                    noPreviousIterations = false;
                if (iteration.EndDate.CompareTo(DateTime.Now) < 0 && iteration.EndDate.CompareTo(lastIteration.EndDate) > 0)
                    lastIteration = iteration;
            }
            if (noPreviousIterations)
                foreach (Iteration iteration in Iterations)
                    if (iteration.StartDate.CompareTo(lastIteration.StartDate) < 0)
                        lastIteration = iteration;

            return lastIteration;
        }

        public override bool Equals(object obj)
        {
            return GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return string.Format("{0}.{1}", SystemId, (Server != null) ? Server.Url : string.Empty).GetHashCode();

        }
    }
}