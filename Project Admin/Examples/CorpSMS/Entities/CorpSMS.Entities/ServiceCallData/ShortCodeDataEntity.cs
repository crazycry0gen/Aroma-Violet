using System.Xml.Serialization;

namespace StratCorp.CorpSMS.Entities
{
    public class ShortCodeDataEntity
    {
        [XmlElement("changeid")]
        public int ChangeId { get; set; }

        [XmlElement("shortcode")]
        public string ShortCode { get; set; }

        [XmlElement("keyword")]
        public string Keyword { get; set; }

        [XmlElement("phonenumber")]
        public string Number { get; set; }

        [XmlElement("message")]
        public string Text { get; set; }

        [XmlElement("received")]
        public string DateReceived { get; set; }

        [XmlIgnore]
        public int BatchId { get; set; }
    }
}
