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
// The project webpage is located at http://smeedee.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Smeedee.DomainModel.CI
{
    [DataContract(IsReference = true)]
    public class CIServer
    {
        [DataMember]
        public virtual string Name { get; set; }
        
        [DataMember]
        public virtual String Url { get; set; }
        private IList<CIProject> internalProjects;

        [DataMember]
        public virtual IEnumerable<CIProject> Projects
        {
            get
            {
                return internalProjects;
            }
            set
            {
                internalProjects = value.ToList();
                foreach (CIProject project in value)
                {
                    project.Server = this;
                }
            }
        }

        public CIServer()
        {
            internalProjects = new List<CIProject>();
        }

        public CIServer(string name, string url)
            : this()
        {
            Name = name;
            Url = url;
        }


        public virtual void AddProject(CIProject newProject)
        {
            if (newProject == null)
                throw new ArgumentNullException("newProject");

            newProject.Server = this;
            internalProjects.Add(newProject);
        }
    }
}