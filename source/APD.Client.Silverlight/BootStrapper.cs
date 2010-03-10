#region File header

// <copyright>
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
// /copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.Windows;
using System.Windows.Controls;
using APD.Client.Framework;
using APD.Client.Framework.Settings;
using APD.Client.Framework.SL;
using APD.Client.Framework.SL.Repositories;
using APD.Client.Widget.BurndownChart.SL;
using APD.Client.Widget.CI.SL;
using APD.Client.Widget.DeveloperInfo.SL.Repositories;
using APD.Client.Widget.General.SL;
using APD.Client.Widget.ProjectInfo.SL;
using APD.Client.Widget.SourceControl.SL;
using APD.Client.Widget.SourceControl.SL.Repositories;
using APD.Client.Widget.TrayBar.SL;
using APD.DomainModel.Config;
using APD.DomainModel.Framework;
using APD.DomainModel.Framework.Logging;
using APD.DomainModel.Holidays;
using APD.Framework.SL.Logging;

using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Composite.UnityExtensions;
using APD.Client.Widget.Admin.SL;
using APD.Client.Services;
using APD.Client.Silverlight.Services.Impl;
using APD.DomainModel.SourceControl;
using APD.Client.Framework.Factories;
using APD.DomainModel.Users;


namespace APD.Client.Silverlight
{
    public class BootStrapper : UnityBootstrapper
    {
        private readonly ClientSettingsReader settingsReader;

        public BootStrapper()
        {
        }

        public BootStrapper(ClientSettingsReader settingsReader)
        {
            this.settingsReader = settingsReader;
        }

        protected override IModuleCatalog GetModuleCatalog()
        {
            var catalog = new ModuleCatalog();

            catalog.AddModule(typeof(LoadingAnimationModule));
            catalog.AddModule(typeof(WorkingDaysLeftModule));
            catalog.AddModule(typeof(SourceControlModule));
            catalog.AddModule(typeof(TopCommitersModule));
            catalog.AddModule(typeof(ContinuousIntegrationModule));
            //catalog.AddModule(typeof(BurndownChartModule));
            catalog.AddModule(typeof(CommitStatisticsModule));
            catalog.AddModule(typeof(AdminModule));
            catalog.AddModule(typeof(CINotifierModule));
            catalog.AddModule(typeof(TrayBarModule));

            return catalog;
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            RegisterAllSharedContainerDependencies();
        }

        private void RegisterAllSharedContainerDependencies()
        {
            UIInvokerFactory.UIInvoker = new SilverlightUIInvoker(new TextBlock().Dispatcher);
            Container.RegisterInstance<IClientSettingsReader>(settingsReader);
            Container.RegisterType<IShell, NiceShell>();
            Container.RegisterInstance<IInvokeUI>(UIInvokerFactory.Assemlble());
            Container.RegisterInstance<IRepository<Changeset>>(
                new StaticRepositoryCache<Changeset>(
                    new SyncChangeSetRepository(new AsyncChangesetRepository()), 15000));
            Container.RegisterType<IRepository<User>, UserWebserviceRepositoryProxy>();
            Container.RegisterType<IRepository<Holiday>, HolidayWebserviceRepository>();
            Container.RegisterType<ICheckIfAdminUIShouldBeDisplayed, CheckIfAdminUIShouldBeDisplayedFromUrl>();
            Container.RegisterInstance<IRepository<Configuration>>(
                new StaticRepositoryCache<Configuration>(
                new ConfigurationRepository(), 3600000));
            Container.RegisterType<IPersistDomainModels<Configuration>, ConfigurationRepository>();
            Container.RegisterType<IPersistDomainModels<LogEntry>, LogEntryWebservicePersister>();
            Container.RegisterType<ILog, DatabaseLogger>();
        }

        protected override void InitializeModules()
        {
            foreach (var moduleInfo in GetModuleCatalog().Modules)
            {
                var moduleType = Type.GetType(moduleInfo.ModuleType);
                var module = (IModule)Container.Resolve(moduleType);
                module.Initialize();

                Container.RegisterInstance(moduleType, module);
            }


            var workingDaysLeftView = new WorkingDaysLeft();
            workingDaysLeftView.DataContext = Container.Resolve<WorkingDaysLeftModule>().View.DataContext;
            Container.Resolve<TrayBarModule>().AddView(workingDaysLeftView);
            Container.Resolve<TrayBarModule>().AddView(Container.Resolve<CINotifierModule>().View);

            Slide slide1 = new Slide("Latest Commits", 10)
            {
                MainContentModule = Container.Resolve<SourceControlModule>(),
                BottomLeftModule = Container.Resolve<TrayBarModule>()
            };
           
            Slide slide2 = new Slide("Build Status", 15)
            {
                MainContentModule = Container.Resolve<ContinuousIntegrationModule>(),
                BottomLeftModule = Container.Resolve<TrayBarModule>()
            };

            //Slide slide3 = new Slide("Burndown Chart", 15)
            //{
            //    MainContentModule = Container.Resolve<BurndownChartModule>()
            //};
          
            Slide slide4 = new Slide("Commit Heroes", 15)
            {
                MainContentModule = Container.Resolve<TopCommitersModule>(),
                BottomLeftModule = Container.Resolve<TrayBarModule>()
            };
         
            Slide slide5 = new Slide("Commit Statistics", 15)
            {
                MainContentModule = Container.Resolve<CommitStatisticsModule>(),
                BottomLeftModule = Container.Resolve<TrayBarModule>()
            };
          
            Slide slide6 = new Slide("Working Days Left", 10)
            {
                MainContentModule = Container.Resolve<WorkingDaysLeftModule>(),
                BottomLeftModule = Container.Resolve<TrayBarModule>()
            };
          
            //Slide slide7 = new Slide("Administration", 15)
            //{
            //    MainContentModule = Container.Resolve<AdminModule>(),
            //    BottomLeftModule = Container.Resolve<TrayBarModule>()
            //};
 
            var shellPresenter = Container.Resolve<ShellPresenter>();

            //shellPresenter.Slides = (new[]
            //                          {
            //                              slide1, slide2, slide3, slide4, slide5, slide6, slide7
            //                          });
            shellPresenter.Slides = (new[]
                                      {
                                          slide1, slide2, slide4, slide5, slide6
                                      });

            shellPresenter.AdminSlide = new Slide("Administration", 15)
            {
                MainContentModule = Container.Resolve<AdminModule>(),
                BottomLeftModule = Container.Resolve<TrayBarModule>()
            };

            shellPresenter.StartSlideRotation();
        }

        protected override DependencyObject CreateShell()
        {
            var shellPresenter = Container.Resolve<ShellPresenter>();

            Container.RegisterInstance<ShellPresenter>(shellPresenter);

            IShell shell = shellPresenter.Shell;

            shell.ShowView();

            return shell as DependencyObject;
        }
    }
}