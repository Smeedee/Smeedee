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
using APD.Client.Framework.SL.VCSCredentialsService;
using APD.DomainModel.Framework.Services;
using System.Threading;

namespace APD.Client.Widget.Admin.SL.Services
{
    public class VCSCredentialsCheckerProxy : ICheckIfCredentialsIsValid
    {
        private VCSCredentialsServiceClient serviceClient;
        private ManualResetEvent resetEvent;
        private bool result;

        public VCSCredentialsCheckerProxy()
        {
            resetEvent = new ManualResetEvent(false);

            serviceClient = new VCSCredentialsServiceClient("CustomBinding_VCSCredentialsService");
            serviceClient.Endpoint.Address =
                WebServiceEndpointResolver.ResolveDynamicEndpointAddress(serviceClient.Endpoint.Address);

            serviceClient.CheckCompleted += (o, e) =>
            {
                result = e.Result;
                resetEvent.Set();
            };
        }

        public bool Check(string provider, string url, string username, string password)
        {
            resetEvent.Reset();
            serviceClient.CheckAsync(provider, url, username, password);
            resetEvent.WaitOne();

            return result;
        }
    }
}
