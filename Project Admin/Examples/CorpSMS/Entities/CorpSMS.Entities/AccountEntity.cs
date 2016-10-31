
namespace StratCorp.CorpSMS.Entities
{
    public class AccountEntity
    {
        public int AccountId { get; set; }

        public string Description { get; set; }

        public string CountryCodePrefix { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
