using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;

namespace Smeedee.Client.Framework.Services.Impl
{
    public class ConfigurationManager : IManageConfigurations
    {
        public const int REFRESH_INTERVAL_SECONDS = 30;
        List<Widget> registeredWidgets = new List<Widget>();

        private readonly ITimer timer;
        private readonly IAsyncRepository<Configuration> asyncRepository;
        private IEnumerable<Configuration> latestConfigurations = new List<Configuration>();

        public ConfigurationManager(ITimer timer, IAsyncRepository<Configuration> asyncRepository)
        {
            this.timer = timer;
            timer.Start(REFRESH_INTERVAL_SECONDS * 1000);
            this.asyncRepository = asyncRepository;
            this.asyncRepository.GetCompleted += asyncRepository_GetCompleted;
            timer.Elapsed += timer_Elapsed;
            BeginGetConfigurations();
            
        }

        void asyncRepository_GetCompleted(object sender, GetCompletedEventArgs<Configuration> e)
        {
            if( e.Result != null )
            {
                latestConfigurations = e.Result;
                UpdateAllWidgets(latestConfigurations);
            }
        }

        private void UpdateAllWidgets(IEnumerable<Configuration> allConfigurations)
        {
            foreach (var registeredWidget in registeredWidgets)
            {
                Configuration newlyFetchedConfigForWidget = GetConfigForWidget(allConfigurations, registeredWidget);
                CheckForChangesAndNotify(newlyFetchedConfigForWidget, registeredWidget);
            }
        }

        private Configuration GetConfigForWidget(IEnumerable<Configuration> latestConfigurations, Widget registeredWidget)
        {
            return latestConfigurations.SingleOrDefault(c => c.Id == registeredWidget.Configuration.Id);
        }

        private void CheckForChangesAndNotify(Configuration latestConfig, Widget registeredWidget)
        {
            if( latestConfig != null )
            {
                if( !ConfigsAreEqual(latestConfig, registeredWidget.Configuration))
                {
                    registeredWidget.Configuration = latestConfig;
                }
            }
        }

        private bool ConfigsAreEqual(Configuration result, Configuration expected)
        {

            bool areEqual = result.Id.Equals(expected.Id);
            areEqual &= result.Name.Equals(expected.Name);
            areEqual &=  result.Settings.Count().Equals(expected.Settings.Count());
            if (areEqual == false) return false;

            for (int i = 0; i < result.Settings.Count(); i++)
            {
                areEqual &= result.Settings.ElementAt(i).Value == expected.Settings.ElementAt(i).Value;
                areEqual &= result.Settings.ElementAt(i).Name.Equals(expected.Settings.ElementAt(i).Name);
                areEqual &= result.Settings.ElementAt(i).Vals.Count().Equals(expected.Settings.ElementAt(i).Vals.Count());
                if (areEqual == false) return false;
                for (int j = 0; j < result.Settings.ElementAt(i).Vals.Count(); j++)
                {
                    areEqual &= result.Settings.ElementAt(i).Vals.ElementAt(j).Equals(expected.Settings.ElementAt(i).Vals.ElementAt(j));
                }
            }

            return areEqual;
        }

        void timer_Elapsed(object sender, EventArgs e)
        {
            BeginGetConfigurations();
        }

        private void BeginGetConfigurations()
        {
            asyncRepository.BeginGet(new AllSpecification<Configuration>());
        }

        public void RegisterForConfigurationUpdates(Widget widgetToRegister)
        {
            registeredWidgets.Add(widgetToRegister);
            var config = GetConfigForWidget(latestConfigurations, widgetToRegister);
            CheckForChangesAndNotify(config, widgetToRegister);
        }
    }
}
