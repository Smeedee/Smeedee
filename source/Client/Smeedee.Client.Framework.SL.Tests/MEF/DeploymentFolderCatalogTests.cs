using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Microsoft.Silverlight.Testing;
using Microsoft.Silverlight.Testing.UnitTesting.Metadata.VisualStudio;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Smeedee.Client.Framework.SL.MEF;
using Smeedee.Client.Framework.SL.MEF.Services;
using Smeedee.Client.Framework.SL.MEF.System.ComponentModel.Composition.Hosting;

namespace Smeedee.Client.Framework.SL.Tests.MEF
{
    public class DeploymentFolderCatalogTests
    {
        public class Context : SilverlightTest
        {
            protected static Uri serverUrl = GetServerUrl();
            protected static Uri silverlightDeploymentMetadataUrl = new Uri(string.Format("{0}SilverlightDeploymentMetadata.ashx", serverUrl));
            protected CustomDeploymentFolderCatalog catalog;
            protected int xapDownloadCounter;
            protected DownloadStringCompletedEventArgs testDownloadMetadataEventArgs;
            protected AsyncCompletedEventArgs metadataDownloadCompletedArgs;
            protected List<string> assembliesDeployedOnServer = new List<string>();

            private static Uri GetServerUrl()
            {
                var url = new string(HtmlPage.Document.DocumentUri.ToString().Reverse().SkipWhile(c => c != '/').Reverse().ToArray());
                return new Uri(url);
            }

            protected void DownloadDeploymentMetadata()
            {
                WebClient a = new WebClient();
                a.DownloadStringCompleted += (o, e) =>
                {
                    testDownloadMetadataEventArgs = e;
                    if (e.Error == null)
                    {
                        var xDoc = XDocument.Parse(e.Result);
                        assembliesDeployedOnServer.AddRange(
                            from element in xDoc.Element("SilverlightDeployment").Elements()
                            select element.Value);
                    };
                };
                a.DownloadStringAsync(silverlightDeploymentMetadataUrl);
            }

            protected bool WaitUntilTestHasDownloadedDeploymentMetadata()
            {
                return testDownloadMetadataEventArgs != null;
            }

            protected bool WaitUntilAllXapsAreDownloaded()
            {
                return xapDownloadCounter == assembliesDeployedOnServer.Count;
            }

            protected bool WaitUntilMetadataDownloadedCompletedIsTriggered()
            {
                return metadataDownloadCompletedArgs != null;
            }
        }

        [TestClass]
        public class When_spawned : Context
        {

            [TestInitialize]
            public void Setup()
            {
                catalog = new CustomDeploymentFolderCatalog();
            }

            [TestMethod]
            public void assure_its_a_ComposablePartCatalog()
            {
                Assert.IsTrue(catalog is ComposablePartCatalog);
            }
        }

        [TestClass]
        public class When_DownloadAsync : Context
        {
            [TestInitialize]
            public void Setup()
            {
                catalog = new CustomDeploymentFolderCatalog()
                {
                    CreateFakeDownloadService = true
                };

                catalog.DownloadAsync();
            }

            [TestMethod]
            public void assure_it_grabs_DeploymentMetadata_from_server()
            {
                catalog.DownloadServiceFake.Verify(s => s.DownloadStringAsync(silverlightDeploymentMetadataUrl));
            }
        }

        [TestClass]
        public class When_downloaded : Context
        {
            private int chaningEventCounter;
            private bool AllCompletedEventIsTriggered = false;

            [TestInitialize]
            public void Setup()
            {
                xapDownloadCounter = 0;
                testDownloadMetadataEventArgs = null;
                metadataDownloadCompletedArgs = null;
                assembliesDeployedOnServer.Clear();

                catalog = new CustomDeploymentFolderCatalog();
                catalog.AllCompleted += (o, e) => AllCompletedEventIsTriggered = true;
                catalog.DownloadCompleted += (o, e) =>
                {
                    xapDownloadCounter++;
                };
                catalog.Changing += (o, e) =>
                {
                    chaningEventCounter++;
                };
                catalog.MetadataDownloadCompleted += (o, e) =>
                {
                    metadataDownloadCompletedArgs = e;
                };

                catalog.DownloadAsync();
            }

            [TestMethod]
            [Asynchronous]
            public void Then_assure_AllCompleted_is_triggered()
            {
                EnqueueCallback(() => DownloadDeploymentMetadata());
                EnqueueConditional(() => WaitUntilAllXapsAreDownloaded());
                EnqueueDelay(200);

                EnqueueCallback(() =>
                {
                    Assert.IsTrue(AllCompletedEventIsTriggered);
                });

                EnqueueTestComplete();
            }

