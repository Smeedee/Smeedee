using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Threading;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Smeedee.Client.Framework.SL.ViewModel.Repositories;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Framework.DSL.Specifications;
using TinyMVVM.Framework;

namespace Smeedee.Client.Framework.SL.Tests.ViewModel.Repositories.WidgetMetadataRepositoryTests
{

    public class WidgetMetadataRepositorySpecs
    {
        [TestClass]
        public class when_get_all_completed : SilverlightTest
        {
            private static WidgetMetadataRepository repository = WidgetMetadataRepository.Instance;
            private static List<WidgetMetadata> result;
            private static List<WidgetMetadata> result2;

            static when_get_all_completed()
            {
                repository.GetCompleted += (o, e) => result = e.Result.ToList();
            }

            [TestInitialize]
            public void Setup()
            {
                //FrameworkBootstrapper.Initialize();
                repository.BeginGet(All.ItemsOf<WidgetMetadata>());
            }

            [TestMethod]
            [Asynchronous]
            [Tag("WidgetMetadataRepository")]
            public void Assure_repository_returns_data()
            {
                EnqueueConditional(() => result != null);

                EnqueueCallback(() =>
                {
                    Assert.IsTrue(result.Count() > 0);
                });

                EnqueueTestComplete();
            }

            [TestMethod]
            [Asynchronous]
            [Tag("WidgetMetadataRepository")]
            public void Assure_widgetMetadata_is_cached()
            {
                EnqueueConditional(() => result != null);
                DateTime startGet = DateTime.MaxValue;

                EnqueueCallback(() =>
                {
                    startGet = DateTime.Now;
                    repository.GetCompleted += (o, e) => result2 = e.Result.ToList();
                    repository.BeginGet(All.ItemsOf<WidgetMetadata>());   
                });

                EnqueueConditional(() => result2 != null);
                
                EnqueueCallback(() =>
                {
                    var timeToDoCall = DateTime.Now - startGet;
                    Assert.IsTrue( timeToDoCall.TotalMilliseconds < 100 );
                });

                EnqueueTestComplete();
            }

            [TestMethod]
            [Asynchronous]
            [Ignore]
            public void Assure_multiple_calls_to_beginGet_only_satisfies_imports_once()
            {
                var importingClass = new ImportingClass();
                EnqueueCallback(() =>
                {
                    Thread.Sleep(5000); // Wait for the WebClient to download the metadata
                    CompositionInitializer.SatisfyImports(importingClass);
                    repository.BeginGet(All.ItemsOf<WidgetMetadata>());
                });

                EnqueueConditional(() => result != null);

                EnqueueCallback(() =>
                {
                    Thread.Sleep(5000);
                    Assert.AreEqual(result.Count, importingClass.Widgets.Count);
                });

                EnqueueTestComplete();
            }
        }

        public class ImportingClass
        {
            [ImportMany(typeof(Widget), AllowRecomposition = true)]
            public List<Widget> Widgets { get; set; }

            public ImportingClass()
            {
                Widgets = new List<Widget>();
            }
        }

    }
}
