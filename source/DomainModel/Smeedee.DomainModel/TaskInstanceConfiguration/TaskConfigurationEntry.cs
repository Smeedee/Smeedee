using System;
using System.Runtime.Serialization;

namespace Smeedee.DomainModel.TaskInstanceConfiguration
{
    [DataContract]
    public class TaskConfigurationEntry
    {

        public TaskConfigurationEntry()
        {
            PersistableType = typeof (string).AssemblyQualifiedName;
            PersistableValue = "";
        }
        public virtual TaskConfiguration TaskConfiguration { get; set; }

        protected string PersistableValue { get; set; }

        private string  persistableType;

        [DataMember]
        public virtual string PersistableType
        {
            get
            {
                return persistableType;
            }
            set
            {
                persistableType = value;
            }
        }

        public virtual Type Type
        {
            get { return Type.GetType(PersistableType); } 
            set { PersistableType = value.AssemblyQualifiedName; } 
        }

        [DataMember]
        public virtual string Name { get; set; }

        [DataMember]
        public virtual object Value
        {
            get { return PersistableValue.ToObject(Type); }
            set { PersistableValue = value.ToPersistableValue(); } 
        }

        [DataMember]
        public virtual int OrderIndex { get; set; }

        [DataMember]
        public virtual string HelpText { get; set; }

        #region Overrides of GetHashCode and Equals. Needed to satisfy Hibernate

        public override int GetHashCode()
        {
            return Name.GetHashCode() + Value.GetHashCode() + Type.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var taskEntry = (TaskConfigurationEntry) obj;
            return Name == taskEntry.Name;
        }

        #endregion
    }

    public static class TaskConfigurationExtensions
    {
        public static object ToObject(this string value, Type type)
        {
            switch (type.FullName)
            {
                case "System.Int32":
                    return int.Parse(value);
                case "System.Int64":
                    return long.Parse(value);
                case "System.Single":
                    return float.Parse(value);
                case "System.Double":
                    return double.Parse(value);
                case "System.DateTime":
                    return DateTime.Parse(value);
                case "System.TimeSpan":
                    return TimeSpan.Parse(value);
                case "System.Boolean":
                    return bool.Parse(value);
                case "System.Uri":
                    return new Uri(value);
            }

            return value;
        }

        public static string ToPersistableValue(this object value)
        {
            return (value == null) ? null : value.ToString();
        }
    }
}