using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace StratCorp.CorpSMS.Infrastructure.Helpers
{
    public static class Serializer
    {
        public static string SerializeXml<T>(T obj)
        {
            StringBuilder xmlString = new StringBuilder();

            var settings = GetXMLWriterSettings();

            var namespaces = GetNamespaces();

            using (XmlWriter xmlWriter = XmlWriter.Create(xmlString, settings))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

                xmlSerializer.Serialize(xmlWriter, obj, namespaces);

                return xmlString.ToString();
            }
        }

        /// <summary>
        /// Strips non-printable ascii characters 
        /// Refer to http://www.w3.org/TR/xml11/#charsets for XML 1.1
        /// Refer to http://www.w3.org/TR/2006/REC-xml-20060816/#charsets for XML 1.0
        /// </summary>
        private static string StripIllegalXMLChars(string xmlString)
        {
            string pattern = @"#x((10?|[0-F])FFF[EF]|FDD[0-9A-F]|[19][0-9A-F]|7F|8[0-46-9A-F]|0?[1-8BCEF]|0)";
            
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            if (regex.IsMatch(xmlString))
            {
                xmlString = regex.Replace(xmlString, "lt"); 
            }
            return xmlString;
        }
  
        public static T DeserializeXml<T>(string xmlString)
        {
            T result;

            xmlString = UpdateXmlString(xmlString);

            xmlString = StripIllegalXMLChars(xmlString);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (TextReader textReader = new StringReader(xmlString))
            {
                result = (T)xmlSerializer.Deserialize(textReader);
            }

            return result;
        }

        /// <summary>
        /// Updates the xml string so that it can be deserialized.
        /// </summary>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        private static string UpdateXmlString(string xmlString)
        {
            string returnString = xmlString;

            // Convert 'True' and 'False' to lower case so that they can be serialized into boolean values.
            returnString = returnString.Replace("False", "false");
            returnString = returnString.Replace("True", "true");

            return returnString;
        }

        private static XmlSerializerNamespaces GetNamespaces()
        {
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            return namespaces;
        }

        private static XmlWriterSettings GetXMLWriterSettings()
        {
            return new XmlWriterSettings()
            {
                OmitXmlDeclaration = true,
                Indent = true
            };
        }
    }
}
