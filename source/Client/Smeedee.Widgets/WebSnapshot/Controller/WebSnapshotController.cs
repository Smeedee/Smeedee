using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using Smeedee.Client.Framework.Controller;
using Smeedee.Client.Framework.Services;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.WebSnapshot;
using Smeedee.Framework;
using Smeedee.Widgets.WebSnapshot.ViewModel;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widgets.WebSnapshot.Controllers
{
    public class WebSnapshotController : ControllerBase<WebSnapshotViewModel>
    {
        private WebSnapshotViewModel webSnapshotViewModel;
        private WebSnapshotSettingsViewModel webSnapshotSettingsViewModel;
        private Configuration config;
        private IRepository<DomainModel.WebSnapshot.WebSnapshot> repository;
        private ILog logger;


        public WebSnapshotController(
            WebSnapshotViewModel webSnapshotViewModel, 
            WebSnapshotSettingsViewModel webSnapshotSettingsViewModel,
            Configuration configuration,
            ITimer timer,   
            ILog logger,
            IUIInvoker uiInvoker,
            IProgressbar loadingNotifier,
            IRepository<DomainModel.WebSnapshot.WebSnapshot> repository
            ) : base(webSnapshotViewModel, timer, uiInvoker, loadingNotifier)
        {
            Guard.Requires<ArgumentNullException>(webSnapshotViewModel != null);
            Guard.Requires<ArgumentNullException>(webSnapshotSettingsViewModel != null);
            Guard.Requires<ArgumentNullException>(configuration != null);

            config = configuration;
            this.webSnapshotViewModel = webSnapshotViewModel;
            this.logger = logger;
            this.webSnapshotSettingsViewModel = webSnapshotSettingsViewModel;
            this.repository = repository;

            Start();
            LoadData();
        }

        private void LoadData()
        {
            logger.WriteEntry(new LogEntry("WebSnapshotController", "LoadData() called"));
            uiInvoker.Invoke(() =>
                                 {
                                     SetIsLoadingData();

                                     try
                                     {
                                         logger.WriteEntry(new LogEntry("WebSnapshotController", "Tries to get snapshot from repository"));
                                         var specification = new WebSnapshotSpecification();
                                         logger.WriteEntry(new LogEntry("Specification", "I maded the specification"+specification.ToString()));
                                         //var snapshot = repository.Get(new WebSnapshotSpecification());
                                         //var snapshot = repository.Get(specification);
                                         //UpdateViewModel(snapshot);
                                     }
                                     catch (Exception e)
                                     {

                                         logger.WriteEntry(new LogEntry("WebSnapshotWidget", e.ToString()));
                                     }

                                     SetIsNotLoadingData();
                                 });
   
        }

        private void UpdateViewModel(IEnumerable<DomainModel.WebSnapshot.WebSnapshot> snapshots)
        {
           if (snapshots.Count() > 0)
           {
               var snapshot = snapshots.First();
               uiInvoker.Invoke(() =>
                                    {
                                        var snapshotPath = snapshot.PictureFilePath;
                                        SetWebSnapshot(snapshotPath);
                                    });
           }
            else
           {
               uiInvoker.Invoke( () => webSnapshotViewModel.HasStoredImage = false);
           }
        }

        private void SetWebSnapshot(string imagePath)
        {
            webSnapshotViewModel.Snapshot = imagePath;
            webSnapshotViewModel.HasStoredImage = imagePath != null;
        }

        public static Configuration GetDefaultConfiguration()
        {
            var config = new Configuration("websnapshot");

            return config;
        }

        //public void UpdateConfiguration(Configuration config)
        //{
        //    Guard.Requires<ArgumentNullException>(config != null);
        //    Guard.Requires<ArgumentException>(config.ContainsSetting(url));
        //    Guard.Requires<ArgumentException>(config.ContainsSetting(refresh_interval));

        //    this.config = config;

        //    webSnapshotSettingsViewModel.InputUrl = config.GetSetting(url).Value;
        //}

        //public Configuration SaveConfiguration()
        //{
        //    config.ChangeSetting(url, webSnapshotSettingsViewModel.InputUrl);
        //    config.ChangeSetting(refresh_interval, webSnapshotSettingsViewModel.RefreshInterval.ToString());

        //    return config;
        //}

        protected override void OnNotifiedToRefresh(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
