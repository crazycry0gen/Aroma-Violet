using System;
using System.Runtime.Serialization;

namespace StratCorp.CorpSMS.ServiceLibrary
{
    [DataContract]
    public class ServiceCallFault
    {
        public ServiceCallFault(string message, Exception exception)
        {
            this.FaultMessage = message;
            this.Exception = exception;
        }

        [DataMember]
        public string FaultMessage { get; set; }

        [DataMember]
        public Exception Exception { get; set; }
    }
}
