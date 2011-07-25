using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Smeedee.Widgets.SL.TeamPicture.ViewModel;
using Smeedee.Widgets.WebSnapshot.ViewModel;
using TinyMVVM.Framework.Services.Impl;

namespace Smeedee.Widgets.WebSnapshot.Controllers
{
    public class WebSnapshotController : ControllerBase<WebSnapshotViewModel>
    {
        private WebSnapshotViewModel webSnapshotViewModel;
        private WebSnapshotSettingsViewModel webSnapshotSettingsViewModel;
        private Configuration config;
        private IRepository<Smeedee.DomainModel.WebSnapshot.WebSnapshot> repository;
        private ILog logger;


        public WebSnapshotController(
            WebSnapshotViewModel webSnapshotViewModel, 
            WebSnapshotSettingsViewModel webSnapshotSettingsViewModel,
            Configuration configuration,
            ITimer timer,   
            ILog logger,
            UIInvoker uiInvoker,
            IProgressbar loadingNotifier,
            IRepository<Smeedee.DomainModel.WebSnapshot.WebSnapshot> repository
            ) : base(webSnapshotViewModel, timer, uiInvoker, loadingNotifier)
        {
            Guard.Requires<ArgumentNullException>(webSnapshotViewModel != null);
            Guard.Requires<ArgumentNullException>(webSnapshotSettingsViewModel != null);
            Guard.Requires<ArgumentNullException>(configuration != null);

            config = configuration;
            this.webSnapshotViewModel = webSnapshotViewModel;
            this.logger = logger;
            this.webSnapshotSettingsViewModel = webSnapshotSettingsViewModel;
            webSnapshotSettingsViewModel.Reset.ExecuteDelegate += OnReset;
            //this.repository = repository;

            Start();
            LoadData();
        }

        private void OnReset()
        {
            uiInvoker.Invoke(() =>
                                 {
                                     //TODO Get SelectedImage and set it as Image
                                     webSnapshotSettingsViewModel.Image = new BitmapImage(new Uri("http://commentisfree.guardian.co.uk/strawberry.jpg"));
                                 });
        }

        private void LoadData()
        {
         uiInvoker.Invoke(() =>
                              {
                                  SetIsLoadingData();

                                  try
                                  {
                                      //var snapshot = repository.Get(new WebSnapshotSpecification());
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
