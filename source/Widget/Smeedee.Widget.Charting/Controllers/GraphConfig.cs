using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smeedee.DomainModel.Config;

namespace Tskjortebutikken.Widgets.Controllers
{
    /// <summary>
    /// Strong typed Wrapper object for the Graph Configuration. 
    /// </summary>
    public class GraphConfig
    {
        public static readonly string collection_setting_name = "collction";
        public static readonly string database_setting_name = "database";
        public static readonly string refrehInterval_setting_name = "refresh-interval";
        public static readonly string maxNrOfDataPoints_setting_name = "max-number-of-datapoints";
        public static readonly string xaxis_property_setting_name = "x-axis-property";
        public static readonly string yaxis_property_Setting_name = "y-axis-property";
        private Configuration configuration;

        public GraphConfig(Configuration configuration)
        {
            this.configuration = configuration;
        }

        public string Database
        {
            get { return configuration.GetSetting(database_setting_name).Value; }
            set { configuration.ChangeSetting(database_setting_name, value); }
        }

        public string Collection
        {
            get { return configuration.GetSetting(collection_setting_name).Value; }
            set { configuration.ChangeSetting(collection_setting_name, value); }
        }

        public int MaxNumberOfDataPoints
        {
            get { return int.Parse(configuration.GetSetting(maxNrOfDataPoints_setting_name).Value); }
            set { configuration.ChangeSetting(maxNrOfDataPoints_setting_name, value.ToString());}
        }

        public string XAxisProperty
        {
            get { return configuration.GetSetting(xaxis_property_setting_name).Value; }
            set { configuration.ChangeSetting(xaxis_property_setting_name, value); }
        }

        public string YAxisProperty
        {
            get { return configuration.GetSetting(yaxis_property_Setting_name).Value; }
            set { configuration.ChangeSetting(yaxis_property_Setting_name, value); }
        }

        public int RefreshInteval
        {
            get { return int.Parse(configuration.GetSetting(refrehInterval_setting_name).Value); }
            set { configuration.ChangeSetting(refrehInterval_setting_name, value.ToString());}
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

        public bool IsValid
        {
            get
            {
                return configuration.ContainsSetting(GraphConfig.refrehInterval_setting_name) &&
                       configuration.GetSetting(GraphConfig.refrehInterval_setting_name).Value != null &&
                       configuration.GetSetting(GraphConfig.refrehInterval_setting_name).Value != "" &&
                       configuration.ContainsSetting(GraphConfig.database_setting_name) == true &&
                       configuration.ContainsSetting(GraphConfig.collection_setting_name) == true &&
                       configuration.ContainsSetting(GraphConfig.xaxis_property_setting_name) == true &&
                       configuration.ContainsSetting(GraphConfig.yaxis_property_Setting_name) == true;

             /*       "Config setting is missing; " + GraphConfig.xaxis_property_setting_name);
                Guard.Requires<ArgumentException>(configuration.ContainsSetting(GraphConfig.yaxis_property_Setting_name),
                    "Config setting is missing; " + GraphConfig.yaxis_property_Setting_name);*/

            }
        }

        public string ErrorMsg
        {
            get
            {
                if (configuration.ContainsSetting(xaxis_property_setting_name) == false)
                    return "Config setting is missing; " + xaxis_property_setting_name;
                else if (configuration.ContainsSetting(yaxis_property_Setting_name) == false)
                    return "Config setting is missing; " + yaxis_property_Setting_name;
                else
                    return string.Empty;
            }
        }

        public static Configuration NewDefaultConfiguration()
        {
            var config = new Configuration("graph-config");
            config.NewSetting(refrehInterval_setting_name, "30000");
            config.NewSetting(database_setting_name);
            config.NewSetting(collection_setting_name);
            config.NewSetting(maxNrOfDataPoints_setting_name, "14");
            config.NewSetting(xaxis_property_setting_name);
            config.NewSetting(yaxis_property_Setting_name);
            config.IsConfigured = false;

            return config;
        }
    }
}
