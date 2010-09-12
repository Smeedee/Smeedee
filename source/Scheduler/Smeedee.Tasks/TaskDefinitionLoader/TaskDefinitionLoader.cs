using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.TaskDefinition;
using Smeedee.Tasks.Framework;
using Smeedee.Tasks.Framework.TaskAttributes;
using TinyMVVM.Framework;

namespace Smeedee.Tasks.TaskDefinitionLoader
{
    public class TaskDefinitionLoader
    {
        public static IEnumerable<string> GetDlls(string folder)
        {
            return Directory.EnumerateFiles(folder).Where(f => f.EndsWith(".dll"));
        }

        public static IEnumerable<TaskDefinition> LoadFromFolder(string folder)
        {
            var definitions = new List<TaskDefinition>();
            foreach (var dll in GetDlls(folder))
            {
                try
                {
                    definitions.AddRange(LoadFromDll(dll));
                } catch (Exception e)
                {
                    var logEntry = new ErrorLogEntry() {
                        Source = "TaskDefinitionLoader",
                        TimeStamp = DateTime.Now,
                        Message = "Failed to load TaskDefinition from Dll. Exception: " + e.Message
                    };
                    ServiceLocator.Instance.GetInstance<ILog>().WriteEntry(logEntry);
                }
            }
            return definitions;
        }

        public static IEnumerable<TaskDefinition> LoadFromDll(string dll)
        {
            if (!File.Exists(dll))
                throw new FileNotFoundException("File not found", dll);

            Assembly assembly = Assembly.LoadFrom(dll);

            var taskTypes = from type in assembly.GetTypes() 
                            where type.IsClass && type.IsSubclassOf(typeof (TaskBase))
                            select type;

            return (from taskType in taskTypes
                    select LoadAttributesAsDomainModel(taskType)).Where(taskDefinition => taskDefinition != null);
        }

        private static TaskDefinition LoadAttributesAsDomainModel(Type type)
        {
            object[] taskAttributes = type.GetCustomAttributes(typeof(TaskAttribute), true);
            object[] settingAttributes = type.GetCustomAttributes(typeof(TaskSettingAttribute), true);

            bool typeHasAttributes = (taskAttributes.Count() > 0 && taskAttributes is TaskAttribute[]);
            if (!typeHasAttributes)
                return null;

            return ToDomainModel(((TaskAttribute[])taskAttributes)[0], settingAttributes);
        }

        private static TaskDefinition ToDomainModel(TaskAttribute attr, object[] settingAttributes)
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

        private static IEnumerable<TaskSettingDefinition> ToSettingsDomainModel(object[] settingAttributes)
        {
            bool hasSettingAttr = (settingAttributes.Count() > 0 && settingAttributes is TaskSettingAttribute[]);
            var settings = hasSettingAttr ? (TaskSettingAttribute[]) settingAttributes : new TaskSettingAttribute[] {};
            return from setting
                   in settings
                   select new TaskSettingDefinition(setting.SettingName, setting.Type, setting.DefaultValue);
        }
    }
}
