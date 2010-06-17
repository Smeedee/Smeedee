using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.ServiceModel.DomainServices.Client;
using System.Text;
using Moq;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Client.Web.Services;
using Smeedee.Widget.Standard.Factories;
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework.Testing;

namespace Smeedee.Client.Tests
{
    [Export(typeof(ServiceLocatorForTesting))]
    public class CustomServiceLocatorForTesting : ServiceLocatorForTesting
    {
        [Export(typeof(Mock<ITimer>))]
        public Mock<ITimer> timerFake;
        [Export(typeof(ITimer))]
        public ITimer timer;

        [Export(typeof(Mock<IUIInvoker>))]
        public Mock<IUIInvoker> uiInvokerFake;
        [Export(typeof(IUIInvoker))]
        public IUIInvoker uiInvoker;

        [Export(typeof(Mock<IModuleLoader>))]
        public Mock<IModuleLoader> moduleLoaderFake;
        [Export(typeof(IModuleLoader))]
        public IModuleLoader moduleLoader;

        [Export(typeof(IDomainContextFactory))]
        public IDomainContextFactory DomainContextFactory;

        public CustomServiceLocatorForTesting()
        {
            timerFake = new Mock<ITimer>();
            timer = timerFake.Object;
            uiInvokerFake = new Mock<IUIInvoker>();
            uiInvoker = uiInvokerFake.Object;
            uiInvokerFake.Setup(s => s.Invoke(It.IsAny<Action>())).Callback((Action a) => a.Invoke());

            moduleLoaderFake = new Mock<IModuleLoader>();
            moduleLoader = moduleLoaderFake.Object;
            moduleLoaderFake.Setup(s => s.LoadTraybarWidgets(It.IsAny<Traybar>())).
                Callback((Traybar traybar) =>
                {
                    traybar.Widgets.Add(new TraybarWidget());
                });
            moduleLoaderFake.Setup(s => s.LoadSlides(It.IsAny<Slideshow>())).
                Callback((Slideshow slideshow) =>
                {
                    slideshow.Slides.Add(new Slide(){ Title = "First slide" });
                    slideshow.Slides.Add(new Slide(){ Title = "Second slide" });
                });

            DomainContextFactory = new DomainContextFactoryMock();

            aggregateCatalog.Catalogs.Add(new TypeCatalog(typeof(CustomServiceLocatorForTesting)));


            /*var myKernel = kernel as StandardKernel;
            myKernel.Bind<Mock<ITimer>>().ToConstant(timerFake);
            myKernel.Bind<ITimer>().ToConstant(timerFake.Object);

            myKernel.Bind<Mock<IUIInvoker>>().ToConstant(uiInvokerFake);
            myKernel.Bind<IUIInvoker>().ToConstant(uiInvokerFake.Object);

            myKernel.Bind<Mock<IModuleLoader>>().ToConstant(moduleLoaderFake);
            myKernel.Bind<IModuleLoader>().ToConstant(moduleLoaderFake.Object);*/
        }

        public class DomainContextFactoryMock : IDomainContextFactory
        {
            public SourceControlContext NewSourceControlContext()
            {
                return new SourceControlContext(new ClientMock());
            }

            private class ClientMock : DomainClient
            {
                protected override IAsyncResult BeginQueryCore(EntityQuery query, AsyncCallback callback, object userState)
                {
                    throw new NotImplementedException();
                }

                protected override QueryCompletedResult EndQueryCore(IAsyncResult asyncResult)
                {
                    throw new NotImplementedException();
                }

                protected override SubmitCompletedResult EndSubmitCore(IAsyncResult asyncResult)
                {
                    throw new NotImplementedException();
                }

                protected override InvokeCompletedResult EndInvokeCore(IAsyncResult asyncResult)
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}