            [TestMethod]
            [Asynchronous]
            public void assure_MetadataDownloadCompleted_event_is_triggered()
            {
                EnqueueConditional(() => WaitUntilAllXapsAreDownloaded());

                EnqueueCallback(() =>
                {
                    Assert.IsNotNull(metadataDownloadCompletedArgs);
                    Assert.IsFalse(metadataDownloadCompletedArgs.Cancelled);
                    Assert.IsNull(metadataDownloadCompletedArgs.UserState);
                });

                EnqueueTestComplete();
            }


            [TestMethod]
            [Asynchronous]
            public void assure_it_trigger_DownloadCompleted_event_after_XAP_is_downloaded()
            {
                EnqueueCallback(() => DownloadDeploymentMetadata());
                EnqueueConditional(() => WaitUntilTestHasDownloadedDeploymentMetadata());
                EnqueueDelay(2000);

                EnqueueCallback(() =>
                {
                    Assert.AreEqual(assembliesDeployedOnServer.Count, xapDownloadCounter);
                });

                EnqueueTestComplete();
            }

            [TestMethod]
            [Asynchronous]
            public void assure_downloaded_xaps_are_loaded_into_AppDomain()
            {
                EnqueueCallback(() => DownloadDeploymentMetadata());
                EnqueueConditional(() => WaitUntilTestHasDownloadedDeploymentMetadata());
                EnqueueConditional(() => WaitUntilAllXapsAreDownloaded());

                EnqueueCallback(() =>
                {
                    //TODO: Find a better test to veriy that assemblies has been loaded into appDomain
                    Assert.AreNotEqual(0, catalog.Parts.Count());
                });

                EnqueueTestComplete();
            }

            [TestMethod]
            [Asynchronous]
            public void assure_Changing_Event_is_Triggered_for_each_XAP_download()
            {
                EnqueueCallback(() => DownloadDeploymentMetadata());
                EnqueueConditional(() => WaitUntilTestHasDownloadedDeploymentMetadata());
                EnqueueConditional(() => WaitUntilAllXapsAreDownloaded());

                EnqueueCallback(() =>
                {
                    Assert.AreEqual(assembliesDeployedOnServer.Count, chaningEventCounter);
                });

                EnqueueTestComplete();
            }

        }

        [TestClass]
        public class When_DeploymentMetadata_endpoint_doesnt_exist_on_server : Context
        {
            [TestInitialize]
            public void Setup()
            {
                catalog = new CustomDeploymentFolderCatalog()
                {
                    MetadataUri = new Uri("http://localhost/doestexist.ashx")
                };
                catalog.MetadataDownloadCompleted += (o, e) =>
                {
                    metadataDownloadCompletedArgs = e;
                };

                catalog.DownloadAsync();
            }

            [TestMethod]
            [Asynchronous]
            public void assure_Error_info_is_provided()
            {
                EnqueueConditional(() => WaitUntilMetadataDownloadedCompletedIsTriggered());

                EnqueueCallback(() =>
                {
                    Assert.IsNotNull(metadataDownloadCompletedArgs.Error);
                });

                EnqueueTestComplete();
            }
        }

        [TestClass]
        public class When_DeploymentMetadata_endpoint_is_in_an_invalid_format : Context
        {
            [TestInitialize]
            public void Setup()
            {
                catalog = new CustomDeploymentFolderCatalog()
                {
                    MetadataUri = new Uri(string.Format("{0}Silverlight.js", serverUrl))
                };
                catalog.MetadataDownloadCompleted += (o, e) =>
                    metadataDownloadCompletedArgs = e;

                catalog.DownloadAsync();
            }

            [TestMethod]
            [Asynchronous]
            public void assure_its_handeled()
            {
                EnqueueConditional(() => WaitUntilMetadataDownloadedCompletedIsTriggered());

                EnqueueCallback(() =>
                {
                    Assert.IsNotNull(metadataDownloadCompletedArgs.Error);
                    Assert.AreEqual("Invalid format in deployment metadata", metadataDownloadCompletedArgs.Error.Message);
                });

                EnqueueTestComplete();
            }
        }

        public class CustomDeploymentFolderCatalog : DeploymentFolderCatalog
        {
            public Mock<IDownloadService> DownloadServiceFake = new Mock<IDownloadService>();
            public Uri MetadataUri { get; set; }
            public bool CreateFakeDownloadService { get; set; }

            public CustomDeploymentFolderCatalog()
            {
            }

            protected override IDownloadService CreateNewDownloadService()
            {
                if (CreateFakeDownloadService)
                    return DownloadServiceFake.Object;
                else
                    return base.CreateNewDownloadService();
            }

            protected override Uri GetDeploymentMetadataUrl()
            {
                if (MetadataUri != null)
                    return MetadataUri;
                else
                    return base.GetDeploymentMetadataUrl();
            }

            public void RaiseMetadataDownloadedCompleted(AsyncCompletedEventArgs e)
            {
                OnMetadataDownloadCompleted(e);
            }
        }

    }
}
