﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StratcolInterface.StratcolInput {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="https://live.stratcol.co.za/dynamic/administration/serv_manager/sysIntegratorInpu" +
        "t.wsdl", ConfigurationName="StratcolInput.addStandardTransPortType")]
    public interface addStandardTransPortType {
        
        // CODEGEN: Generating message contract since the operation addStandardTrans is neither RPC nor document wrapped.
        [System.ServiceModel.OperationContractAttribute(Action="addStandardTrans", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        StratcolInterface.StratcolInput.addStandardTransResponse addStandardTrans(StratcolInterface.StratcolInput.addStandardTransRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://live.stratcol.co.za/dynamic/administration/serv_manager/sysIntegratorInpu" +
        "t.wsdl")]
    public partial class batchHeader : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string user_idField;
        
        private string batch_refField;
        
        private int total_transField;
        
        private double total_valueField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string user_id {
            get {
                return this.user_idField;
            }
            set {
                this.user_idField = value;
                this.RaisePropertyChanged("user_id");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string batch_ref {
            get {
                return this.batch_refField;
            }
            set {
                this.batch_refField = value;
                this.RaisePropertyChanged("batch_ref");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public int total_trans {
            get {
                return this.total_transField;
            }
            set {
                this.total_transField = value;
                this.RaisePropertyChanged("total_trans");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public double total_value {
            get {
                return this.total_valueField;
            }
            set {
                this.total_valueField = value;
                this.RaisePropertyChanged("total_value");
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://live.stratcol.co.za/dynamic/administration/serv_manager/sysIntegratorInpu" +
        "t.wsdl")]
    public partial class trans : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string stc_refField;
        
        private string user_refField;
        
        private string surnameField;
        
        private string initialsField;
        
        private string mobile_noField;
        
        private string account_nameField;
        
        private string branch_codeField;
        
        private string id_reg_noField;
        
        private string account_noField;
        
        private int account_typeField;
        
        private string ccard_ssvField;
        
        private string ccard_expField;
        
        private string user_idField;
        
        private string start_dateField;
        
        private double amountField;
        
        private string tran_typeField;
        
        private string day_of_monthField;
        
        private string continueField;
        
        private string no_of_deductField;
        
        private string final_dateField;
        
        private int escalation_percField;
        
        private string escalation_monthField;
        
        private string publication_refField;
        
        private string batch_refField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string stc_ref {
            get {
                return this.stc_refField;
            }
            set {
                this.stc_refField = value;
                this.RaisePropertyChanged("stc_ref");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string user_ref {
            get {
                return this.user_refField;
            }
            set {
                this.user_refField = value;
                this.RaisePropertyChanged("user_ref");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string surname {
            get {
                return this.surnameField;
            }
            set {
                this.surnameField = value;
                this.RaisePropertyChanged("surname");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public string initials {
            get {
                return this.initialsField;
            }
            set {
                this.initialsField = value;
                this.RaisePropertyChanged("initials");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=4)]
        public string mobile_no {
            get {
                return this.mobile_noField;
            }
            set {
                this.mobile_noField = value;
                this.RaisePropertyChanged("mobile_no");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=5)]
        public string account_name {
            get {
                return this.account_nameField;
            }
            set {
                this.account_nameField = value;
                this.RaisePropertyChanged("account_name");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=6)]
        public string branch_code {
            get {
                return this.branch_codeField;
            }
            set {
                this.branch_codeField = value;
                this.RaisePropertyChanged("branch_code");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=7)]
        public string id_reg_no {
            get {
                return this.id_reg_noField;
            }
            set {
                this.id_reg_noField = value;
                this.RaisePropertyChanged("id_reg_no");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=8)]
        public string account_no {
            get {
                return this.account_noField;
            }
            set {
                this.account_noField = value;
                this.RaisePropertyChanged("account_no");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=9)]
        public int account_type {
            get {
                return this.account_typeField;
            }
            set {
                this.account_typeField = value;
                this.RaisePropertyChanged("account_type");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=10)]
        public string ccard_ssv {
            get {
                return this.ccard_ssvField;
            }
            set {
                this.ccard_ssvField = value;
                this.RaisePropertyChanged("ccard_ssv");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=11)]
        public string ccard_exp {
            get {
                return this.ccard_expField;
            }
            set {
                this.ccard_expField = value;
                this.RaisePropertyChanged("ccard_exp");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=12)]
        public string user_id {
            get {
                return this.user_idField;
            }
            set {
                this.user_idField = value;
                this.RaisePropertyChanged("user_id");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=13)]
        public string start_date {
            get {
                return this.start_dateField;
            }
            set {
                this.start_dateField = value;
                this.RaisePropertyChanged("start_date");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=14)]
        public double amount {
            get {
                return this.amountField;
            }
            set {
                this.amountField = value;
                this.RaisePropertyChanged("amount");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=15)]
        public string tran_type {
            get {
                return this.tran_typeField;
            }
            set {
                this.tran_typeField = value;
                this.RaisePropertyChanged("tran_type");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=16)]
        public string day_of_month {
            get {
                return this.day_of_monthField;
            }
            set {
                this.day_of_monthField = value;
                this.RaisePropertyChanged("day_of_month");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=17)]
        public string @continue {
            get {
                return this.continueField;
            }
            set {
                this.continueField = value;
                this.RaisePropertyChanged("continue");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=18)]
        public string no_of_deduct {
            get {
                return this.no_of_deductField;
            }
            set {
                this.no_of_deductField = value;
                this.RaisePropertyChanged("no_of_deduct");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=19)]
        public string final_date {
            get {
                return this.final_dateField;
            }
            set {
                this.final_dateField = value;
                this.RaisePropertyChanged("final_date");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=20)]
        public int escalation_perc {
            get {
                return this.escalation_percField;
            }
            set {
                this.escalation_percField = value;
                this.RaisePropertyChanged("escalation_perc");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=21)]
        public string escalation_month {
            get {
                return this.escalation_monthField;
            }
            set {
                this.escalation_monthField = value;
                this.RaisePropertyChanged("escalation_month");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=22)]
        public string publication_ref {
            get {
                return this.publication_refField;
            }
            set {
                this.publication_refField = value;
                this.RaisePropertyChanged("publication_ref");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=23)]
        public string batch_ref {
            get {
                return this.batch_refField;
            }
            set {
                this.batch_refField = value;
                this.RaisePropertyChanged("batch_ref");
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://live.stratcol.co.za/dynamic/administration/serv_manager/sysIntegratorInpu" +
        "t.wsdl")]
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://live.stratcol.co.za/dynamic/administration/serv_manager/sysIntegratorInpu" +
        "t.wsdl")]
    public partial class result : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string messageField;
        
        private string file_codeField;
        
        private int header_transField;
        
        private double header_totalField;
        
        private int received_transField;
        
        private double received_totalField;
        
        private int uploaded_successfulField;
        
        private int uploaded_rejectedField;
        
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
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public int header_trans {
            get {
                return this.header_transField;
            }
            set {
                this.header_transField = value;
                this.RaisePropertyChanged("header_trans");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public double header_total {
            get {
                return this.header_totalField;
            }
            set {
                this.header_totalField = value;
                this.RaisePropertyChanged("header_total");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=4)]
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
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=5)]
        public double received_total {
            get {
                return this.received_totalField;
            }
            set {
                this.received_totalField = value;
                this.RaisePropertyChanged("received_total");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=6)]
        public int uploaded_successful {
            get {
                return this.uploaded_successfulField;
            }
            set {
                this.uploaded_successfulField = value;
                this.RaisePropertyChanged("uploaded_successful");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=7)]
        public int uploaded_rejected {
            get {
                return this.uploaded_rejectedField;
            }
            set {
                this.uploaded_rejectedField = value;
                this.RaisePropertyChanged("uploaded_rejected");
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
    public partial class addStandardTransRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="https://live.stratcol.co.za/dynamic/administration/serv_manager/sysIntegratorInpu" +
            "t.wsdl")]
        public StratcolInterface.StratcolInput.batchHeader batchHeader;
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="https://live.stratcol.co.za/dynamic/administration/serv_manager/sysIntegratorInpu" +
            "t.wsdl")]
        public StratcolInterface.StratcolInput.authenticateClient authenticateClient;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="https://live.stratcol.co.za/dynamic/administration/serv_manager/sysIntegratorInpu" +
            "t.wsdl", Order=0)]
        [System.Xml.Serialization.XmlArrayItemAttribute("tranDetails", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public StratcolInterface.StratcolInput.trans[] arrayOfTrans;
        
        public addStandardTransRequest() {
        }
        
        public addStandardTransRequest(StratcolInterface.StratcolInput.batchHeader batchHeader, StratcolInterface.StratcolInput.authenticateClient authenticateClient, StratcolInterface.StratcolInput.trans[] arrayOfTrans) {
            this.batchHeader = batchHeader;
            this.authenticateClient = authenticateClient;
            this.arrayOfTrans = arrayOfTrans;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class addStandardTransResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="https://live.stratcol.co.za/dynamic/administration/serv_manager/sysIntegratorInpu" +
            "t.wsdl", Order=0)]
        public StratcolInterface.StratcolInput.result result;
        
        public addStandardTransResponse() {
        }
        
        public addStandardTransResponse(StratcolInterface.StratcolInput.result result) {
            this.result = result;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface addStandardTransPortTypeChannel : StratcolInterface.StratcolInput.addStandardTransPortType, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class addStandardTransPortTypeClient : System.ServiceModel.ClientBase<StratcolInterface.StratcolInput.addStandardTransPortType>, StratcolInterface.StratcolInput.addStandardTransPortType {
        
        public addStandardTransPortTypeClient() {
        }
        
        public addStandardTransPortTypeClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public addStandardTransPortTypeClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public addStandardTransPortTypeClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public addStandardTransPortTypeClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        StratcolInterface.StratcolInput.addStandardTransResponse StratcolInterface.StratcolInput.addStandardTransPortType.addStandardTrans(StratcolInterface.StratcolInput.addStandardTransRequest request) {
            return base.Channel.addStandardTrans(request);
        }
        
        public StratcolInterface.StratcolInput.result addStandardTrans(StratcolInterface.StratcolInput.batchHeader batchHeader, StratcolInterface.StratcolInput.authenticateClient authenticateClient, StratcolInterface.StratcolInput.trans[] arrayOfTrans) {
            StratcolInterface.StratcolInput.addStandardTransRequest inValue = new StratcolInterface.StratcolInput.addStandardTransRequest();
            inValue.batchHeader = batchHeader;
            inValue.authenticateClient = authenticateClient;
            inValue.arrayOfTrans = arrayOfTrans;
            StratcolInterface.StratcolInput.addStandardTransResponse retVal = ((StratcolInterface.StratcolInput.addStandardTransPortType)(this)).addStandardTrans(inValue);
            return retVal.result;
        }
    }
}
