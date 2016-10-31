using System.Xml.Serialization;

namespace StratCorp.CorpSMS.Entities
{
    public class GeneralSettingsEntity
    {
        [XmlElement("id")]
        public long LatestId { get; set; }

        [XmlElement("max_recs")]
        public int RecordCount { get; set; }

        [XmlElement("cols_returned")]
        public string ReturnColumns { get; set; }

        [XmlElement("date_format")]
        public string DateFormat { get; set; }
    }
}
