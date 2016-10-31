using System;
using System.Runtime.Serialization;

namespace StratCorp.CorpSMS.ServiceLibrary.DataContracts
{
    [DataContract]
    [KnownType(typeof(DeliveryReport))]
    [KnownType(typeof(SendMessage))]
    public class Message
    {
        [DataMember]
        public int MessageId { get; set; }

        [DataMember]
        public string Number { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public byte[] EncryptedText { get; set; }

        [DataMember]
        public int OriginID { get; set; }

        [DataMember]
        public int UniqueID { get; set; }

        [DataMember]
        public DateTime DateCreated { get; set; }
    }
}
