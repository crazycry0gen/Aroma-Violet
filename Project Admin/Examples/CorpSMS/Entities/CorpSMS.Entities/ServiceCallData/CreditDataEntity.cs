using System.Xml.Serialization;

namespace StratCorp.CorpSMS.Entities
{
    public class CreditDataEntity
    {
        [XmlElement("credits")]
        public int CreditAmount { get; set; }        
    }
}
