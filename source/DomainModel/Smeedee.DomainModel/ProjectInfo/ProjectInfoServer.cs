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

using System.Collections.Generic;
using System.Linq;


namespace Smeedee.DomainModel.ProjectInfo
{
    public class ProjectInfoServer
    {
        public virtual string Name { get; set; }
        public virtual string Url { get; set; }

        private IList<Project> internalProjects;
        public virtual IEnumerable<Project> Projects
        {
            get
            {
                return internalProjects;
            }
            set
            {
                internalProjects = value.ToList();
                foreach (Project project in value)
                {
                    project.Server = this;
                }
            }
        }

        public ProjectInfoServer()
        {
            Projects = new List<Project>();
            internalProjects = new List<Project>();
        }

        public ProjectInfoServer(string name, string url)
            : this()
        {
            Name = name;
            Url = url;
        }


        public virtual void AddProject(Project newProject)
        {
            newProject.Server = this;
            internalProjects.Add(newProject);
            Projects = internalProjects;
        }
    }
}