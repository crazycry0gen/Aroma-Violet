using System;

namespace StratCorp.CorpSMS.Entities
{
    public class MessageReplyEntity
    {
        public int MessageReplyId { get; set; }

        public int MessageId { get; set; }

        public int ReplyId { get; set; }

        public string Text { get; set; }

        public DateTime DateReceived { get; set; }
    }
}
