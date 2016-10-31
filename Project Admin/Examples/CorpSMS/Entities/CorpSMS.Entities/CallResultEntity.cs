using System.Xml.Serialization;

namespace StratCorp.CorpSMS.Entities
{
    public class CallResultEntity
    {
        [XmlElement("result")]
        public bool Result { get; set; }

        [XmlElement("error")]
        public string Error { get; set; }
    }
}
