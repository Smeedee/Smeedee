using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Smeedee.Integration.Framework.Utils
{
    public class FileUtilities : IFileUtilities
    {

        public IEnumerable<string> GetDLLs(string folder)
        {
            return Directory.EnumerateFiles(folder).Where(f => f.EndsWith(".dll"));
        }

        public IEnumerable<Type> FindImplementationsOrSubclassesOf<T>(string folder)
        {
            var dlls = GetDLLs(folder);
            var types = GetAllTypesFromDlls(dlls);

            return from type in types
                   where (type.IsSubclassOf(typeof(T)) || typeof(T).IsAssignableFrom(type)) && 
                            type.IsClass && 
                            type != typeof(T)
                   select type;
        }

        private IEnumerable<Type> GetAllTypesFromDlls(IEnumerable<string> dlls)
        {
            var types = new List<Type>();

            foreach (var dll in dlls)
            {
                try
                {
                    types.AddRange(Assembly.LoadFrom(dll).GetTypes());
                }
                catch (Exception)
                {
                    //a dll could not be loaded, but this should not stop other dll's from being searched 
                }
            }

            return types;
        }

    }
}
