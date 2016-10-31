using System.Xml.Serialization;

namespace StratCorp.CorpSMS.Entities
{
    [XmlRoot("api_result")]
    public class ShortCodeResultEntity
    {
        [XmlElement("data")]
        public ShortCodeDataEntity[] Results { get; set; }
    }
}
