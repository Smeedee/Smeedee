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
using System.Linq;

using APD.Client.Framework.SL;
using APD.Client.Widget.CI.SL.CIRepositoryService;
using APD.Client.Widget.CI.SL.Repository;
using APD.DomainModel.CI;
using APD.DomainModel.Framework;


namespace APD.Client.Widget.CI.SL.Repository
{
    public class CIProjectRepository : IRepository<CIServer>
    {
        private ManualResetEvent resetEvent = new ManualResetEvent(false);
        private List<CIServer> ciServers;
        private CIRepositoryServiceClient client;

        private Exception invocationException;

        public CIProjectRepository()
        {
            ciServers = new List<CIServer>();

            client = new CIRepositoryServiceClient();
            client.Endpoint.Address =
                WebServiceEndpointResolver.ResolveDynamicEndpointAddress(client.Endpoint.Address);
            client.GetCompleted += new EventHandler<GetCompletedEventArgs>(Client_GetCompleted);
        }

        #region ICIProjectRepository Members

        public IEnumerable<CIServer> Get(Specification<CIServer> specification)
        {
            client.GetAsync(new AllSpecification<CIServer>());
            resetEvent.Reset();
            resetEvent.WaitOne();

            if (invocationException != null)
                throw invocationException;

            var servers = ciServers;
            return servers;
        }

        private void Client_GetCompleted(object sender, GetCompletedEventArgs e)
        {

            var qServers = e.Result;

            if (qServers != null && qServers.Count() > 0)
            {
                ciServers = qServers;
            }

            invocationException = e.Error;
            
            resetEvent.Set();
        }

        #endregion
    }
}