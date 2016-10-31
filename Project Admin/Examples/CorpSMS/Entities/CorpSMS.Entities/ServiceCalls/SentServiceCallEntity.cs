using System.Xml.Serialization;

namespace StratCorp.CorpSMS.Entities
{
    [XmlRoot("sent", Namespace = "", IsNullable = false)]
    public class SentServiceCallEntity
    {
        [XmlElement("settings")]
        public GeneralSettingsEntity Settings { get; set; }
    }
}
