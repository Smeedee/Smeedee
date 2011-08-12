using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.TaskInstanceConfiguration;
using Smeedee.Framework;
using Smeedee.Tasks.Framework;
using Smeedee.Tasks.Framework.TaskAttributes;
using Smeedee.Widgets.WebSnapshot.Util;

namespace Smeedee.Tasks.WebSnapshot
{
    [Task("WebSnapshot Task",
        Author = "Smeedee Team",
        Description = "Takes snapshots of web pages or downloads images. Provides images for the Web Snapshot Widget. " +
                      "An current limitation is probably due to Javascript-heavy sites, which results in a blank snapshot.",
        Version = 1,
        Webpage = "http://smeedee.org")]
    [TaskSetting(1, WEBPAGE, typeof(string), "", @"URL to webpage for snapshotting or image for downloading. You can also use file:// or C:\ to find an image on a local disk")]
    [TaskSetting(2, XPATH, typeof(string), "", "Optional XPath expression, that matches an image tag on the given URL. This will download the image instead of snapshotting the entire page. Best results with simpler expressions that doesn't use // or @")]
    [TaskSetting(3, SMEEDEEPATH, typeof(string), @"C:\Program Files\Smeedee", "Path to Smeedee base directory, where snapshots will be stored in order to be accessible on the server")]
    

    public class WebSnapshotTask : TaskBase
    {
        private TaskConfiguration config;
        private WebImageFetcher webImageFetcher;
        private string filename;
        private string filePath;
        private IPersistDomainModels<Smeedee.DomainModel.WebSnapshot.WebSnapshot> databasepersister;

        private string lastWebpageValue;
        private string lastXpathValue;
        private string lastConfigName;
        private string lastPathName;
        private string folderPath;


        public WebSnapshotTask(TaskConfiguration config,  IPersistDomainModels<Smeedee.DomainModel.WebSnapshot.WebSnapshot> databasePersister)
        {
            this.config = config;
            this.databasepersister = databasePersister;
            UpdateConfigValues();
            
            Guard.Requires<TaskConfigurationException>(lastWebpageValue != null);
            Guard.Requires<TaskConfigurationException>(URLValidator.IsValidUrl(lastWebpageValue));
            Guard.Requires<TaskConfigurationException>(lastPathName != null);

            webImageFetcher = new WebImageFetcher(new WebImageProvider());
            filename = GenerateFilename() + ".png";
            folderPath = Path.Combine(lastPathName, "WebSnapshots");
            filePath = Path.Combine(folderPath, filename);

            Interval = TimeSpan.FromMilliseconds(config.DispatchInterval);
        }

        public const string WEBPAGE = "Webpage or Image URL";
        public const string XPATH = "Optional XPath expression";
        public const string SMEEDEEPATH = "Path to Smeedee install";

        public override string Name
        {
            get { return "WebSnapshot Task"; }
            set { }
        }

        public override void Execute()
        {
            Bitmap picture = FetchPicture(
                (config.ReadEntryValue(WEBPAGE) as string),
                (config.ReadEntryValue(XPATH) as string));

            if (picture == null)
                return;

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            picture.Save(filePath, ImageFormat.Png);

            var model = new DomainModel.WebSnapshot.WebSnapshot
                            {
                                Name = config.Name,
                                PictureFilePath = filePath,
                                PictureHeight = picture.Height,
                                PictureWidth = picture.Width,
                                Timestamp = GetCurrentTimeStamp(),
                            };

            databasepersister.Save(model);
            UpdateConfigValues();
        }

        private Bitmap FetchPicture(string url, string xpath)
        {
            if (xpath.Length == 0)
            {
                return webImageFetcher.GetBitmapFromURL(url);
            }

            return webImageFetcher.GetBitmapFromURL(url,xpath);

        }

        private void UpdateConfigValues()
        {
            lastWebpageValue = config.ReadEntryValue(WEBPAGE) as string;
            lastXpathValue = config.ReadEntryValue(XPATH) as string;
            lastConfigName = config.Name;
            lastPathName = config.ReadEntryValue(SMEEDEEPATH) as string;
        }

        private string GetCurrentTimeStamp()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssffff");
        }

        public string GenerateFilename()
        {
            return Regex.Replace(config.Name, @"[^a-zA-Z_0-9]", string.Empty) + "-" +
                Regex.Replace(config.ReadEntryValue(WEBPAGE) as string, @"[^a-zA-Z_0-9]", string.Empty);
        }


    }
}
