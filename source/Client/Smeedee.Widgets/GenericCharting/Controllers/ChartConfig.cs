using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Smeedee.DomainModel.Config;
using Smeedee.Widgets.GenericCharting.ViewModels;

namespace Smeedee.Widgets.GenericCharting.Controllers
{
    public class ChartConfig
    {
        public static readonly string chart_setting_name = "chartname";
        public static readonly string x_axis_setting_name = "x-axis";
        public static readonly string y_axis_setting_name = "y-axis";

        public static readonly string x_axis_setting_type = "x-axis type";

        public static readonly string series_setting_prefix = "series_";

        public static readonly string series_setting_action = series_setting_prefix + "action";
        public static readonly string series_setting_name = series_setting_prefix + "name";
        public static readonly string series_setting_legend = series_setting_prefix + "legend";
        public static readonly string series_setting_type = series_setting_prefix + "type";

        public static readonly string series_setting_database = series_setting_prefix + "database";
        public static readonly string series_setting_collection = series_setting_prefix + "collection";
        
        

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
            get { return configuration.GetSetting(x_axis_setting_type).Value;}
            set { configuration.ChangeSetting(x_axis_setting_type, value); }
        }
        public bool IsConfigured
        {
            get { return configuration.IsConfigured; }
            set { configuration.IsConfigured = value; }
        }

        public bool IsValid
        {
            get
            {
                return configuration.GetSetting(series_setting_name).Value != null;
            }
        }

        public void SetSeries(Collection<SeriesConfigViewModel> seriesConfig)
        {
            var names = new List<string>();
            var collections = new List<string>();
            var databases = new List<string>();
            var legends = new List<string>();
            var actions = new List<string>();
            var types = new List<string>();

            foreach (var series in seriesConfig)
            {
                names.Add(series.Name);
                collections.Add(series.Collection);
                databases.Add(series.Database);
                legends.Add(series.Legend ?? "");
                actions.Add(series.Action ?? "");
                types.Add(series.ChartType ?? "");
            }
            
            configuration.ChangeSetting(series_setting_name, names.ToArray());
            configuration.ChangeSetting(series_setting_collection, collections.ToArray());
            configuration.ChangeSetting(series_setting_database, databases.ToArray());
            configuration.ChangeSetting(series_setting_legend, legends.ToArray());
            configuration.ChangeSetting(series_setting_action, actions.ToArray());
            configuration.ChangeSetting(series_setting_type, types.ToArray());
        }

        public List<SeriesConfigViewModel> GetSeries()
        {
            var list = new List<SeriesConfigViewModel>();
            var names = configuration.GetSetting(series_setting_name).Vals.ToArray();
            var collections = configuration.GetSetting(series_setting_collection).Vals.ToArray();
            var databases = configuration.GetSetting(series_setting_database).Vals.ToArray();
            var legends = configuration.GetSetting(series_setting_legend).Vals.ToArray();
            var actions = configuration.GetSetting(series_setting_action).Vals.ToArray();
            var types = configuration.GetSetting(series_setting_type).Vals.ToArray();

            for (int i = 0; i < names.Count(); i++)
            {
                list.Add(new SeriesConfigViewModel
                    {
                        Name = names[i],
                        Collection = collections[i],
                        Database = databases[i],
                        Legend = legends[i],
                        Action = actions[i],
                        ChartType = types[i]
                    });
            }
            return list;
        }

        public Configuration Configuration
        {
            get { return configuration; }
            set { configuration = value; }
        }
        
        //public bool IsValid
        //{
        //    get
        //    {
        //        return
        //            configuration.ContainsSetting(ChartConfig.chart_setting_name) &&
        //            configuration.ContainsSetting(ChartConfig.x_axis_setting_name) &&
        //            configuration.ContainsSetting(ChartConfig.y_axis_setting_name) &&
        //            configuration.ContainsSetting(ChartConfig.x_axis_setting_type);

        //        /*       "Config setting is missing; " + GraphConfig.xaxis_property_setting_name);
        //        Guard.Requires<ArgumentException>(configuration.ContainsSetting(GraphConfig.yaxis_property_Setting_name),
        //            "Config setting is missing; " + GraphConfig.yaxis_property_Setting_name);*/

        //    }
        //}

        //public string ErrorMsg
        //{
        //    get
        //    {
        //        if (configuration.ContainsSetting(chart_setting_name) == false)
        //            return "Config setting is missing; " + chart_setting_name;
        //        else if (configuration.ContainsSetting(x_axis_setting_name) == false)
        //            return "Config setting is missing; " + x_axis_setting_name;
        //        else if (configuration.ContainsSetting(y_axis_setting_name) == false)
        //            return "Config setting is missing; " + y_axis_setting_name;
        //        else if (configuration.ContainsSetting(x_axis_setting_type) == false)
        //            return "Config setting is missing; " + x_axis_setting_type;
        //        else
        //        return string.Empty;
        //    }
        //}

        public static Configuration NewDefaultConfiguration()
        {
            var config = new Configuration("GenericCharting");
            config.NewSetting(chart_setting_name);
            config.NewSetting(x_axis_setting_name);
            config.NewSetting(y_axis_setting_name);
            config.NewSetting(x_axis_setting_type);
            config.NewSetting(series_setting_name);
            config.NewSetting(series_setting_collection);
            config.NewSetting(series_setting_database);
            config.NewSetting(series_setting_legend);
            config.NewSetting(series_setting_action);
            config.NewSetting(series_setting_type);
            config.IsConfigured = false;
            return config;
        }


    }
}
