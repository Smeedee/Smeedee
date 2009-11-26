using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using APD.Client.Framework.SL;
using APD.Client.Widget.SourceControl.SL.ChangesetRepositoryService;
using APD.DomainModel.Framework;
using APD.DomainModel.SourceControl;


namespace APD.Client.Widget.SourceControl.SL.Repositories
{
    public class AsyncChangesetRepository : IAsyncRepository<Changeset>
    {
        ChangesetRepositoryServiceClient client = new ChangesetRepositoryServiceClient();

        public AsyncChangesetRepository()
        {
            client.Endpoint.Address = 
                WebServiceEndpointResolver.ResolveDynamicEndpointAddress(client.Endpoint.Address);

            client.GetCompleted += new EventHandler<GetCompletedEventArgs>(client_GetCompleted);
        }

        void  client_GetCompleted(object sender, GetCompletedEventArgs e)
        {
            Specification<Changeset> specification = e.UserState as Specification<Changeset>;
 	        if( GetCompleted != null )
 	            GetCompleted(this, new GetCompletedEventArgs<Changeset>(e.Result, specification));
        }

        public void BeginGet(Specification<Changeset> specification)
        {
            client.GetAsync(specification, specification);
        }

        public event EventHandler<GetCompletedEventArgs<Changeset>> GetCompleted;
    }
}
