using System.Xml.Serialization;

namespace StratCorp.CorpSMS.Entities
{
    [XmlRoot("options", Namespace = "", IsNullable = false)]
    public class ShortCodeServiceCallEntity
    {
        [XmlElement("settings")]
        public ShortCodeSettingsEntity Settings { get; set; }
    }
}
