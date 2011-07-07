using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smeedee.Client.Framework.Services;
using Smeedee.DomainModel.Config;
using Smeedee.Framework;
using Smeedee.Widgets.WebSnapshot.ViewModel;

namespace Smeedee.Widgets.WebSnapshot.Controllers
{
    public class WebSnapshotController
    {
        private WebSnapshotViewModel webSnapshotViewModel;
        private Configuration config;
        private ITimer timer;

        private const string refresh_interval = "refresh-interval";
        private const string url = "url";

        public WebSnapshotController(WebSnapshotViewModel webSnapshotViewModel, Configuration configuration, ITimer timer)
        {
            Guard.Requires<ArgumentNullException>(webSnapshotViewModel != null);
            Guard.Requires<ArgumentNullException>(configuration != null);
            Guard.Requires<ArgumentNullException>(timer != null);

            this.config = configuration;

            this.webSnapshotViewModel = webSnapshotViewModel;
            

            this.timer = timer;
        }

        public static Configuration GetDefaultConfiguration()
        {
            var config = new Configuration("websnapshot");

            config.NewSetting(url, "");
            config.NewSetting(refresh_interval, "30");

            return config;
        }
    }
}
