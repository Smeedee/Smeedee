﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Smeedee.Client.Web.Tests.ConfigurationRepositoryService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://smeedee.org", ConfigurationName="ConfigurationRepositoryService.ConfigurationRepositoryService")]
    public interface ConfigurationRepositoryService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://smeedee.org/ConfigurationRepositoryService/Get", ReplyAction="http://smeedee.org/ConfigurationRepositoryService/GetResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Smeedee.DomainModel.Config.ConfigurationByName))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Smeedee.DomainModel.Framework.AllSpecification<Smeedee.DomainModel.Config.Configuration>))]
        Smeedee.DomainModel.Config.Configuration[] Get(Smeedee.DomainModel.Framework.Specification<Smeedee.DomainModel.Config.Configuration> specification);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://smeedee.org/ConfigurationRepositoryService/Save", ReplyAction="http://smeedee.org/ConfigurationRepositoryService/SaveResponse")]
        void Save(Smeedee.DomainModel.Config.Configuration[] configurations);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ConfigurationRepositoryServiceChannel : Smeedee.Client.Web.Tests.ConfigurationRepositoryService.ConfigurationRepositoryService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ConfigurationRepositoryServiceClient : System.ServiceModel.ClientBase<Smeedee.Client.Web.Tests.ConfigurationRepositoryService.ConfigurationRepositoryService>, Smeedee.Client.Web.Tests.ConfigurationRepositoryService.ConfigurationRepositoryService {
        
        public ConfigurationRepositoryServiceClient() {
        }
        
        public ConfigurationRepositoryServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ConfigurationRepositoryServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ConfigurationRepositoryServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ConfigurationRepositoryServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public Smeedee.DomainModel.Config.Configuration[] Get(Smeedee.DomainModel.Framework.Specification<Smeedee.DomainModel.Config.Configuration> specification) {
            return base.Channel.Get(specification);
        }
        
        public void Save(Smeedee.DomainModel.Config.Configuration[] configurations) {
            base.Channel.Save(configurations);
        }
    }
}
