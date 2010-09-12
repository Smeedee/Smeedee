//using System.ComponentModel.Composition;
//using Moq;
//using Ninject;
//using Smeedee.Client.Framework.Services;
//using Smeedee.Client.Framework.ViewModel;
//using System;
//using Smeedee.DomainModel.Framework;
//using TinyMVVM.Framework;
//using TinyMVVM.Framework.Services;

//namespace Smeedee.Client.Framework.Tests
//{
//    [Export(typeof(TinyMVVM.Framework.Testing.ServiceLocatorForTesting))]
//    public class ServiceLocatorForTesting : TinyMVVM.Framework.Testing.ServiceLocatorForTesting
//    {
//        private IKernel kernel;

//        public ServiceLocatorForTesting()
//        {
//            kernel = new StandardKernel();

//            var uiInvokerMock = new Mock<IUIInvoker>();
//            uiInvokerMock.Setup(s => s.Invoke(It.IsAny<Action>())).Callback((Action a) => a.Invoke());

//            var iTimerMock = new Mock<ITimer>();

//            var moduleLoaderFake = new Mock<IModuleLoader>();
//            moduleLoaderFake.Setup(s => s.LoadTraybarWidgets(It.IsAny<Traybar>())).
//                Callback((Traybar traybar) =>
//                {
//                    traybar.Widgets.Add(new Widget());
//                });
//            moduleLoaderFake.Setup(s => s.LoadSlides(It.IsAny<Slideshow>())).
//                Callback((Slideshow slideshow) =>
//                {
//                    slideshow.Slides.Add(new Slide { Title = "First slide", Widget = new Widget()});
//                    slideshow.Slides.Add(new Slide { Title = "Second slide", Widget = new Widget()});
//                });

//            var modalDialogServiceFake = new Mock<IModalDialogService>();
//            modalDialogServiceFake.Setup(s => s.Show(
//                It.IsAny<Dialog>(),
//                It.IsAny<Action<bool>>())).Callback((Dialog dialog, Action<bool> dialogClosedCallback) =>
//                {
//                    dialogClosedCallback.Invoke(true);
//                });

//            var widgetMetadataRepoFake = new Mock<IAsyncRepository<WidgetMetadata>>();

//                kernel.Bind<IUIInvoker>().ToConstant(uiInvokerMock.Object);
//            kernel.Bind<ITimer>().ToConstant(iTimerMock.Object);
//            kernel.Bind<IModuleLoader>().ToConstant(moduleLoaderFake.Object);
//            kernel.Bind<IModalDialogService>().ToConstant(modalDialogServiceFake.Object);
//            kernel.Bind<IAsyncRepository<WidgetMetadata>>().ToConstant(widgetMetadataRepoFake.Object);

//            kernel.Bind<Mock<IUIInvoker>>().ToConstant(uiInvokerMock);
//            kernel.Bind<Mock<ITimer>>().ToConstant(iTimerMock);
//            kernel.Bind<Mock<IModuleLoader>>().ToConstant(moduleLoaderFake);
//            kernel.Bind<Mock<IModalDialogService>>().ToConstant(modalDialogServiceFake);
//            kernel.Bind<Mock<IAsyncRepository<WidgetMetadata>>>().ToConstant(widgetMetadataRepoFake);

//        }

//        public void Bind<T>(T instance)
//        {
//            kernel.Bind<T>().ToConstant(instance);
//        }

//        public override T GetInstance<T>()
//        {
//                return kernel.Get<T>();
//        }

//        public static IServiceLocator GetServiceLocator()
//        {
//            return new Smeedee.Client.Framework.Tests.ServiceLocatorForTesting();
//        }
//    }
//}