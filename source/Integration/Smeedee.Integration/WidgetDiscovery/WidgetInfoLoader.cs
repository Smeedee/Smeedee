using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config.SlideConfig;

namespace Smeedee.Integration.WidgetDiscovery
{
    public class WidgetInfoLoader : IGetWidgetInfoFromXapFiles
    {
        private readonly IGetAssembliesFromXAP xapUnpacker;

        public WidgetInfoLoader(IGetAssembliesFromXAP xapUnpacker)
        {
            this.xapUnpacker = xapUnpacker;
        }

        public IEnumerable<WidgetInfo> FromDirectory(DirectoryInfo directoryToSearch)
        {
            if( directoryToSearch.Exists == false )
                throw new ArgumentException("The directory does not exist: " + directoryToSearch.FullName);

            var xapsInDirectory = directoryToSearch.GetFiles("*.xap", SearchOption.AllDirectories);
            
            foreach (var xapFile in xapsInDirectory)
            {
                var assembliesInXap = xapUnpacker.GetAssemblies(xapFile.FullName);

                var widgetInfosInAssembly = GetWidgetInfosFromAssemblies(assembliesInXap);
            }

            return new List<WidgetInfo>();
        }

        private IEnumerable<WidgetInfo> GetWidgetInfosFromAssemblies(IEnumerable<Assembly> assemblies)
        {
            List<WidgetInfo> widgetInfos = new List<WidgetInfo>();
            foreach (var assembly in assemblies)
            {
                try
                {
                    var allwidgetInfosInAssembly = GetWidgetInfoFromAssembly(assembly);
                    widgetInfos.AddRange(allwidgetInfosInAssembly);
                }
                catch (ReflectionTypeLoadException typeLoadException)
                {

                }
            }

            return widgetInfos;
        }

        private IEnumerable<WidgetInfo> GetWidgetInfoFromAssembly(Assembly assembly)
        {
            List<WidgetInfo> widgetInfos = new List<WidgetInfo>();
            var allTypesInAssembly = assembly.GetTypes();
            foreach (var type in allTypesInAssembly)
            {
                bool isWidget = type.IsSubclassOf(typeof (Widget));
                if( isWidget )
                {
                    object[] attributeInfo =  type.GetCustomAttributes(typeof (WidgetInfoAttribute), false);
                    bool hasDeclaredWidgetInformation = attributeInfo != null && attributeInfo.Length > 0;
                    if( hasDeclaredWidgetInformation )
                    {
                        var widgetInfo = GetWidgetInfoFromType(type, (WidgetInfoAttribute) attributeInfo[0]);
                        widgetInfos.Add(widgetInfo);
                    }
                }
            }

            return widgetInfos;
        }

        private WidgetInfo GetWidgetInfoFromType(Type type, WidgetInfoAttribute attribute)
        {
            return null;
        }
    }
}
