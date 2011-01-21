using System;
using System.ComponentModel;

namespace Smeedee.Tasks.Framework.TaskAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class TaskAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long Version { get; set; }
        public string Author { get; set; }
        public string Webpage { get; set; }

        public TaskAttribute(string taskName)
        {
            Name = taskName;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TaskSettingAttribute : Attribute
    {

        
        public int IndexOrder
        {
            get
            {
                return _indexOrder;  
            } 
            set
            {
                _indexOrder = value;
            }
        }

        private int _indexOrder = int.MaxValue;

        public string SettingName { get; set; }
        public Type Type { get; set; }
        public string DefaultValue { get; set; }
        public string HelpText { get; set; }

        public TaskSettingAttribute(string name, Type T, string defaultValue)
        {
            SettingName = name;
            Type = T;
            DefaultValue = defaultValue;
        }

        public TaskSettingAttribute(string name, Type T, string defaultValue, string helpText)
            : this(name, T, defaultValue)
        {
            HelpText = helpText;
        }

        public TaskSettingAttribute(int indexOrder, string name, Type T, string defaultValue) 
            : this(name, T, defaultValue)
        {
            IndexOrder = indexOrder;
        }

        public TaskSettingAttribute(int indexOrder, string name, Type T, string defaultValue, string helpText)
            : this(indexOrder, name, T, defaultValue)
        {
            HelpText = helpText;
        }
    }
}
