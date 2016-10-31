using System.Xml.Serialization;

namespace StratCorp.CorpSMS.Entities
{
    [XmlRoot("api_result", Namespace = "")]
    public class CreditResultEntity
    {
        [XmlElement("call_result")]
        public CallResultEntity CallResult { get; set; }

        [XmlElement("data")]
        public CreditDataEntity Credits { get; set; }
    }
}
