using System;

namespace StratCorp.CorpSMS.Entities
{
    public class MessageQueueEntity
    {
        public int MessageQueueId { get; set; }

        public Guid BatchId { get; set; }

        public string Number { get; set; }

        public string Text { get; set; }

        public int MessageId { get; set; }

        public int OriginId { get; set; }

        public int UniqueId { get; set; }
    }
}
