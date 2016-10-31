using System;
using System.ServiceModel;
using CorpSMSServiceAssembly.MessageService;

namespace CorpSMSServiceAssembly
{
    public class SMSService
    {
        public static string SendSMS(string number, string text, int originId, int uniqueId)
        {
            var proxy = GetProxy();            
            

            proxy.SendSingleSMS(new Message()
                {
                    Number = number,
                    Text = text,
                    OriginID = originId,
                    UniqueID = uniqueId
                });

            return string.Empty;
        }

        public static string SendQueuedMessages(string batchId)
        {            
            var proxy = GetProxy();

            Guid guid = new Guid(batchId);
            proxy.SendQueuedMessages(guid);

            return string.Empty;
        }

        public static string SendAllQueuedMessages()
        {
            var proxy = GetProxy();

            proxy.SendAllQueuedMessages();

            return string.Empty;
        }

        public static string RefreshDeliveryReport()
        {
            var proxy = GetProxy();
            
            proxy.RefreshDeliveryReport();

            return string.Empty;
        }

        private static MessageServiceClient GetProxy()
        {                       
            EndpointAddress endPoint = new EndpointAddress("http://172.1.0.205:81/CorpSMS.ServiceHost/MessageService.svc");
            //EndpointAddress endPoint = new EndpointAddress("http://localhost:8732/StratCorp.CorpSMS.ServiceLibrary/SMSService/");            
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            binding.ReceiveTimeout = new TimeSpan(0, 30, 0);
            binding.SendTimeout = new TimeSpan(0, 30, 0);

            MessageServiceClient proxy = new MessageServiceClient(binding, endPoint);            

            return proxy;
        }
    }
}
