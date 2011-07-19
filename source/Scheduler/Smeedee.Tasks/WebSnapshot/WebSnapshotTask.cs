using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.TaskInstanceConfiguration;
using Smeedee.Tasks.Framework;
using Smeedee.Tasks.Framework.TaskAttributes;
using Smeedee.Widgets.WebSnapshot.Util;

namespace Smeedee.Tasks.WebSnapshot
{
    [Task("WebSnapshot Task",
        Author = "Smeedee Team",
        Description = "Takes snapshots of web pages, recommended update interval is no less than 10 minutes as it is an expensive operation",
        Version = 1,
        Webpage = "http://smeedee.org")]
    [TaskSetting(1, WEBPAGE, typeof(Uri), "http://", "Webpage or image URL")]
    [TaskSetting(2, XPATH, typeof(string), "", "XPath expression to an image tag")]

    public class WebSnapshotTask : TaskBase
    {
        private TaskConfiguration config;
        private WebImageFetcher webImageFetcher;
        private IPersistDomainModels<Smeedee.DomainModel.WebSnapshot.WebSnapshot> databasepersister;

        public WebSnapshotTask(TaskConfiguration config,  IPersistDomainModels<Smeedee.DomainModel.WebSnapshot.WebSnapshot> databasePersister)
        {
            this.config = config;
            this.databasepersister = databasePersister;
            webImageFetcher = new WebImageFetcher(new WebImageProvider());
        }

        public const string WEBPAGE = "Webpage URL";
        public const string XPATH = "Optional XPath expression";

        public override string Name
        {
            get { return "WebSnapshot Task"; }
            set { }
        }

        public override void Execute()
        {
            Bitmap picture = null;
            if ((config.ReadEntryValue(XPATH) as string).Length == 0)
            {
                picture = webImageFetcher.GetBitmapFromURL(config.ReadEntryValue(WEBPAGE) as string);
            }
            else
            {
                picture = webImageFetcher.GetBitmapFromURL(config.ReadEntryValue(WEBPAGE) as string,
                                                           config.ReadEntryValue(XPATH) as string);
            }

            var pictureData = makePixelArray(picture);
            var model = new DomainModel.WebSnapshot.WebSnapshot
                            {
                                Name = config.Name,
                                Picture = ToByteArray(pictureData),
                                PictureHeight = picture.Height,
                                PictureWidth = picture.Width,

                                CropHeight = picture.Height,
                                CropWidth = picture.Width,
                                CropCoordinateX = 0,
                                CropCoordinateY = 0
                            };
            
            databasepersister.Save(model);
        }

        private int[] makePixelArray(Bitmap picture)
        {
            var pixels = new int[picture.Height * picture.Width];
            int position = 0;

            for (int i = 0; i < picture.Width; i++)
            {
                for (int j = 0; j < picture.Height; j++)
                {
                    var pixel = picture.GetPixel(i, j);
                    pixels[position++] = pixel.B | (pixel.G << 8) | (pixel.R << 16) | (pixel.A << 24);                    
                }
            }

            return pixels;
        }

        private byte[] ToByteArray(int[] data)
        {
            int len = data.Length << 2;
            byte[] result = new byte[len];
            Buffer.BlockCopy(data, 0, result, 0, len);
            return result;
        }
    }
}
