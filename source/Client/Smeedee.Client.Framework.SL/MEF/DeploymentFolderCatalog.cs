using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Media.Animation;
using System.Xml;
using System.Xml.Linq;
using Smeedee.Client.Framework.SL.MEF.Services;

namespace Smeedee.Client.Framework.SL.MEF
{
    public class DeploymentFolderCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
    {        
        public event EventHandler<AsyncCompletedEventArgs> DownloadCompleted;
        public event EventHandler<AsyncCompletedEventArgs> MetadataDownloadCompleted;

        private int numberOfXapsDeployedOnServer, numberOfXapsDownloaded;
        private IDownloadService downloadService;
        private List<DeploymentCatalog> deploymentCatalogs = new List<DeploymentCatalog>();

        public override IQueryable<ComposablePartDefinition> Parts
        {
            get
            {
                var parts = new List<ComposablePartDefinition>();
                foreach (var catalog in deploymentCatalogs)
                    parts.AddRange(catalog.Parts);
                return parts.AsQueryable();
            }
        }

        protected IDownloadService DownloadService
        {
            get
            {
                if (downloadService == null)
                {
                    downloadService = CreateNewDownloadService();
                }
                return downloadService;
            }
        }

        public void DownloadAsync()
        {
            DownloadService.DownloadStringCompleted += (sender, e) =>
            {
                if (e.Error != null)
                {
                    OnMetadataDownloadCompleted(e);   
                }
                else
                {
                    if (!MetadataIsValid(e.Result))
                    {
                        OnMetadataDownloadCompleted(new AsyncCompletedEventArgs(
                            new Exception("Invalid format in deployment metadata"),
                            false,
                            null));
                    }
                    else
                    {
                        OnMetadataDownloadCompleted(e);

                        addAndDownloadDeploymentCatalogs(ParseMetadata(e.Result));
                    }
                }
            };

            DownloadService.DownloadStringAsync(GetDeploymentMetadataUrl());
        }

        private void addAndDownloadDeploymentCatalogs(IEnumerable<string> xapsDeployedOnServer)
        {
            numberOfXapsDeployedOnServer = xapsDeployedOnServer.Count();

            deploymentCatalogs.Clear();

            foreach (var deployedXap in xapsDeployedOnServer)
            {
                //string url = GetBaseUrl() + "ClientBin/" + deployedXap;
                var xapUri = new Uri(deployedXap, UriKind.Relative);
                Debug.WriteLine("Dwnloading: " + xapUri);
                var deploymentCatalog = new DeploymentCatalog(xapUri);
                deploymentCatalogs.Add(deploymentCatalog);
                deploymentCatalog.Changed += (o, ee) => OnChanged(ee);
                deploymentCatalog.Changing += (o, ee) => OnChanging(ee);
                deploymentCatalog.DownloadCompleted += (o, ee) => OnDownloadCompleted(ee);

                deploymentCatalog.DownloadAsync();
            }
        }

        private bool MetadataIsValid(string data)
        {
            XDocument xDoc = null;
            try
            {
                xDoc = XDocument.Parse(data);
            }
            catch (XmlException)
            {
                
            }
            
            return xDoc != null &&
                xDoc.Element("SilverlightDeployment") != null;
        }

        private IEnumerable<string> ParseMetadata(string data)
        {
            var xDoc = XDocument.Parse(data);
            var urls = from element in xDoc.Element("SilverlightDeployment").Elements()
                   select element.Value;

            // Windows 7 seems to have problem when resolving localhost to IPv6 when out
            // of browser.
            //var rewrittenUrls = urls.Select(u => u.Replace("/localhost", "/127.0.0.1"));

            return urls;
        }

        protected virtual Uri GetDeploymentMetadataUrl()
        {
            string basePath = GetBaseUrl();

            return new Uri(string.Format("{0}SilverlightDeploymentMetadata.ashx",
                               basePath)); 
        }

        private string GetBaseUrl()
        {
            var result = new string(
                Application.Current.Host.Source.AbsoluteUri.
                    Replace("ClientBin/", "").
                    Reverse().
                    SkipWhile(c => c != '/').
                    Reverse().ToArray());

            //result = result.Replace("/localhost", "/127.0.0.1");

            return result;
        }

        protected virtual IDownloadService CreateNewDownloadService()
        {
            return new HttpDownloadService(); 
        }

        protected virtual void OnMetadataDownloadCompleted(AsyncCompletedEventArgs e)
        {
            if (MetadataDownloadCompleted != null)
                MetadataDownloadCompleted(this, e);
        }

        protected virtual void OnChanging(ComposablePartCatalogChangeEventArgs e)
        {
            if (Changing != null)
                Changing(this, e);
        }

        protected virtual void OnChanged(ComposablePartCatalogChangeEventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }

        protected virtual void OnDownloadCompleted(AsyncCompletedEventArgs e)
        {
            Debug.WriteLine("Error?: " + GetErrorMsg(e));
            Debug.WriteLine("OnDownloadCompleted");
            numberOfXapsDownloaded++;

            if (DownloadCompleted != null)
                DownloadCompleted(this, e);

            if (numberOfXapsDownloaded == numberOfXapsDeployedOnServer &&
                AllCompleted != null)
            {
                Debug.WriteLine("Xaps deployed on server: " + numberOfXapsDeployedOnServer);
                Debug.WriteLine("Downloaded xaps: " + numberOfXapsDownloaded);
                AllCompleted(this, EventArgs.Empty);
            }
        }

        private string GetErrorMsg(AsyncCompletedEventArgs e)
        {
            var msg = (e.Error != null ? e.Error.ToString() : "no error");

            if (e.Error != null && e.Error.GetType() == typeof(WebException))
            {
                var webException = e.Error as WebException;
                msg += " url: " + webException.Response.ResponseUri.ToString();
            }

            return msg;
        }

        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed;
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing;
        public event EventHandler AllCompleted;
    }
}
