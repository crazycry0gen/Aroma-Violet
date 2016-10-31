using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.Xml;
using System.IO;

//Set namespace to the project name
namespace StratcolInterface // <-- EDIT
{
    // IMPORTANT //
    // Please do not change any of the following code as this might result in
    // unwanted results or failure of the function completely.
    //
    // The code below intercepts the XML (SOAP Request and Response Envelopes)
    // and returns them as string variables.

    class XMLInspector : IEndpointBehavior
    {
        public string lastRequestXML
        {
            get
            {
                return soapInspector.lastRequestXML;
            }
        }

        public string lastResponseXML
        {
            get
            {
                return soapInspector.lastResponseXML;
            }
        }

        public string lastResponseText
        {
            get
            {
                string strXMLdoc = soapInspector.lastResponseXML;
                XmlDocument xmlDoc = new XmlDocument();

                using (StringReader reader = new StringReader(strXMLdoc))
                {
                    xmlDoc.Load(reader);

                    XmlNode screenNode = xmlDoc.DocumentElement.FirstChild;
                    return screenNode.ParentNode.InnerText;
                }

            }
        }

        private MyMessageInspector soapInspector = new MyMessageInspector();

        public void AddBindingParameters(ServiceEndpoint endPoint, BindingParameterCollection bindingParameters)
        {

        }

        public void ApplyDispatchBehavior(ServiceEndpoint endPoint, EndpointDispatcher endPointDispatcher)
        {

        }

        public void Validate(ServiceEndpoint endPoint)
        {

        }

        public void ApplyClientBehavior(ServiceEndpoint endPoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(soapInspector);
        }

        public class MyMessageInspector : IClientMessageInspector
        {
            public string lastRequestXML { get; private set; }
            public string lastResponseXML { get; private set; }

            public void AfterReceiveReply(ref Message reply, object corActionState)
            {
                lastResponseXML = reply.ToString();
            }

            public object BeforeSendRequest(ref Message request, IClientChannel channel)
            {
                lastRequestXML = request.ToString();
                return request;
            }
        }
    }
}
