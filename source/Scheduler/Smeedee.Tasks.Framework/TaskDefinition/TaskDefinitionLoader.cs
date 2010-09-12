using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.TaskDefinition;
using Smeedee.Integration.Database.DomainModel.Repositories;
using Smeedee.Integration.Framework.Utils;
using Smeedee.Tasks.Framework.Smeedee.Tasks.Framework;
using Smeedee.Tasks.Framework.TaskAttributes;

namespace Smeedee.Tasks.Framework.TaskDefinitions
{
    public class TaskDefinitionLoader
    {
        private static readonly IFileUtilities fileUtility = new FileUtilities();

        private readonly ILog _logger;

        public TaskDefinitionLoader(ILog logger)
        {
            _logger = logger;
        }

        public IEnumerable<TaskDefinition> LoadFromFolder(string folder)
        {
            var definitions = new List<TaskDefinition>();
            foreach (var dll in fileUtility.GetDLLs(folder))
            {
                try
                {
                    definitions.AddRange(LoadFromDLL(dll));
                } 
                catch (BadImageFormatException e)
                {
                    //Ignore. The .dll file isn't a loadable assembly.
                    //It might be unmanaged code or using the wrong .NET version
                    //Either way, we can't expect to load task defintions from it,
                    //but we can expect files like this to be in the folder we're loading from.
                }
                catch (Exception e)
                {
                    var logEntry = ErrorLogEntry.Create(this,
                                                        "Failed to load TaskDefinition from Dll. Exception: " + e.Message);
                    if (e is ReflectionTypeLoadException)
                        logEntry.Message += "\r\nLoader Exception: "+((ReflectionTypeLoadException)e).LoaderExceptions[0].Message;
                    _logger.WriteEntry(logEntry);
                }
            }
            return definitions;
        }

        public IEnumerable<TaskDefinition> LoadFromDLL(string dll)
        {
            if (!File.Exists(dll))
                throw new FileNotFoundException("File not found", dll);

            return LoadFromDLL(Assembly.LoadFrom(dll));
        }

        public IEnumerable<TaskDefinition> LoadFromDLL(Assembly dll)
        {
            var taskTypes = from type in dll.GetTypes()
                            where type.IsClass && type.IsSubclassOf(typeof(TaskBase)) && type.IsAbstract == false
                            select type;

            return (from taskType in taskTypes
                    select LoadAttributesAsDomainModel(taskType)).Where(taskDefinition => taskDefinition != null);
        }

        private TaskDefinition LoadAttributesAsDomainModel(Type type)
        {
            object[] taskAttributes = type.GetCustomAttributes(typeof(TaskAttribute), true);
            object[] settingAttributes = type.GetCustomAttributes(typeof(TaskSettingAttribute), true);

            bool typeHasAttributes = (taskAttributes.Count() > 0 && taskAttributes is TaskAttribute[]);
            if (!typeHasAttributes)
                return null;

            return ToDomainModel(((TaskAttribute[])taskAttributes)[0], settingAttributes);
        }

        private TaskDefinition ToDomainModel(TaskAttribute attr, object[] settingAttributes)
        {
            return new TaskDefinition
            {
                Name = attr.Name,
                Author = attr.Author,
                Description = attr.Description,
                Webpage = attr.Webpage,
                Version = attr.Version,
                SettingDefinitions = ToSettingsDomainModel(settingAttributes)
            };
        }

        private List<TaskSettingDefinition> ToSettingsDomainModel(object[] settingAttributes)
        {
            bool hasSettingAttr = (settingAttributes.Count() > 0 && settingAttributes is TaskSettingAttribute[]);
            var settings = hasSettingAttr ? (TaskSettingAttribute[]) settingAttributes : new TaskSettingAttribute[] {};
            return (from setting
                   in settings
                   select new TaskSettingDefinition(setting.IndexOrder, setting.SettingName, setting.Type, setting.DefaultValue)).ToList();
        }
    }
}