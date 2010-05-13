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
using System.Threading;
using APD.Client.Framework.SL;
using APD.Client.Framework.SL.ProjectInfoRepositoryService;
using APD.DomainModel.Framework;
using APD.DomainModel.ProjectInfo;

using ProjectInfoServer = APD.DomainModel.ProjectInfo.ProjectInfoServer;


namespace APD.Client.Widget.BurndownChart.SL.Repositories
{
    public class ProjectInfoRepository : IRepository<ProjectInfoServer>
    {
        private ProjectInfoRepositoryServiceClient client;
        private ManualResetEvent resetEvent;
        private IEnumerable<ProjectInfoServer> servers;

        private Exception invocationException;

        public ProjectInfoRepository()
        {
            client = new ProjectInfoRepositoryServiceClient();
            client.Endpoint.Address =
                WebServiceEndpointResolver.ResolveDynamicEndpointAddress(client.Endpoint.Address);
            resetEvent = new ManualResetEvent(false);
            servers = new List<ProjectInfoServer>();

            client.GetCompleted += (sender, eventArgs) =>
            {
                invocationException = eventArgs.Error;
                servers = eventArgs.Result;
                resetEvent.Set();
            };
        }

        public IEnumerable<ProjectInfoServer> Get(Specification<ProjectInfoServer> specification)
        {
            client.GetAsync(specification);
            resetEvent.Reset();
            resetEvent.WaitOne();

            if(invocationException != null)
                throw invocationException;

            return servers;
        }
    }
}