﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StratcolInterface.StratcolRecall {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="https://live.stratcol.co.za/dynamic/administration/serv_manager/sysIntegratorReca" +
        "ll.wsdl", ConfigurationName="StratcolRecall.recallStandardTransPortType")]
    public interface recallStandardTransPortType {
        
        // CODEGEN: Generating message contract since the operation recallStandardTrans is neither RPC nor document wrapped.
        [System.ServiceModel.OperationContractAttribute(Action="recallStandardTrans", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        StratcolInterface.StratcolRecall.recallStandardTransResponse recallStandardTrans(StratcolInterface.StratcolRecall.recallStandardTransRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://live.stratcol.co.za/dynamic/administration/serv_manager/sysIntegratorReca" +
        "ll.wsdl")]
    public partial class authenticateClient : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string keyField;
        
        private string modeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string key {
            get {
                return this.keyField;
            }
            set {
                this.keyField = value;
                this.RaisePropertyChanged("key");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string mode {
            get {
                return this.modeField;
            }
            set {
                this.modeField = value;
                this.RaisePropertyChanged("mode");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://live.stratcol.co.za/dynamic/administration/serv_manager/sysIntegratorReca" +
        "ll.wsdl")]
    public partial class recallTrans : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string file_codeField;
        
        private int received_transField;
        
        private double received_totalField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string file_code {
            get {
                return this.file_codeField;
            }
            set {
                this.file_codeField = value;
                this.RaisePropertyChanged("file_code");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public int received_trans {
            get {
                return this.received_transField;
            }
            set {
                this.received_transField = value;
                this.RaisePropertyChanged("received_trans");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public double received_total {
            get {
                return this.received_totalField;
            }
            set {
                this.received_totalField = value;
                this.RaisePropertyChanged("received_total");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://live.stratcol.co.za/dynamic/administration/serv_manager/sysIntegratorReca" +
        "ll.wsdl")]
    public partial class returnResult : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string messageField;
        
        private string file_codeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string message {
            get {
                return this.messageField;
            }
            set {
                this.messageField = value;
                this.RaisePropertyChanged("message");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string file_code {
            get {
                return this.file_codeField;
            }
            set {
                this.file_codeField = value;
                this.RaisePropertyChanged("file_code");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class recallStandardTransRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="https://live.stratcol.co.za/dynamic/administration/serv_manager/sysIntegratorReca" +
            "ll.wsdl")]
        public StratcolInterface.StratcolRecall.authenticateClient authenticateClient;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="https://live.stratcol.co.za/dynamic/administration/serv_manager/sysIntegratorReca" +
            "ll.wsdl", Order=0)]
        public StratcolInterface.StratcolRecall.recallTrans recallTrans;
        
        public recallStandardTransRequest() {
        }
        
        public recallStandardTransRequest(StratcolInterface.StratcolRecall.authenticateClient authenticateClient, StratcolInterface.StratcolRecall.recallTrans recallTrans) {
            this.authenticateClient = authenticateClient;
            this.recallTrans = recallTrans;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class recallStandardTransResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="https://live.stratcol.co.za/dynamic/administration/serv_manager/sysIntegratorReca" +
            "ll.wsdl", Order=0)]
        public StratcolInterface.StratcolRecall.returnResult returnResult;
        
        public recallStandardTransResponse() {
        }
        
        public recallStandardTransResponse(StratcolInterface.StratcolRecall.returnResult returnResult) {
            this.returnResult = returnResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface recallStandardTransPortTypeChannel : StratcolInterface.StratcolRecall.recallStandardTransPortType, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class recallStandardTransPortTypeClient : System.ServiceModel.ClientBase<StratcolInterface.StratcolRecall.recallStandardTransPortType>, StratcolInterface.StratcolRecall.recallStandardTransPortType {
        
        public recallStandardTransPortTypeClient() {
        }
        
        public recallStandardTransPortTypeClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public recallStandardTransPortTypeClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public recallStandardTransPortTypeClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public recallStandardTransPortTypeClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        StratcolInterface.StratcolRecall.recallStandardTransResponse StratcolInterface.StratcolRecall.recallStandardTransPortType.recallStandardTrans(StratcolInterface.StratcolRecall.recallStandardTransRequest request) {
            return base.Channel.recallStandardTrans(request);
        }
        
        public StratcolInterface.StratcolRecall.returnResult recallStandardTrans(StratcolInterface.StratcolRecall.authenticateClient authenticateClient, StratcolInterface.StratcolRecall.recallTrans recallTrans) {
            StratcolInterface.StratcolRecall.recallStandardTransRequest inValue = new StratcolInterface.StratcolRecall.recallStandardTransRequest();
            inValue.authenticateClient = authenticateClient;
            inValue.recallTrans = recallTrans;
            StratcolInterface.StratcolRecall.recallStandardTransResponse retVal = ((StratcolInterface.StratcolRecall.recallStandardTransPortType)(this)).recallStandardTrans(inValue);
            return retVal.returnResult;
        }
    }
}
