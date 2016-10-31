using System.Xml.Serialization;

namespace StratCorp.CorpSMS.Entities
{
    [XmlRoot("reply", Namespace = "", IsNullable = false)]
    public class ReplyServiceCallEntity
    {
        [XmlElement("settings")]
        public GeneralSettingsEntity Settings { get; set; }
    }
}
