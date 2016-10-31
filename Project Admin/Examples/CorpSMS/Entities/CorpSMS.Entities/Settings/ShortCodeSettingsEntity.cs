using System.Xml.Serialization;

namespace StratCorp.CorpSMS.Entities
{
    public class ShortCodeSettingsEntity
    {
        [XmlElement("id")]
        public long LatestId { get; set; }

        [XmlElement("date_format")]
        public string DateFormat { get; set; }
    }
}