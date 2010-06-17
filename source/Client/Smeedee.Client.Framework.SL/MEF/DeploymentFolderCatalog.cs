using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Browser;
using System.Xml;
using System.Xml.Linq;
using Smeedee.Client.Framework.SL.MEF.Services;

namespace Smeedee.Client.Framework.SL.MEF
{
    public class DeploymentFolderCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
    {        
        public event EventHandler<AsyncCompletedEventArgs> DownloadCompleted;
        public event EventHandler<AsyncCompletedEventArgs> MetadataDownloadCompleted;

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
            DownloadService.DownloadStringAsync(GetDeploymentMetadataUrl());
            downloadService.DownloadStringCompleted += (sender, e) =>
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
        }

        private void addAndDownloadDeploymentCatalogs(IEnumerable<string> xapsDeployedOnServer)
        {
            deploymentCatalogs.Clear();

            foreach (var deployedXap in xapsDeployedOnServer)
            {
                var deploymentCatalog = new DeploymentCatalog(deployedXap);
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
            return from element in xDoc.Element("SilverlightDeployment").Elements()
                   select element.Value;
        }

        protected virtual Uri GetDeploymentMetadataUrl()
        {
            string basePath = new string(
                HtmlPage.Document.DocumentUri.AbsoluteUri.
                    Reverse().
                    SkipWhile(c => c != '/').
                    Reverse().ToArray());

            return new Uri(string.Format("{0}SilverlightDeploymentMetadata.ashx",
                               basePath)); 
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
            if (DownloadCompleted != null)
                DownloadCompleted(this, e);
        }

        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed;
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing;
    }
}
