using System;
using System.Net;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Smeedee.Client.Framework.SL
{
    public class WebserviceEndpointResolver
    {
        public static EndpointAddress ResolveDynamicEndpointAddress(EndpointAddress configuredEndpoint)
        {
            Uri appUri = Application.Current.Host.Source;
            string virtualPath = Regex.Match(appUri.OriginalString,
                                 "http[s]?://[^/]+?/(?<VirtualPath>[^/]+)").Groups["VirtualPath"].Value;

            string serviceUrl = string.Format("{0}://{1}:{2}{4}",
                appUri.Scheme, appUri.Host, appUri.Port, virtualPath, configuredEndpoint.Uri.AbsolutePath)
            ;

            return new EndpointAddress(serviceUrl);

        }
    }
}
