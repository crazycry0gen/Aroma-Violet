using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StratCorp.CorpSMS.ServiceLibrary.DataContracts
{
    [DataContract]
    public class DeliveryReport : Message
    {
        private ICollection<MessageStatus> messageStatus = new List<MessageStatus>();

        [DataMember]
        public ICollection<MessageStatus> MessageStatus
        {
            get
            {
                return messageStatus;
            }
            set
            {
                messageStatus = value;
            }
        }
    }
}
