using System;
using System.Collections.Generic;

namespace StratCorp.CorpSMS.Entities
{
    /// <summary>
    /// This class contains the outbound SMS Entity.
    /// </summary>
    public class MessageEntity : EntityBase
    {
        private ICollection<MessageDetailEntity> _messageDetail = new List<MessageDetailEntity>();

        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public int MessageId { get; set; }

        /// <summary>
        /// Gets or sets the number the SMS is sent to.
        /// </summary>
        public string NumberTo { get; set; }

        public string Sender { get; set; }

        /// <summary>
        /// Gets or sets the text of the SMS.
        /// </summary>        
        public string Text { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public byte[] EncryptedText { get; set; }

        public DateTime DateCreated { get; set; }

        public bool HasPrefix { get; set; }

        public int OriginId { get; set; }

        public int UniqueId { get; set; }

        public int EventId { get; set; }

        public Guid RowGuid { get; set; }

        public string Status { get; set; }
    }
}
