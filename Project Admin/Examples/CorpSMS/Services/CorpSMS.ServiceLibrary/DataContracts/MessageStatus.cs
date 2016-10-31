using System;
using System.Runtime.Serialization;

namespace StratCorp.CorpSMS.ServiceLibrary.DataContracts
{
    [DataContract]
    public class MessageStatus
    {
        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public DateTime StatusDate { get; set; }

        [DataMember]
        public string Reason { get; set; }
    }
}
