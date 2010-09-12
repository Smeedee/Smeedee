﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This code was auto-generated by Microsoft.Silverlight.ServiceReference, version 4.0.50401.0
// 
namespace Smeedee.Client.Framework.SL.ConfigurationRepositoryService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://smeedee.org", ConfigurationName="ConfigurationRepositoryService.ConfigurationRepositoryService")]
    public interface ConfigurationRepositoryService {
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://smeedee.org/ConfigurationRepositoryService/Get", ReplyAction="http://smeedee.org/ConfigurationRepositoryService/GetResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Smeedee.DomainModel.Config.ConfigurationByName))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Smeedee.DomainModel.Framework.AllSpecification<Smeedee.DomainModel.Config.Configuration>))]
        System.IAsyncResult BeginGet(Smeedee.DomainModel.Framework.Specification<Smeedee.DomainModel.Config.Configuration> specification, System.AsyncCallback callback, object asyncState);
        
        System.Collections.Generic.List<Smeedee.DomainModel.Config.Configuration> EndGet(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://smeedee.org/ConfigurationRepositoryService/Save", ReplyAction="http://smeedee.org/ConfigurationRepositoryService/SaveResponse")]
        System.IAsyncResult BeginSave(System.Collections.Generic.List<Smeedee.DomainModel.Config.Configuration> configurations, System.AsyncCallback callback, object asyncState);
        
        void EndSave(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ConfigurationRepositoryServiceChannel : Smeedee.Client.Framework.SL.ConfigurationRepositoryService.ConfigurationRepositoryService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GetCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public GetCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public System.Collections.Generic.List<Smeedee.DomainModel.Config.Configuration> Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((System.Collections.Generic.List<Smeedee.DomainModel.Config.Configuration>)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ConfigurationRepositoryServiceClient : System.ServiceModel.ClientBase<Smeedee.Client.Framework.SL.ConfigurationRepositoryService.ConfigurationRepositoryService>, Smeedee.Client.Framework.SL.ConfigurationRepositoryService.ConfigurationRepositoryService {
        
        private BeginOperationDelegate onBeginGetDelegate;
        
        private EndOperationDelegate onEndGetDelegate;
        
        private System.Threading.SendOrPostCallback onGetCompletedDelegate;
        
        private BeginOperationDelegate onBeginSaveDelegate;
        
        private EndOperationDelegate onEndSaveDelegate;
        
        private System.Threading.SendOrPostCallback onSaveCompletedDelegate;
        
        private BeginOperationDelegate onBeginOpenDelegate;
        
        private EndOperationDelegate onEndOpenDelegate;
        
        private System.Threading.SendOrPostCallback onOpenCompletedDelegate;
        
        private BeginOperationDelegate onBeginCloseDelegate;
        
        private EndOperationDelegate onEndCloseDelegate;
        
        private System.Threading.SendOrPostCallback onCloseCompletedDelegate;
        
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
        
        public System.Net.CookieContainer CookieContainer {
            get {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    return httpCookieContainerManager.CookieContainer;
                }
                else {
                    return null;
                }
            }
            set {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    httpCookieContainerManager.CookieContainer = value;
                }
                else {
                    throw new System.InvalidOperationException("Unable to set the CookieContainer. Please make sure the binding contains an HttpC" +
                            "ookieContainerBindingElement.");
                }
            }
        }
        
        public event System.EventHandler<GetCompletedEventArgs> GetCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> SaveCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> OpenCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> CloseCompleted;
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult Smeedee.Client.Framework.SL.ConfigurationRepositoryService.ConfigurationRepositoryService.BeginGet(Smeedee.DomainModel.Framework.Specification<Smeedee.DomainModel.Config.Configuration> specification, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginGet(specification, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Collections.Generic.List<Smeedee.DomainModel.Config.Configuration> Smeedee.Client.Framework.SL.ConfigurationRepositoryService.ConfigurationRepositoryService.EndGet(System.IAsyncResult result) {
            return base.Channel.EndGet(result);
        }
        
        private System.IAsyncResult OnBeginGet(object[] inValues, System.AsyncCallback callback, object asyncState) {
            Smeedee.DomainModel.Framework.Specification<Smeedee.DomainModel.Config.Configuration> specification = ((Smeedee.DomainModel.Framework.Specification<Smeedee.DomainModel.Config.Configuration>)(inValues[0]));
            return ((Smeedee.Client.Framework.SL.ConfigurationRepositoryService.ConfigurationRepositoryService)(this)).BeginGet(specification, callback, asyncState);
        }
        
        private object[] OnEndGet(System.IAsyncResult result) {
            System.Collections.Generic.List<Smeedee.DomainModel.Config.Configuration> retVal = ((Smeedee.Client.Framework.SL.ConfigurationRepositoryService.ConfigurationRepositoryService)(this)).EndGet(result);
            return new object[] {
                    retVal};
        }
        
        private void OnGetCompleted(object state) {
            if ((this.GetCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.GetCompleted(this, new GetCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void GetAsync(Smeedee.DomainModel.Framework.Specification<Smeedee.DomainModel.Config.Configuration> specification) {
            this.GetAsync(specification, null);
        }
        
        public void GetAsync(Smeedee.DomainModel.Framework.Specification<Smeedee.DomainModel.Config.Configuration> specification, object userState) {
            if ((this.onBeginGetDelegate == null)) {
                this.onBeginGetDelegate = new BeginOperationDelegate(this.OnBeginGet);
            }
            if ((this.onEndGetDelegate == null)) {
                this.onEndGetDelegate = new EndOperationDelegate(this.OnEndGet);
            }
            if ((this.onGetCompletedDelegate == null)) {
                this.onGetCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetCompleted);
            }
            base.InvokeAsync(this.onBeginGetDelegate, new object[] {
                        specification}, this.onEndGetDelegate, this.onGetCompletedDelegate, userState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult Smeedee.Client.Framework.SL.ConfigurationRepositoryService.ConfigurationRepositoryService.BeginSave(System.Collections.Generic.List<Smeedee.DomainModel.Config.Configuration> configurations, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginSave(configurations, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        void Smeedee.Client.Framework.SL.ConfigurationRepositoryService.ConfigurationRepositoryService.EndSave(System.IAsyncResult result) {
            base.Channel.EndSave(result);
        }
        
        private System.IAsyncResult OnBeginSave(object[] inValues, System.AsyncCallback callback, object asyncState) {
            System.Collections.Generic.List<Smeedee.DomainModel.Config.Configuration> configurations = ((System.Collections.Generic.List<Smeedee.DomainModel.Config.Configuration>)(inValues[0]));
            return ((Smeedee.Client.Framework.SL.ConfigurationRepositoryService.ConfigurationRepositoryService)(this)).BeginSave(configurations, callback, asyncState);
        }
        
        private object[] OnEndSave(System.IAsyncResult result) {
            ((Smeedee.Client.Framework.SL.ConfigurationRepositoryService.ConfigurationRepositoryService)(this)).EndSave(result);
            return null;
        }
        
        private void OnSaveCompleted(object state) {
            if ((this.SaveCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.SaveCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void SaveAsync(System.Collections.Generic.List<Smeedee.DomainModel.Config.Configuration> configurations) {
            this.SaveAsync(configurations, null);
        }
        
        public void SaveAsync(System.Collections.Generic.List<Smeedee.DomainModel.Config.Configuration> configurations, object userState) {
            if ((this.onBeginSaveDelegate == null)) {
                this.onBeginSaveDelegate = new BeginOperationDelegate(this.OnBeginSave);
            }
            if ((this.onEndSaveDelegate == null)) {
                this.onEndSaveDelegate = new EndOperationDelegate(this.OnEndSave);
            }
            if ((this.onSaveCompletedDelegate == null)) {
                this.onSaveCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnSaveCompleted);
            }
            base.InvokeAsync(this.onBeginSaveDelegate, new object[] {
                        configurations}, this.onEndSaveDelegate, this.onSaveCompletedDelegate, userState);
        }
        
        private System.IAsyncResult OnBeginOpen(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(callback, asyncState);
        }
        
        private object[] OnEndOpen(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndOpen(result);
            return null;
        }
        
        private void OnOpenCompleted(object state) {
            if ((this.OpenCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.OpenCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void OpenAsync() {
            this.OpenAsync(null);
        }
        
        public void OpenAsync(object userState) {
            if ((this.onBeginOpenDelegate == null)) {
                this.onBeginOpenDelegate = new BeginOperationDelegate(this.OnBeginOpen);
            }
            if ((this.onEndOpenDelegate == null)) {
                this.onEndOpenDelegate = new EndOperationDelegate(this.OnEndOpen);
            }
            if ((this.onOpenCompletedDelegate == null)) {
                this.onOpenCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnOpenCompleted);
            }
            base.InvokeAsync(this.onBeginOpenDelegate, null, this.onEndOpenDelegate, this.onOpenCompletedDelegate, userState);
        }
        
        private System.IAsyncResult OnBeginClose(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginClose(callback, asyncState);
        }
        
        private object[] OnEndClose(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndClose(result);
            return null;
        }
        
        private void OnCloseCompleted(object state) {
            if ((this.CloseCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.CloseCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void CloseAsync() {
            this.CloseAsync(null);
        }
        
        public void CloseAsync(object userState) {
            if ((this.onBeginCloseDelegate == null)) {
                this.onBeginCloseDelegate = new BeginOperationDelegate(this.OnBeginClose);
            }
            if ((this.onEndCloseDelegate == null)) {
                this.onEndCloseDelegate = new EndOperationDelegate(this.OnEndClose);
            }
            if ((this.onCloseCompletedDelegate == null)) {
                this.onCloseCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnCloseCompleted);
            }
            base.InvokeAsync(this.onBeginCloseDelegate, null, this.onEndCloseDelegate, this.onCloseCompletedDelegate, userState);
        }
        
        protected override Smeedee.Client.Framework.SL.ConfigurationRepositoryService.ConfigurationRepositoryService CreateChannel() {
            return new ConfigurationRepositoryServiceClientChannel(this);
        }
        
        private class ConfigurationRepositoryServiceClientChannel : ChannelBase<Smeedee.Client.Framework.SL.ConfigurationRepositoryService.ConfigurationRepositoryService>, Smeedee.Client.Framework.SL.ConfigurationRepositoryService.ConfigurationRepositoryService {
            
            public ConfigurationRepositoryServiceClientChannel(System.ServiceModel.ClientBase<Smeedee.Client.Framework.SL.ConfigurationRepositoryService.ConfigurationRepositoryService> client) : 
                    base(client) {
            }
            
            public System.IAsyncResult BeginGet(Smeedee.DomainModel.Framework.Specification<Smeedee.DomainModel.Config.Configuration> specification, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[1];
                _args[0] = specification;
                System.IAsyncResult _result = base.BeginInvoke("Get", _args, callback, asyncState);
                return _result;
            }
            
            public System.Collections.Generic.List<Smeedee.DomainModel.Config.Configuration> EndGet(System.IAsyncResult result) {
                object[] _args = new object[0];
                System.Collections.Generic.List<Smeedee.DomainModel.Config.Configuration> _result = ((System.Collections.Generic.List<Smeedee.DomainModel.Config.Configuration>)(base.EndInvoke("Get", _args, result)));
                return _result;
            }
            
            public System.IAsyncResult BeginSave(System.Collections.Generic.List<Smeedee.DomainModel.Config.Configuration> configurations, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[1];
                _args[0] = configurations;
                System.IAsyncResult _result = base.BeginInvoke("Save", _args, callback, asyncState);
                return _result;
            }
            
            public void EndSave(System.IAsyncResult result) {
                object[] _args = new object[0];
                base.EndInvoke("Save", _args, result);
            }
        }
    }
}
