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

#endregion

using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using APD.DomainModel.CI;
using APD.DomainModel.Framework;


namespace APD.Client.Widget.CI.SL.Repository
{
    public class CIProjectRepositoryMock : IRepository<CIServer>
    {
        private Timer timer;
        private Random rand = new Random(DateTime.Now.Second);

        List<CIProject> fakeProjects = new List<CIProject>();

        public CIProjectRepositoryMock()
        {
            var project = new CIProject("testproject");
            var latest = new Build();
            latest.Status = DomainModel.CI.BuildStatus.FinishedSuccefully;
            project.AddBuild( latest );

            fakeProjects.Add(project);


            project = new CIProject("project2");
            latest = new Build();
            latest.Status = DomainModel.CI.BuildStatus.Building;
            latest.Trigger = new CodeModifiedTrigger("jklsdf");


            project.AddBuild(latest);

            fakeProjects.Add(project);


            timer = new Timer(updateTimerCallback, null, 5000,5000);
        }


        void updateTimerCallback(object state)
        {
            fakeProjects[0].LatestBuild.Status = (DomainModel.CI.BuildStatus) rand.Next(2, 4);
        }

        public IEnumerable<CIServer> Get(Specification<CIServer>  specification)
        {
            var servers = new List<CIServer>();
            servers.Add(new CIServer("haldis", "http://haldis.samuelsen.com"));
            fakeProjects.ForEach(p => 
                servers.First().AddProject(p));
            
            return servers;
        }
    }
}
