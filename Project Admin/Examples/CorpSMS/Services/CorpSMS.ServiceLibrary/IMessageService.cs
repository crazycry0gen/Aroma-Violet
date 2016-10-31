using System;
using System.Collections.Generic;
using System.ServiceModel;
using StratCorp.CorpSMS.ServiceLibrary.DataContracts;

namespace StratCorp.CorpSMS.ServiceLibrary
{
    [ServiceContract]
    public interface IMessageService
    {
        [OperationContract(Name = "SendSingleSMS")]
        [FaultContract(typeof(ServiceCallFault))]
        ICollection<Message> SendSMS(Message smsEntity);

        [OperationContract]
        [FaultContract(typeof(ServiceCallFault))]
        ICollection<Message> SendSMS(ICollection<Message> smsCollection);

        [OperationContract]
        [FaultContract(typeof(ServiceCallFault))]
        void SendQueuedMessages(Guid guid);

        [OperationContract]
        [FaultContract(typeof(ServiceCallFault))]
        void SendAllQueuedMessages();

        [OperationContract]
        [FaultContract(typeof(ServiceCallFault))]
        void RefreshDeliveryReport();

        [OperationContract]
        [FaultContract(typeof(ServiceCallFault))]
        void RefreshCredits();

        [OperationContract]
        [FaultContract(typeof(ServiceCallFault))]
        void RefreshReplies();

        [OperationContract]
        [FaultContract(typeof(ServiceCallFault))]
        void RefreshShortCodes();

        [OperationContract]
        [FaultContract(typeof(ServiceCallFault))]
        ICollection<DeliveryReport> GetDeliveryReports();

        [OperationContract]
        [FaultContract(typeof(ServiceCallFault))]
        ICollection<DeliveryReport> GetDeliveryReportByOriginId(int originId, int uniqueId);

        [OperationContract]
        [FaultContract(typeof(ServiceCallFault))]
        ICollection<Message> GetShortCodeMessages(DateTime startTime, DateTime endTime);
    }
}
