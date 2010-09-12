using System;

namespace Smeedee.DomainModel.TaskInstanceConfiguration
{
    public class TaskConfigurationException : Exception
    {
        public TaskConfigurationException()
            : base("The configuration is not correct")
        {
            
        }

        public TaskConfigurationException(string message)
            : base(message)
        {
        }
    }
}