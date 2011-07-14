using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smeedee.DomainModel.Config;

namespace Smeedee.Widgets.GenericCharting.Controllers
{
    public class ChartConfig
    {
        public static readonly string chart_setting_name = "chart name";

        public static readonly string collection_setting_name = "collection";
        public static readonly string database_setting_name = "database";
        
        //public static readonly string xaxis_property_setting_name = "x-axis-property";
        //public static readonly string yaxis_property_Setting_name = "y-axis-property";
        private Configuration configuration;

        public ChartConfig(Configuration configuration)
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

        
        //public string XAxisProperty
        //{
        //    get { return configuration.GetSetting(xaxis_property_setting_name).Value; }
        //    set { configuration.ChangeSetting(xaxis_property_setting_name, value); }
        //}

        //public string YAxisProperty
        //{
        //    get { return configuration.GetSetting(yaxis_property_Setting_name).Value; }
        //    set { configuration.ChangeSetting(yaxis_property_Setting_name, value); }
        //}
        

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
                return
                    configuration.ContainsSetting(ChartConfig.database_setting_name) == true &&
                    configuration.ContainsSetting(ChartConfig.collection_setting_name) == true;
                //&&
                //configuration.ContainsSetting(ChartConfig.xaxis_property_setting_name) == true &&
                //configuration.ContainsSetting(ChartConfig.yaxis_property_Setting_name) == true;

                /*       "Config setting is missing; " + GraphConfig.xaxis_property_setting_name);
                Guard.Requires<ArgumentException>(configuration.ContainsSetting(GraphConfig.yaxis_property_Setting_name),
                    "Config setting is missing; " + GraphConfig.yaxis_property_Setting_name);*/

            }
        }

        public string ErrorMsg
        {
            get
            {
                //if (configuration.ContainsSetting(xaxis_property_setting_name) == false)
                //    return "Config setting is missing; " + xaxis_property_setting_name;
                //else if (configuration.ContainsSetting(yaxis_property_Setting_name) == false)
                //    return "Config setting is missing; " + yaxis_property_Setting_name;
                //else
                    return string.Empty;
            }
        }

        public static Configuration NewDefaultConfiguration()
        {
            var config = new Configuration("graph-config");
            config.NewSetting(database_setting_name);
            config.NewSetting(collection_setting_name);
            //config.NewSetting(xaxis_property_setting_name);
            //config.NewSetting(yaxis_property_Setting_name);
            config.IsConfigured = false;

            return config;
        }
    }
}
