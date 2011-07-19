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
        public static readonly string x_axis_setting_name = "x-axis";
        public static readonly string y_axis_setting_name = "y-axis";

        public static readonly string x_axis_setting_type = "x-axis type";

        public static readonly string database_setting_name = "database";
        public static readonly string collection_setting_name = "collection";

        public static readonly string data_name_setting = "data name";
        public static readonly string chart_type_setting = "chart type";

        private Configuration configuration;

        public ChartConfig(Configuration configuration)
        {
            this.configuration = configuration;
        }

        public string ChartName
        {
            get { return configuration.GetSetting(chart_setting_name).Value; }
            set { configuration.ChangeSetting(chart_setting_name, value); }
        }

        public string XAxisName
        {
            get { return configuration.GetSetting(x_axis_setting_name).Value; }
            set { configuration.ChangeSetting(x_axis_setting_name, value); }
        }

        public string YAxisName
        {
            get { return configuration.GetSetting(y_axis_setting_name).Value; }
            set { configuration.ChangeSetting(y_axis_setting_name, value); }
        }

        public string XAxisType
        {
            get { return configuration.GetSetting(x_axis_setting_type).Value; }
            set { configuration.ChangeSetting(x_axis_setting_type, value); }
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

        public string DataName
        {
            get { return configuration.GetSetting(data_name_setting).Value; }
            set { configuration.ChangeSetting(data_name_setting, value); }
        }

        public string SelectedChartType
        {
            get { return configuration.GetSetting(chart_type_setting).Value; }
            set { configuration.ChangeSetting(chart_type_setting, value); }
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
                return
                    configuration.ContainsSetting(ChartConfig.chart_setting_name) &&
                    configuration.ContainsSetting(ChartConfig.x_axis_setting_name) &&
                    configuration.ContainsSetting(ChartConfig.y_axis_setting_name) &&
                    configuration.ContainsSetting(ChartConfig.x_axis_setting_type) &&
                    configuration.ContainsSetting(ChartConfig.database_setting_name) &&
                    configuration.ContainsSetting(ChartConfig.collection_setting_name) &&
                    configuration.ContainsSetting(ChartConfig.data_name_setting) &&
                    configuration.ContainsSetting(ChartConfig.chart_type_setting);

                /*       "Config setting is missing; " + GraphConfig.xaxis_property_setting_name);
                Guard.Requires<ArgumentException>(configuration.ContainsSetting(GraphConfig.yaxis_property_Setting_name),
                    "Config setting is missing; " + GraphConfig.yaxis_property_Setting_name);*/

            }
        }

        public string ErrorMsg
        {
            get
            {
                if (configuration.ContainsSetting(chart_setting_name) == false)
                    return "Config setting is missing; " + chart_setting_name;
                else if (configuration.ContainsSetting(x_axis_setting_name) == false)
                    return "Config setting is missing; " + x_axis_setting_name;
                else if (configuration.ContainsSetting(y_axis_setting_name) == false)
                    return "Config setting is missing; " + y_axis_setting_name;
                else if (configuration.ContainsSetting(x_axis_setting_type) == false)
                    return "Config setting is missing; " + x_axis_setting_type;
                else if (configuration.ContainsSetting(database_setting_name) == false)
                    return "Config setting is missing; " + database_setting_name;
                else if (configuration.ContainsSetting(collection_setting_name) == false)
                    return "Config setting is missing; " + collection_setting_name;
                else if (configuration.ContainsSetting(data_name_setting) == false)
                    return "Config setting is missing; " + data_name_setting;
                else if (configuration.ContainsSetting(chart_type_setting) == false)
                    return "Config setting is missing; " + chart_type_setting;
                else
                return string.Empty;
            }
        }

        //Not used
        //public static Configuration NewDefaultConfiguration()
        //{
        //    var config = new Configuration("graph-config");
        //    config.NewSetting(chart_setting_name);
        //    config.NewSetting(x_axis_setting_name);
        //    config.NewSetting(y_axis_setting_name);
        //    config.NewSetting(x_axis_setting_type);
        //    config.NewSetting(database_setting_name);
        //    config.NewSetting(collection_setting_name);
        //    config.NewSetting(data_name_setting);
        //    config.NewSetting(chart_type_setting);
        //    config.IsConfigured = false;

        //    return config;
        //}
    }
}
