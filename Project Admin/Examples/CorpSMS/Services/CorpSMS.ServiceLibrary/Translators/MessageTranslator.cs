using System;
using StratCorp.CorpSMS.Entities;
using StratCorp.CorpSMS.ServiceLibrary.DataContracts;

namespace StratCorp.CorpSMS.ServiceLibrary.Translators
{
    public static class MessageTranslator
    {
        public static MessageEntity MessageContractToMessageEntity(Message contract)
        {
            MessageEntity entity = new MessageEntity()
            {
                MessageId = contract.MessageId,
                NumberTo = contract.Number,
                Text = contract.Text,
                OriginId = contract.OriginID,
                UniqueId = contract.UniqueID,
                DateCreated = contract.DateCreated
            };

            return entity;
        }

        public static Message MessageEntityToSendMessageContract(MessageEntity entity)
        {
            Message contract = new Message()
            {
                MessageId = entity.MessageId,
                Number = entity.NumberTo,
                Text = entity.Text,
                EncryptedText = entity.EncryptedText,
                OriginID = entity.OriginId,
                UniqueID = entity.UniqueId,
                DateCreated = entity.DateCreated                
            };

            return contract;
        }

        public static DeliveryReport MessageEntityToDeliveryReport(MessageEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            DeliveryReport report = new DeliveryReport()
            {
                //Number = entity.Number,
                //Text = entity.Text,
                //OriginID = entity.OriginId,
                //UniqueID = entity.UniqueId
            };
            //if (entity.MessageDetail != null)
            //{
            //    entity.MessageDetail.ForAll(detail =>
            //    {
            //        report.MessageStatus.Add(new MessageStatus()
            //        {
            //            Status = detail.StatusDescription,
            //            StatusDate = detail.StatusDate,
            //            Reason = detail.Reason
            //        });
            //    });
            //}

            return report;
        }
    }
}
