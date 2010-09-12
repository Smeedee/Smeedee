//using System.ComponentModel.Composition;
//using System.ComponentModel.Composition.Hosting;
//using Smeedee.Client.Framework.Services.Impl;
//using TinyMVVM.Framework;
//using TinyMVVM.Framework.Services.Impl;

//namespace Smeedee.Client.Framework
//{
//    [Export(typeof(IServiceLocator))]
//    public class ServiceLocatorForClient : DefaultServiceLocator
//    {
//        public ServiceLocatorForClient()
//        {
//            aggregateCatalog.Catalogs.Add(new AssemblyCatalog(GetType().Assembly));
//            aggregateCatalog.Catalogs.Add(new TypeCatalog(
//                typeof(ModuleLoader),
//                typeof(StandardTimer),
//                typeof(UIInvoker)));
//        }
//    }
//}
