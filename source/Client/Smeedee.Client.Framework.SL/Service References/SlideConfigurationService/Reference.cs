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
namespace Smeedee.Client.Framework.SL.SlideConfigurationService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="", ConfigurationName="SlideConfigurationService.SlideConfigurationRepositoryService")]
    public interface SlideConfigurationRepositoryService {
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="urn:SlideConfigurationRepositoryService/Get", ReplyAction="urn:SlideConfigurationRepositoryService/GetResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Smeedee.DomainModel.Framework.AllSpecification<Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration>))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Smeedee.DomainModel.Config.SlideConfig.SlideConfigurationByIdSpecification))]
        System.IAsyncResult BeginGet(Smeedee.DomainModel.Framework.Specification<Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration> specification, System.AsyncCallback callback, object asyncState);
        
        System.Collections.Generic.List<Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration> EndGet(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="urn:SlideConfigurationRepositoryService/Save", ReplyAction="urn:SlideConfigurationRepositoryService/SaveResponse")]
        System.IAsyncResult BeginSave(Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration configuration, System.AsyncCallback callback, object asyncState);
        
        void EndSave(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="urn:SlideConfigurationRepositoryService/SaveAll", ReplyAction="urn:SlideConfigurationRepositoryService/SaveAllResponse")]
        System.IAsyncResult BeginSaveAll(System.Collections.Generic.List<Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration> configs, System.AsyncCallback callback, object asyncState);
        
        void EndSaveAll(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="urn:SlideConfigurationRepositoryService/Delete", ReplyAction="urn:SlideConfigurationRepositoryService/DeleteResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Smeedee.DomainModel.Framework.AllSpecification<Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration>))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Smeedee.DomainModel.Config.SlideConfig.SlideConfigurationByIdSpecification))]
        System.IAsyncResult BeginDelete(Smeedee.DomainModel.Framework.Specification<Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration> specification, System.AsyncCallback callback, object asyncState);
        
        void EndDelete(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface SlideConfigurationRepositoryServiceChannel : Smeedee.Client.Framework.SL.SlideConfigurationService.SlideConfigurationRepositoryService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GetCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public GetCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public System.Collections.Generic.List<Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration> Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((System.Collections.Generic.List<Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration>)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class SlideConfigurationRepositoryServiceClient : System.ServiceModel.ClientBase<Smeedee.Client.Framework.SL.SlideConfigurationService.SlideConfigurationRepositoryService>, Smeedee.Client.Framework.SL.SlideConfigurationService.SlideConfigurationRepositoryService {
        
        private BeginOperationDelegate onBeginGetDelegate;
        
        private EndOperationDelegate onEndGetDelegate;
        
        private System.Threading.SendOrPostCallback onGetCompletedDelegate;
        
        private BeginOperationDelegate onBeginSaveDelegate;
        
        private EndOperationDelegate onEndSaveDelegate;
        
        private System.Threading.SendOrPostCallback onSaveCompletedDelegate;
        
        private BeginOperationDelegate onBeginSaveAllDelegate;
        
        private EndOperationDelegate onEndSaveAllDelegate;
        
        private System.Threading.SendOrPostCallback onSaveAllCompletedDelegate;
        
        private BeginOperationDelegate onBeginDeleteDelegate;
        
        private EndOperationDelegate onEndDeleteDelegate;
        
        private System.Threading.SendOrPostCallback onDeleteCompletedDelegate;
        
        private BeginOperationDelegate onBeginOpenDelegate;
        
        private EndOperationDelegate onEndOpenDelegate;
        
        private System.Threading.SendOrPostCallback onOpenCompletedDelegate;
        
        private BeginOperationDelegate onBeginCloseDelegate;
        
        private EndOperationDelegate onEndCloseDelegate;
        
        private System.Threading.SendOrPostCallback onCloseCompletedDelegate;
        
        public SlideConfigurationRepositoryServiceClient() {
        }
        
        public SlideConfigurationRepositoryServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public SlideConfigurationRepositoryServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SlideConfigurationRepositoryServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SlideConfigurationRepositoryServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
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
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> SaveAllCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> DeleteCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> OpenCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> CloseCompleted;
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult Smeedee.Client.Framework.SL.SlideConfigurationService.SlideConfigurationRepositoryService.BeginGet(Smeedee.DomainModel.Framework.Specification<Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration> specification, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginGet(specification, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Collections.Generic.List<Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration> Smeedee.Client.Framework.SL.SlideConfigurationService.SlideConfigurationRepositoryService.EndGet(System.IAsyncResult result) {
            return base.Channel.EndGet(result);
        }
        
        private System.IAsyncResult OnBeginGet(object[] inValues, System.AsyncCallback callback, object asyncState) {
            Smeedee.DomainModel.Framework.Specification<Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration> specification = ((Smeedee.DomainModel.Framework.Specification<Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration>)(inValues[0]));
            return ((Smeedee.Client.Framework.SL.SlideConfigurationService.SlideConfigurationRepositoryService)(this)).BeginGet(specification, callback, asyncState);
        }
        
        private object[] OnEndGet(System.IAsyncResult result) {
            System.Collections.Generic.List<Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration> retVal = ((Smeedee.Client.Framework.SL.SlideConfigurationService.SlideConfigurationRepositoryService)(this)).EndGet(result);
            return new object[] {
                    retVal};
        }
        
        private void OnGetCompleted(object state) {
            if ((this.GetCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.GetCompleted(this, new GetCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void GetAsync(Smeedee.DomainModel.Framework.Specification<Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration> specification) {
            this.GetAsync(specification, null);
        }
        
        public void GetAsync(Smeedee.DomainModel.Framework.Specification<Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration> specification, object userState) {
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
        System.IAsyncResult Smeedee.Client.Framework.SL.SlideConfigurationService.SlideConfigurationRepositoryService.BeginSave(Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration configuration, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginSave(configuration, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        void Smeedee.Client.Framework.SL.SlideConfigurationService.SlideConfigurationRepositoryService.EndSave(System.IAsyncResult result) {
            base.Channel.EndSave(result);
        }
        
        private System.IAsyncResult OnBeginSave(object[] inValues, System.AsyncCallback callback, object asyncState) {
            Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration configuration = ((Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration)(inValues[0]));
            return ((Smeedee.Client.Framework.SL.SlideConfigurationService.SlideConfigurationRepositoryService)(this)).BeginSave(configuration, callback, asyncState);
        }
        
        private object[] OnEndSave(System.IAsyncResult result) {
            ((Smeedee.Client.Framework.SL.SlideConfigurationService.SlideConfigurationRepositoryService)(this)).EndSave(result);
            return null;
        }
        
        private void OnSaveCompleted(object state) {
            if ((this.SaveCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.SaveCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void SaveAsync(Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration configuration) {
            this.SaveAsync(configuration, null);
        }
        
        public void SaveAsync(Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration configuration, object userState) {
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
                        configuration}, this.onEndSaveDelegate, this.onSaveCompletedDelegate, userState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult Smeedee.Client.Framework.SL.SlideConfigurationService.SlideConfigurationRepositoryService.BeginSaveAll(System.Collections.Generic.List<Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration> configs, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginSaveAll(configs, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        void Smeedee.Client.Framework.SL.SlideConfigurationService.SlideConfigurationRepositoryService.EndSaveAll(System.IAsyncResult result) {
            base.Channel.EndSaveAll(result);
        }
        
        private System.IAsyncResult OnBeginSaveAll(object[] inValues, System.AsyncCallback callback, object asyncState) {
            System.Collections.Generic.List<Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration> configs = ((System.Collections.Generic.List<Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration>)(inValues[0]));
            return ((Smeedee.Client.Framework.SL.SlideConfigurationService.SlideConfigurationRepositoryService)(this)).BeginSaveAll(configs, callback, asyncState);
        }
        
        private object[] OnEndSaveAll(System.IAsyncResult result) {
            ((Smeedee.Client.Framework.SL.SlideConfigurationService.SlideConfigurationRepositoryService)(this)).EndSaveAll(result);
            return null;
        }
        
        private void OnSaveAllCompleted(object state) {
            if ((this.SaveAllCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.SaveAllCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void SaveAllAsync(System.Collections.Generic.List<Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration> configs) {
            this.SaveAllAsync(configs, null);
        }
        
        public void SaveAllAsync(System.Collections.Generic.List<Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration> configs, object userState) {
            if ((this.onBeginSaveAllDelegate == null)) {
                this.onBeginSaveAllDelegate = new BeginOperationDelegate(this.OnBeginSaveAll);
            }
            if ((this.onEndSaveAllDelegate == null)) {
                this.onEndSaveAllDelegate = new EndOperationDelegate(this.OnEndSaveAll);
            }
            if ((this.onSaveAllCompletedDelegate == null)) {
                this.onSaveAllCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnSaveAllCompleted);
            }
            base.InvokeAsync(this.onBeginSaveAllDelegate, new object[] {
                        configs}, this.onEndSaveAllDelegate, this.onSaveAllCompletedDelegate, userState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult Smeedee.Client.Framework.SL.SlideConfigurationService.SlideConfigurationRepositoryService.BeginDelete(Smeedee.DomainModel.Framework.Specification<Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration> specification, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginDelete(specification, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        void Smeedee.Client.Framework.SL.SlideConfigurationService.SlideConfigurationRepositoryService.EndDelete(System.IAsyncResult result) {
            base.Channel.EndDelete(result);
        }
        
        private System.IAsyncResult OnBeginDelete(object[] inValues, System.AsyncCallback callback, object asyncState) {
            Smeedee.DomainModel.Framework.Specification<Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration> specification = ((Smeedee.DomainModel.Framework.Specification<Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration>)(inValues[0]));
            return ((Smeedee.Client.Framework.SL.SlideConfigurationService.SlideConfigurationRepositoryService)(this)).BeginDelete(specification, callback, asyncState);
        }
        
        private object[] OnEndDelete(System.IAsyncResult result) {
            ((Smeedee.Client.Framework.SL.SlideConfigurationService.SlideConfigurationRepositoryService)(this)).EndDelete(result);
            return null;
        }
        
        private void OnDeleteCompleted(object state) {
            if ((this.DeleteCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.DeleteCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void DeleteAsync(Smeedee.DomainModel.Framework.Specification<Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration> specification) {
            this.DeleteAsync(specification, null);
        }
        
        public void DeleteAsync(Smeedee.DomainModel.Framework.Specification<Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration> specification, object userState) {
            if ((this.onBeginDeleteDelegate == null)) {
                this.onBeginDeleteDelegate = new BeginOperationDelegate(this.OnBeginDelete);
            }
            if ((this.onEndDeleteDelegate == null)) {
                this.onEndDeleteDelegate = new EndOperationDelegate(this.OnEndDelete);
            }
            if ((this.onDeleteCompletedDelegate == null)) {
                this.onDeleteCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnDeleteCompleted);
            }
            base.InvokeAsync(this.onBeginDeleteDelegate, new object[] {
                        specification}, this.onEndDeleteDelegate, this.onDeleteCompletedDelegate, userState);
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
        
        protected override Smeedee.Client.Framework.SL.SlideConfigurationService.SlideConfigurationRepositoryService CreateChannel() {
            return new SlideConfigurationRepositoryServiceClientChannel(this);
        }
        
        private class SlideConfigurationRepositoryServiceClientChannel : ChannelBase<Smeedee.Client.Framework.SL.SlideConfigurationService.SlideConfigurationRepositoryService>, Smeedee.Client.Framework.SL.SlideConfigurationService.SlideConfigurationRepositoryService {
            
            public SlideConfigurationRepositoryServiceClientChannel(System.ServiceModel.ClientBase<Smeedee.Client.Framework.SL.SlideConfigurationService.SlideConfigurationRepositoryService> client) : 
                    base(client) {
            }
            
            public System.IAsyncResult BeginGet(Smeedee.DomainModel.Framework.Specification<Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration> specification, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[1];
                _args[0] = specification;
                System.IAsyncResult _result = base.BeginInvoke("Get", _args, callback, asyncState);
                return _result;
            }
            
            public System.Collections.Generic.List<Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration> EndGet(System.IAsyncResult result) {
                object[] _args = new object[0];
                System.Collections.Generic.List<Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration> _result = ((System.Collections.Generic.List<Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration>)(base.EndInvoke("Get", _args, result)));
                return _result;
            }
            
            public System.IAsyncResult BeginSave(Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration configuration, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[1];
                _args[0] = configuration;
                System.IAsyncResult _result = base.BeginInvoke("Save", _args, callback, asyncState);
                return _result;
            }
            
            public void EndSave(System.IAsyncResult result) {
                object[] _args = new object[0];
                base.EndInvoke("Save", _args, result);
            }
            
            public System.IAsyncResult BeginSaveAll(System.Collections.Generic.List<Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration> configs, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[1];
                _args[0] = configs;
                System.IAsyncResult _result = base.BeginInvoke("SaveAll", _args, callback, asyncState);
                return _result;
            }
            
            public void EndSaveAll(System.IAsyncResult result) {
                object[] _args = new object[0];
                base.EndInvoke("SaveAll", _args, result);
            }
            
            public System.IAsyncResult BeginDelete(Smeedee.DomainModel.Framework.Specification<Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration> specification, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[1];
                _args[0] = specification;
                System.IAsyncResult _result = base.BeginInvoke("Delete", _args, callback, asyncState);
                return _result;
            }
            
            public void EndDelete(System.IAsyncResult result) {
                object[] _args = new object[0];
                base.EndInvoke("Delete", _args, result);
            }
        }
    }
}
