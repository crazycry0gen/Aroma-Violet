using System.Collections.Generic;

namespace StratCorp.CorpSMS.Entities
{
    public class SendServiceCallEntity
    {
        private ICollection<MessageEntity> _entries = new List<MessageEntity>();

        public AccountEntity Account { get; set; }

        public SendSettingsEntity Settings { get; set; }

        public ICollection<MessageEntity> Entries
        {
            get
            {
                return _entries;
            }
        }
    }
}
