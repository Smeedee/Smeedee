using System;
using System.Linq;
using System.Windows.Browser;
using Smeedee.Client.Framework.Services;

namespace Smeedee.Client.Framework.SL.Services.Impl
{
    public class MetadataService : IMetadataService
    {
        public Uri GetDeploymentUrl()
        {
            return new Uri (
                new string(
                    HtmlPage.Document.DocumentUri.AbsoluteUri.
                        Reverse().
                        SkipWhile(c => c != '/').
                        Reverse().ToArray()));
        }
    }
}
