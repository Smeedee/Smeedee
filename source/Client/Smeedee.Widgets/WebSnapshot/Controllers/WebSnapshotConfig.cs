using System;
using Smeedee.DomainModel.Config;

namespace Smeedee.Widgets.WebSnapshot.Controllers
{
    public class WebSnapshotConfig
    {
        private Configuration configuration;
        public static readonly string url = "url";
        public static readonly string coordinateX ="cropCoordinateX";
        public static readonly string coordinateY = "cropCoordinateY";
        public static readonly string rectangleHeight ="rectangle-height";
        public static readonly string rectangleWidth= "rectangle-width";

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
            set { configuration.ChangeSetting(coordinateX, string.IsNullOrEmpty(value) ? "0" : value); }
        }

        public string CoordinateY
        {
            get { return configuration.GetSetting(coordinateY).Value; }
            set { configuration.ChangeSetting(coordinateY, string.IsNullOrEmpty(value) ? "0" : value); }
        }

        public string RectangleHeight 
        {
            get { return configuration.GetSetting(rectangleHeight).Value; }
            set { configuration.ChangeSetting(rectangleHeight, string.IsNullOrEmpty(value) ? "0" : value); }
        }

        public string RectangleWidth
        {
            get { return configuration.GetSetting(rectangleWidth).Value; }
            set { configuration.ChangeSetting(rectangleWidth, string.IsNullOrEmpty(value) ? "0" : value); }
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
