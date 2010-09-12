using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Collections.Generic;
using Smeedee.Client.Framework.SL.MEF;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Framework;

namespace Smeedee.Client.Framework.SL.ViewModel.Repositories
{
    public class WidgetMetadataRepository : IAsyncRepository<WidgetMetadata>
    {
        public DeploymentFolderCatalog deploymentFolderCatalog;
        private Specification<WidgetMetadata> specification;
        [ImportMany(AllowRecomposition = true)]
        public IEnumerable<Lazy<Widget, IWidgetMetadata>> widgets = new List<Lazy<Widget, IWidgetMetadata>>();
        private IEnumerable<WidgetMetadata> availableWidgets;

        public static WidgetMetadataRepository Instance = new WidgetMetadataRepository();
        private bool isGetting = false;

        private WidgetMetadataRepository()
        {
            deploymentFolderCatalog = new DeploymentFolderCatalog();
            CompositionHost.Initialize(deploymentFolderCatalog);
            deploymentFolderCatalog.AllCompleted += new EventHandler(deploymentFolderCatalog_AllCompleted);
        }

        void deploymentFolderCatalog_AllCompleted(object sender, EventArgs e)
        {
            isGetting = false;
            CompositionInitializer.SatisfyImports(this);
            ExtractWidgetMetadataViewModels();
            TriggerGetCompleted(specification);
        }

        private void ExtractWidgetMetadataViewModels()
        {
            availableWidgets = (from widgetMetadata in widgets
                   select new WidgetMetadata()
                       {
                           Name = widgetMetadata.Metadata.Name,
                           UserSelectedTitle=widgetMetadata.Metadata.Name,
                           Description = widgetMetadata.Metadata.Description,
                           Author = widgetMetadata.Metadata.Author,
                           Version = widgetMetadata.Metadata.Version,
                           Tags = widgetMetadata.Metadata.Tags,
                           IsDescriptionCapped = true,
                           Type = widgetMetadata.Value.GetType()
                       }).ToList();
        }

        public void BeginGet(Specification<WidgetMetadata> specification)
        {
            if (isGetting)
                return;
            
            this.specification = specification;

            if (availableWidgets == null)
            {
                deploymentFolderCatalog.DownloadAsync();
                isGetting = true;
            }
            else
            {
                ExtractWidgetMetadataViewModels();
                TriggerGetCompleted(specification);
            }
        }

        private void TriggerGetCompleted(Specification<WidgetMetadata> specification)
        {
            if (GetCompleted != null)
                GetCompleted(this, 
                    new GetCompletedEventArgs<WidgetMetadata>(availableWidgets, specification));
        }

        public event EventHandler<GetCompletedEventArgs<WidgetMetadata>> GetCompleted;
    }
}
