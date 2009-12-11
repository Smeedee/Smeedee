﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4927
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This code was auto-generated by Microsoft.Silverlight.ServiceReference, version 3.0.40818.0
// 
namespace APD.Client.Framework.SL.LogEntryWebserviceRepository {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="", ConfigurationName="LogEntryWebserviceRepository.LogEntryRepositoryService")]
    public interface LogEntryRepositoryService {
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="urn:LogEntryRepositoryService/Log", ReplyAction="urn:LogEntryRepositoryService/LogResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(APD.DomainModel.Framework.Logging.ErrorLogEntry))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(APD.DomainModel.Framework.Logging.WarningLogEntry))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(APD.DomainModel.Framework.Logging.InfoLogEntry))]
        System.IAsyncResult BeginLog(APD.DomainModel.Framework.Logging.LogEntry logEntry, System.AsyncCallback callback, object asyncState);
        
        void EndLog(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="urn:LogEntryRepositoryService/Get", ReplyAction="urn:LogEntryRepositoryService/GetResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(APD.DomainModel.Framework.AllSpecification<APD.DomainModel.Framework.Logging.LogEntry>))]
        System.IAsyncResult BeginGet(APD.DomainModel.Framework.Specification<APD.DomainModel.Framework.Logging.LogEntry> specification, System.AsyncCallback callback, object asyncState);
        
        System.Collections.ObjectModel.ObservableCollection<APD.DomainModel.Framework.Logging.LogEntry> EndGet(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public interface LogEntryRepositoryServiceChannel : APD.Client.Framework.SL.LogEntryWebserviceRepository.LogEntryRepositoryService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public partial class GetCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public GetCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public System.Collections.ObjectModel.ObservableCollection<APD.DomainModel.Framework.Logging.LogEntry> Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((System.Collections.ObjectModel.ObservableCollection<APD.DomainModel.Framework.Logging.LogEntry>)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public partial class LogEntryRepositoryServiceClient : System.ServiceModel.ClientBase<APD.Client.Framework.SL.LogEntryWebserviceRepository.LogEntryRepositoryService>, APD.Client.Framework.SL.LogEntryWebserviceRepository.LogEntryRepositoryService {
        
        private BeginOperationDelegate onBeginLogDelegate;
        
        private EndOperationDelegate onEndLogDelegate;
        
        private System.Threading.SendOrPostCallback onLogCompletedDelegate;
        
        private BeginOperationDelegate onBeginGetDelegate;
        
        private EndOperationDelegate onEndGetDelegate;
        
        private System.Threading.SendOrPostCallback onGetCompletedDelegate;
        
        private BeginOperationDelegate onBeginOpenDelegate;
        
        private EndOperationDelegate onEndOpenDelegate;
        
        private System.Threading.SendOrPostCallback onOpenCompletedDelegate;
        
        private BeginOperationDelegate onBeginCloseDelegate;
        
        private EndOperationDelegate onEndCloseDelegate;
        
        private System.Threading.SendOrPostCallback onCloseCompletedDelegate;
        
        public LogEntryRepositoryServiceClient() {
        }
        
        public LogEntryRepositoryServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public LogEntryRepositoryServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LogEntryRepositoryServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LogEntryRepositoryServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
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
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> LogCompleted;
        
        public event System.EventHandler<GetCompletedEventArgs> GetCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> OpenCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> CloseCompleted;
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult APD.Client.Framework.SL.LogEntryWebserviceRepository.LogEntryRepositoryService.BeginLog(APD.DomainModel.Framework.Logging.LogEntry logEntry, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginLog(logEntry, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        void APD.Client.Framework.SL.LogEntryWebserviceRepository.LogEntryRepositoryService.EndLog(System.IAsyncResult result) {
            base.Channel.EndLog(result);
        }
        
        private System.IAsyncResult OnBeginLog(object[] inValues, System.AsyncCallback callback, object asyncState) {
            APD.DomainModel.Framework.Logging.LogEntry logEntry = ((APD.DomainModel.Framework.Logging.LogEntry)(inValues[0]));
            return ((APD.Client.Framework.SL.LogEntryWebserviceRepository.LogEntryRepositoryService)(this)).BeginLog(logEntry, callback, asyncState);
        }
        
        private object[] OnEndLog(System.IAsyncResult result) {
            ((APD.Client.Framework.SL.LogEntryWebserviceRepository.LogEntryRepositoryService)(this)).EndLog(result);
            return null;
        }
        
        private void OnLogCompleted(object state) {
            if ((this.LogCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.LogCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void LogAsync(APD.DomainModel.Framework.Logging.LogEntry logEntry) {
            this.LogAsync(logEntry, null);
        }
        
        public void LogAsync(APD.DomainModel.Framework.Logging.LogEntry logEntry, object userState) {
            if ((this.onBeginLogDelegate == null)) {
                this.onBeginLogDelegate = new BeginOperationDelegate(this.OnBeginLog);
            }
            if ((this.onEndLogDelegate == null)) {
                this.onEndLogDelegate = new EndOperationDelegate(this.OnEndLog);
            }
            if ((this.onLogCompletedDelegate == null)) {
                this.onLogCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnLogCompleted);
            }
            base.InvokeAsync(this.onBeginLogDelegate, new object[] {
                        logEntry}, this.onEndLogDelegate, this.onLogCompletedDelegate, userState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult APD.Client.Framework.SL.LogEntryWebserviceRepository.LogEntryRepositoryService.BeginGet(APD.DomainModel.Framework.Specification<APD.DomainModel.Framework.Logging.LogEntry> specification, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginGet(specification, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Collections.ObjectModel.ObservableCollection<APD.DomainModel.Framework.Logging.LogEntry> APD.Client.Framework.SL.LogEntryWebserviceRepository.LogEntryRepositoryService.EndGet(System.IAsyncResult result) {
            return base.Channel.EndGet(result);
        }
        
        private System.IAsyncResult OnBeginGet(object[] inValues, System.AsyncCallback callback, object asyncState) {
            APD.DomainModel.Framework.Specification<APD.DomainModel.Framework.Logging.LogEntry> specification = ((APD.DomainModel.Framework.Specification<APD.DomainModel.Framework.Logging.LogEntry>)(inValues[0]));
            return ((APD.Client.Framework.SL.LogEntryWebserviceRepository.LogEntryRepositoryService)(this)).BeginGet(specification, callback, asyncState);
        }
        
        private object[] OnEndGet(System.IAsyncResult result) {
            System.Collections.ObjectModel.ObservableCollection<APD.DomainModel.Framework.Logging.LogEntry> retVal = ((APD.Client.Framework.SL.LogEntryWebserviceRepository.LogEntryRepositoryService)(this)).EndGet(result);
            return new object[] {
                    retVal};
        }
        
        private void OnGetCompleted(object state) {
            if ((this.GetCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.GetCompleted(this, new GetCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void GetAsync(APD.DomainModel.Framework.Specification<APD.DomainModel.Framework.Logging.LogEntry> specification) {
            this.GetAsync(specification, null);
        }
        
        public void GetAsync(APD.DomainModel.Framework.Specification<APD.DomainModel.Framework.Logging.LogEntry> specification, object userState) {
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
        
        protected override APD.Client.Framework.SL.LogEntryWebserviceRepository.LogEntryRepositoryService CreateChannel() {
            return new LogEntryRepositoryServiceClientChannel(this);
        }
        
        private class LogEntryRepositoryServiceClientChannel : ChannelBase<APD.Client.Framework.SL.LogEntryWebserviceRepository.LogEntryRepositoryService>, APD.Client.Framework.SL.LogEntryWebserviceRepository.LogEntryRepositoryService {
            
            public LogEntryRepositoryServiceClientChannel(System.ServiceModel.ClientBase<APD.Client.Framework.SL.LogEntryWebserviceRepository.LogEntryRepositoryService> client) : 
                    base(client) {
            }
            
            public System.IAsyncResult BeginLog(APD.DomainModel.Framework.Logging.LogEntry logEntry, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[1];
                _args[0] = logEntry;
                System.IAsyncResult _result = base.BeginInvoke("Log", _args, callback, asyncState);
                return _result;
            }
            
            public void EndLog(System.IAsyncResult result) {
                object[] _args = new object[0];
                base.EndInvoke("Log", _args, result);
            }
            
            public System.IAsyncResult BeginGet(APD.DomainModel.Framework.Specification<APD.DomainModel.Framework.Logging.LogEntry> specification, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[1];
                _args[0] = specification;
                System.IAsyncResult _result = base.BeginInvoke("Get", _args, callback, asyncState);
                return _result;
            }
            
            public System.Collections.ObjectModel.ObservableCollection<APD.DomainModel.Framework.Logging.LogEntry> EndGet(System.IAsyncResult result) {
                object[] _args = new object[0];
                System.Collections.ObjectModel.ObservableCollection<APD.DomainModel.Framework.Logging.LogEntry> _result = ((System.Collections.ObjectModel.ObservableCollection<APD.DomainModel.Framework.Logging.LogEntry>)(base.EndInvoke("Get", _args, result)));
                return _result;
            }
        }
    }
}