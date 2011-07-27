using System;
using Smeedee.DomainModel.Config;

namespace Smeedee.Widgets.WebSnapshot.Controllers
{
    public class WebSnapshotConfig
    {
        private Configuration configuration;
        public static readonly string url = "URL";
        public static readonly string coordinateX ="Coordinate X";
        public static readonly string coordinateY = "Coordinate Y";
        public static readonly string rectangleHeight ="Crop rectangle height";
        public static readonly string rectangleWidth= "Crop rectangle width";

        public WebSnapshotConfig(Configuration configuration)
        {
            this.configuration = configuration;
        }

        public string URL
        {
            get { return configuration.GetSetting(url).Value; }
            set { configuration.ChangeSetting(url, value); }
        }

        public string CoordinateX
        {
            get { return configuration.GetSetting(coordinateX).Value; }
            set { configuration.ChangeSetting(coordinateX, value); }
        }

        public string CoordinateY
        {
            get { return configuration.GetSetting(coordinateY).Value; }
            set { configuration.ChangeSetting(coordinateY, value); }
        }

        public string RectangleHeight 
        {
            get { return configuration.GetSetting(rectangleHeight).Value; }
            set { configuration.ChangeSetting(rectangleHeight, value); }
        }

        public string RectangleWidth
        {
            get { return configuration.GetSetting(rectangleWidth).Value; }
            set { configuration.ChangeSetting(rectangleWidth, value); }
        }
        
        public bool IsConfigured
        {
            get { return configuration.IsConfigured; }
            set { configuration.IsConfigured = value; }
        }

        public Configuration Configuration
        {
            get { return configuration; }
        }

        public static Configuration NewDefaultConfiguration()
        {
            var config = new Configuration("WebSnapshot");
            config.NewSetting(url);
            config.NewSetting(coordinateX);
            config.NewSetting(coordinateY);
            config.NewSetting(rectangleHeight);
            config.NewSetting(rectangleWidth);
            config.IsConfigured = false;
            return config;
        }
    }
}
