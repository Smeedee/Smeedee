using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Smeedee.DomainModel.TaskDefinition
{
    [DataContract]
    public class TaskDefinition
    {
        [DataMember]
        public virtual String Name { get; set; }

        [DataMember]
        public virtual String Description { get; set; }

        [DataMember]
        public virtual long Version { get; set; }

        [DataMember]
        public virtual String Author { get; set; }

        [DataMember]
        public virtual String Webpage { get; set; }

        [DataMember]
        public virtual List<TaskSettingDefinition> SettingDefinitions { get; set; }
    }

    [DataContract]
    public class TaskSettingDefinition
    {
        [DataMember]
        public string SettingName { get; set; }

        [DataMember] 
        public string type;

        public Type Type 
        { 
            get
            {
                return Type.GetType(type);
            } 
            set
            {
                type = value.FullName;
            } 
        }

        [DataMember]
        public string DefaultValue { get; set; }

        [DataMember]
        public int OrderIndex { get; set; }

        public TaskSettingDefinition(string name, Type type)
        {
            SettingName = name;
            Type = type;
        }

        public TaskSettingDefinition(string name, Type type, string defaultValue) 
            : this(name, type)
        {
            DefaultValue = defaultValue;
        }

        public TaskSettingDefinition(int orderIndex, string name, Type type)
            : this(name, type)
        {
            OrderIndex = orderIndex;
        }

        public TaskSettingDefinition(int orderIndex, string name, Type type, string defaultValue)
            : this(name, type, defaultValue)
        {
            OrderIndex = orderIndex;
        }


    }
}