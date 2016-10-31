using System.Runtime.Serialization;

namespace StratCorp.CorpSMS.ServiceLibrary.DataContracts
{
    [DataContract]
    public class SendMessage : Message
    {
        [DataMember]
        public bool Failed { get; set; }

        [DataMember]
        public string FailedReason { get; set; }
    }
}
