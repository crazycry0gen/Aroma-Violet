using System.Xml.Serialization;

namespace StratCorp.CorpSMS.Entities
{
    public class SendSettingsEntity
    {
        [XmlElement("live")]
        public bool IsLive { get; set; }

        [XmlElement("return_credits")]
        public bool ReturnCredits { get; set; }

        [XmlElement("return_msgs_success_count")]
        public bool ReturnMessageSuccessCount { get; set; }

        [XmlElement("return_msgs_failed_count")]
        public bool ReturnMessageFailedCount { get; set; }

        [XmlElement("return_entries_success_status")]
        public bool ReturnEntriesSuccessStatus { get; set; }

        [XmlElement("return_entries_failed_status")]
        public bool ReturnEntriesFailedStatus { get; set; }

        [XmlElement("default_senderid")]
        public string DefaultSender { get; set; }

        [XmlElement("default_data1")]
        public string DefaultText1 { get; set; }

        [XmlElement("default_data2")]
        public string DefaultText2 { get; set; }

        [XmlElement("default_date")]
        public string DefaultDate { get; set; }

        [XmlElement("default_time")]
        public string DefaultTime { get; set; }

        [XmlElement("default_flash")]
        public bool Flash { get; set; }

        [XmlElement("default_type")]
        public string Type { get; set; }

        [XmlElement("default_costcentre")]
        public string CostCentre { get; set; }
    }
}
