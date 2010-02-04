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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;


namespace APD.DomainModel.CI
{
    [DataContract(IsReference = true)]
    public class CIProject : IComparable<CIProject>
    {
        private IList<Build> internalBuilds;
        public CIProject()
        {
            internalBuilds = new List<Build>();
        }

        public CIProject(string name)
            : this()
        {
            ProjectName = name;
        }

        [DataMember]
        public virtual string SystemId { get; set; }
        
        [DataMember]
        public virtual string ProjectName { get; set; }
        
        [DataMember]
        public virtual CIServer Server { get; set; }

        [DataMember]
        public virtual IEnumerable<Build> Builds 
        { 
            get
            {
                return internalBuilds;
            }
            set
            {
                internalBuilds = value.ToList();
                SetAllBuildsToProjectLinks();
            }
        }
        private void SetAllBuildsToProjectLinks()
        {
            foreach (var build in internalBuilds)
            {
                build.Project = this;
            }
        }

        public virtual void AddBuild(Build build)
        {
            build.Project = this;
            internalBuilds.Add(build);
        }
       
        public virtual int CompareTo(CIProject other)
        {
            if( LatestBuild.Status == other.LatestBuild.Status)
            {
                 return ReversedBuildCompareResult(other.LatestBuild);
            }
            if (LatestBuild.Status == BuildStatus.FinishedWithFailure)
            {
                return -1;
            }
            if (other.LatestBuild.Status == BuildStatus.FinishedWithFailure)
            {
                return 1;
            }

            return LatestBuild.Status.CompareTo(other.LatestBuild.Status);
        }

        private int ReversedBuildCompareResult(Build otherLatestBuild) {
            
            if(LatestBuild.CompareTo(otherLatestBuild)==1)
            {
                return -1;
            }

            if (LatestBuild.CompareTo(otherLatestBuild)==-1)
            {
                return 1;
            }

            return 0;
        }

        public override bool Equals(object obj)
        {
            return this.GetHashCode().Equals(obj.GetHashCode());
        }

        public override int GetHashCode()
        {
            return string.Format("{0}.{1}", SystemId, (Server != null) ? Server.Url : string.Empty).GetHashCode();
        }

        public virtual Build LatestBuild
        {
            get
            {
                if (internalBuilds.Count > 0)
                {
                    return internalBuilds.LastOrDefault();
                }
                else
                {
                    return null;
                }
            }
        }
    }
}