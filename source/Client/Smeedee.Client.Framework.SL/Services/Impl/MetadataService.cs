using System;
using System.Linq;
using System.Windows;
using System.Windows.Browser;
using Smeedee.Client.Framework.Services;

namespace Smeedee.Client.Framework.SL.Services.Impl
{
    public class MetadataService : IMetadataService
    {
        public Uri GetDeploymentUrl()
        {
            //TODO: This is copied and pasted from the DeploymentFolderCatalog class.
            //Remove the duplication by extract this out to a utility class/method.
            string basePath = new string(
                Application.Current.Host.Source.AbsoluteUri.
                    Replace("ClientBin/", "").
                    Reverse().
                    SkipWhile(c => c != '/').
                    Reverse().ToArray());

            return new Uri(basePath);
        }
    }
}
