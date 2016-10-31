using System;
using System.Collections.Generic;
using System.Linq;
using StratCorp.CorpSMS.Entities;

namespace StratCorp.CorpSMS.DataAccess
{
    public class MessageDataAccessor : IDisposable
    {
        public SendSettingsEntity GetSendSettings()
        {
            using (var context = new CorpSMSEntities())
            {
                context.CommandTimeout = 0;
                var setting = context.SendSettings.Select(p => p).FirstOrDefault();
                var entity = new SendSettingsEntity()
                {
                    IsLive = setting.IsLive,
                    ReturnMessageSuccessCount = setting.ReturnMessageSuccessCount,
                    ReturnMessageFailedCount = setting.ReturnMessageFailedCount,
                    ReturnCredits = setting.ReturnCredits,
                    ReturnEntriesSuccessStatus = setting.ReturnEntriesSuccessStatus,
                    ReturnEntriesFailedStatus = setting.ReturnEntriesFailedStatus,
                    DefaultText1 = setting.DefaultText1,
                    DefaultText2 = setting.DefaultText2,
                    DefaultDate = DateTime.Now.ToString(setting.DefaultDateFormat),
                    DefaultTime = DateTime.Now.ToString(setting.DefaultTimeFormat),
                    DefaultSender = setting.DefaultSender,
                    Flash = setting.Flash,
                    Type = setting.Type,
                    CostCentre = setting.CostCentre      
                };

                return entity;
            }
        }

        public GeneralSettingsEntity GetSentSettings(int accountID)
        {
            using (var context = new CorpSMSEntities())
            {
                var setting = context.SentSettings.Where(p => p.AccountID == accountID && p.IsActive == true).FirstOrDefault();
                var entity = new GeneralSettingsEntity()
                {
                    LatestId = setting.LatestID,
                    RecordCount = setting.RecordCount,
                    ReturnColumns = setting.ReturnColumns,
                    DateFormat = setting.DateFormat
                };

                return entity;
            }
        }

        public GeneralSettingsEntity GetReplySettings(int accountID)
        {
            using (var context = new CorpSMSEntities())
            {
                //TODO: Make sure settings exist.
                var setting = context.ReplySettings.Where(p => p.AccountID == accountID && p.IsActive == true).FirstOrDefault();
                var entity = new GeneralSettingsEntity()
                {
                    LatestId = setting.LatestID,
                    RecordCount = setting.RecordCount,
                    ReturnColumns = setting.ReturnColumns,
                    DateFormat = setting.DateFormat
                };

                return entity;
            }
        }

        public ShortCodeSettingsEntity GetShortCodeSettings(int accountID)
        {
            using (var context = new CorpSMSEntities())
            {
                ShortCodeSettingsEntity entity = null;
                var setting = context.ShortCodeSettings.Where(p => p.AccountID == accountID && p.IsActive == true).FirstOrDefault();
                if (setting != null)
                {
                    entity = new ShortCodeSettingsEntity()
                    {
                        LatestId = setting.LatestID,
                        DateFormat = setting.DateFormat
                    };
                }

                return entity;
            }
        }

        /// <summary>
        /// Remove the Message from the Queue in the Barred List
        /// </summary>
        public void RemoveBarredMessages()
        {
            using (var context = new CorpSMSEntities())
            {
                context.spRemoveBarredMessages();
            }
        }

        /// <summary>
        /// RAdd numbers  in the Barred List
        /// </summary>
        public void AddStopReplies()
        {
            using (var context = new CorpSMSEntities())
            {
                context.spAddStopReplies();
            }
        }        

        /// <summary>
        /// Remove the Message from the Queue if it has been send successfully or failed more than 5 times
        /// </summary>
        /// <param name="uniqueID"></param>
        public void RemoveSentMessages(int uniqueID)
        {
            using (var context = new CorpSMSEntities())
            {

                context.spRemoveSentMessages(uniqueID);
            }
        }

        public void UpdateSentSettingsLatestID(long latestID, int accountID)
        {
            using (var context = new CorpSMSEntities())
            {
                var setting = context.SentSettings.Where(p => p.AccountID == accountID).FirstOrDefault();

                if (setting != null)
                {
                    // Detach the entity first
                    context.Detach(setting);

                    setting.LatestID = latestID;

                    // Attach the entity again.
                    context.Attach(setting);
                    context.ObjectStateManager.ChangeObjectState(setting, System.Data.EntityState.Modified);

                    context.SaveChanges();
                }
            }
        }

        public void UpdateReplySettingsLatestID(long latestID, int accountID)
        {
            using (var context = new CorpSMSEntities())
            {
                var setting = context.ReplySettings.Where(p => p.AccountID == accountID).FirstOrDefault();

                if (setting != null)
                {
                    // Detach the entity first
                    context.Detach(setting);

                    setting.LatestID = latestID;

                    // Attach the entity again.
                    context.Attach(setting);
                    context.ObjectStateManager.ChangeObjectState(setting, System.Data.EntityState.Modified);

                    context.SaveChanges();
                }
            }
        }

        public void UpdateShortCodeSettingsLatestID(long latestID, int accountID)
        {
            using (var context = new CorpSMSEntities())
            {
                var setting = context.ShortCodeSettings.Where(p => p.AccountID == accountID).FirstOrDefault();

                if (setting != null)
                {
                    // Detach the entity first
                    context.Detach(setting);

                    setting.LatestID = latestID;

                    // Attach the entity again.
                    context.Attach(setting);
                    context.ObjectStateManager.ChangeObjectState(setting, System.Data.EntityState.Modified);

                    context.SaveChanges();
                }
            }
        }

        public MessageEntity SetMessage(MessageEntity entity)
        {
            using (var context = new CorpSMSEntities())
            {
                var message = context.Messages.Where(p => p.RowGuid == entity.RowGuid).FirstOrDefault();

                if (message != null)
                {
                    // Detach the entity first
                    context.Detach(message);

                    message = EntityToMessage(entity, message);

                    // Attach the entity again.
                    context.Attach(message);
                    context.ObjectStateManager.ChangeObjectState(message, System.Data.EntityState.Modified);
                }
                else
                {
                    message = EntityToMessage(entity, new Message());
                    context.AddToMessages(message);
                }

                context.SaveChanges();

                return MessageToEntity(message);
            }
        }

        public MessageEntity SetMessageForced(MessageEntity entity)
        {
            using (var context = new CorpSMSEntities())
            {
                var message = context.Messages.Where(p => p.RowGuid == entity.RowGuid).FirstOrDefault();

                if (message != null)
                {
                    // Detach the entity first
                    context.Detach(message);

                    message = EntityToMessage(entity, message);

                    // Attach the entity again.
                    context.Attach(message);
                    context.ObjectStateManager.ChangeObjectState(message, System.Data.EntityState.Modified);
                }
                else
                {
                    message = EntityToMessage(entity, new Message());
                    context.AddToMessages(message);
                }

                context.SaveChanges();

                return MessageToEntity(message);
            }
        }

        public MessageEntity GetMessageByRowGuid(Guid guid)
        {
            using (var context = new CorpSMSEntities())
            {
                var message = context.Messages.Where(p => p.RowGuid == guid).FirstOrDefault();
                MessageEntity entity = null;

                if (message != null)
                {
                    entity = MessageToEntity(message);
                }

                return entity;
            }
        }

        public DeliveryReportEntity InsertDeliveryReport(DeliveryReportEntity entity)
        {
            using (var context = new CorpSMSEntities())
            {
                var report = context.DeliveryReports.Where(p => p.DeliveryReportId == entity.DeliveryReportId).FirstOrDefault();

                if (report != null)
                {
                    // Detach the entity first
                    context.Detach(report);

                    report = EntityToDeliveryReport(entity, report);

                    // Attach the entity again.
                    context.Attach(report);
                    context.ObjectStateManager.ChangeObjectState(report, System.Data.EntityState.Modified);
                }
                else
                {
                    report = EntityToDeliveryReport(entity, new DeliveryReport());
                    context.AddToDeliveryReports(report);
                }

                context.SaveChanges();

                return DeliveryReportToEntity(report);
            }
        }

        public MessageReplyEntity InsertMessageReply(MessageReplyEntity entity)
        {
            using (var context = new CorpSMSEntities())
            {
                var reply = context.MessageReplies.Where(p => p.MessageReplyId == entity.MessageReplyId).FirstOrDefault();

                if (reply != null)
                {
                    // Detach the entity first
                    context.Detach(reply);

                    reply = EntityToMessageReply(entity, reply);

                    // Attach the entity again.
                    context.Attach(reply);
                    context.ObjectStateManager.ChangeObjectState(reply, System.Data.EntityState.Modified);
                }
                else
                {
                    reply = EntityToMessageReply(entity, new MessageReply());
                    context.AddToMessageReplies(reply);
                }

                context.SaveChanges();

                return MessageReplyToEntity(reply);
            }
        }

        private static Message EntityToMessage(MessageEntity entity, Message message)
        {
            message.Number = entity.NumberTo;
            message.Sender = entity.Sender;
            message.Text = entity.Text;
            message.Created = entity.DateCreated;
            message.OriginId = entity.OriginId;
            message.UniqueId = entity.UniqueId;
            message.EventId = entity.EventId;
            message.RowGuid = entity.RowGuid;
            message.Status = entity.Status;

            return message;
        }

        private static MessageEntity MessageToEntity(Message message)
        {
            return new MessageEntity()
            {
                MessageId = message.MessageId,
                NumberTo = message.Number,
                Sender = message.Sender,
                Text = message.Text,
                DateCreated = message.Created,
                OriginId = message.OriginId,
                UniqueId = message.UniqueId,
                EventId = message.EventId,
                RowGuid = message.RowGuid
            };
        }

        private static DeliveryReport EntityToDeliveryReport(DeliveryReportEntity entity, DeliveryReport deliveryReport)
        {
            deliveryReport.MessageId = entity.MessageId;
            deliveryReport.ChangeId = entity.ChangeId;
            deliveryReport.SentId = entity.SentId;
            deliveryReport.Status = entity.Status;
            deliveryReport.StatusDate = entity.StatusDate;

            return deliveryReport;
        }

        private static DeliveryReportEntity DeliveryReportToEntity(DeliveryReport deliveryReport)
        {
            return new DeliveryReportEntity()
            {
                DeliveryReportId = deliveryReport.DeliveryReportId,
                MessageId = deliveryReport.MessageId,
                ChangeId = deliveryReport.ChangeId,
                SentId = deliveryReport.SentId,
                Status = deliveryReport.Status,
                StatusDate = deliveryReport.StatusDate
            };
        }

        private static MessageReply EntityToMessageReply(MessageReplyEntity entity, MessageReply messageReply)
        {
            messageReply.MessageId = entity.MessageId;
            messageReply.ReplyId = entity.ReplyId;
            messageReply.Text = entity.Text;
            messageReply.DateReceived = entity.DateReceived;

            return messageReply;
        }

        private static MessageReplyEntity MessageReplyToEntity(MessageReply reply)
        {
            return new MessageReplyEntity()
            {
                MessageReplyId = reply.MessageReplyId,
                MessageId = reply.MessageId,
                ReplyId = reply.ReplyId,
                Text = reply.Text,
                DateReceived = reply.DateReceived
            };
        }

        private static MessageQueue EntityToMessageQueue(MessageQueueEntity entity, MessageQueue message)
        {
            message.MessageQueueId = entity.MessageQueueId;
            message.Number = entity.Number;
            message.Text = entity.Text;
            message.BatchId = entity.BatchId;
            message.MessageId = entity.MessageId;
            message.OriginId = entity.OriginId;
            message.UniqueId = entity.UniqueId;

            return message;
        }

        private static MessageQueueEntity MessageQueueToEntity(MessageQueue message)
        {
            return new MessageQueueEntity()
            {
                MessageQueueId = message.MessageQueueId,
                Number = message.Number,
                Text = message.Text,
                BatchId = message.BatchId,
                MessageId = TypeSafety.GetValue<int>(message.MessageId),
                OriginId = message.OriginId,
                UniqueId = message.UniqueId
            };
        }

        public void InsertMessageDetail(ShortCodeDataEntity entity)
        {
            try
            {
                using (var context = new CorpSMSEntities())
                {
                    context.SetShortCodeMessage(
                        entity.ShortCode,
                        TypeSafety.GetValue<string>(entity.Number),
                        TypeSafety.GetValue<string>(entity.Text),
                        entity.ChangeId,
                        TypeSafety.GetValue<DateTime>(entity.DateReceived),
                        entity.BatchId,
                        "AsymmetricKey1");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ICollection<AccountEntity> GetAccounts()
        {
            using (var context = new CorpSMSEntities())
            {
                var query = context.Accounts;

                List<AccountEntity> accountList = new List<AccountEntity>();
                foreach (var account in query)
                {
                    accountList.Add(new AccountEntity()
                        {
                            AccountId = account.AccountID,
                            Description = account.Description,
                            CountryCodePrefix = account.CountryCodePrefix,
                            Username = account.Username,
                            Password = account.Password
                        });
                }

                return accountList;
            }
        }

        public AccountEntity GetDefaultAccount()
        {
            using (var context = new CorpSMSEntities())
            {
                var query = context.Accounts.Where(p => p.IsDefault).FirstOrDefault();
                AccountEntity entity = null;

                if (query != null)
                {
                    entity = new AccountEntity()
                   {
                       AccountId = query.AccountID,
                       CountryCodePrefix = query.CountryCodePrefix,
                       Description = query.Description,
                       Username = query.Username,
                       Password = query.Password
                   };
                }

                return entity;
            }
        }

        public ICollection<MessageEntity> GetDeliveryReports()
        {
            using (var context = new CorpSMSEntities())
            {
                List<MessageEntity> messages = new List<MessageEntity>();
                var query = context.Messages.Include("MessageDetails");

                query.ForAll(message =>
                    {
                        messages.Add(ConvertToMessageEntity(message));
                    });

                return messages;
            }
        }

        public ICollection<MessageEntity> GetDeliveryReportByOriginId(int originId, int uniqueId)
        {
            using (var context = new CorpSMSEntities())
            {
                List<MessageEntity> messages = new List<MessageEntity>();
                var query = context.Messages.Include("MessageDetails")
                    .Where(p => p.OriginId == originId && p.UniqueId == uniqueId);

                query.ForAll(message =>
                {
                    messages.Add(ConvertToMessageEntity(message));
                });

                return messages;
            }
        }

        public ICollection<MessageEntity> GetShortCodeMessages(DateTime startTime, DateTime endTime)
        {
            using (var context = new CorpSMSEntities())
            {
                List<MessageEntity> messages = new List<MessageEntity>();
                context.GetShortCodeMessage("AsymmetricKey1", "GWEST2003%", startTime, endTime).ForAll(message =>
                    {
                        messages.Add(ConvertToMessageEntity(message));
                    });

                return messages;
            }
        }

        private static MessageEntity ConvertToMessageEntity(Message message)
        {
            MessageEntity entity = new MessageEntity()
            {
                MessageId = message.MessageId,
                NumberTo = message.Number,
                Sender = message.Sender,
                Text = message.Text,
                EncryptedText = message.EncryptedText,
                OriginId = message.OriginId,
                UniqueId = message.UniqueId,
                DateCreated = message.Created,
                Status = message.Status
            };

            return entity;
        }

        public void SetCredit(CreditEntity entity)
        {
            try
            {
                using (var context = new CorpSMSEntities())
                {
                    var credit = Credit.CreateCredit(
                        0,
                        entity.AccountId,
                        entity.Amount,
                        DateTime.Now);

                    context.AddToCredits(credit);
                    context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ICollection<MessageQueueEntity> GetQueuedMessages(Guid batchId)
        {
            using (var context = new CorpSMSEntities())
            {
                var query = context.MessageQueues.Where(p => p.BatchId == batchId);

                List<MessageQueueEntity> messageList = new List<MessageQueueEntity>();
                foreach (var message in query)
                {
                    messageList.Add(MessageQueueToEntity(message));
                }

                return messageList;
            }
        }

        public ICollection<MessageQueueEntity> GetTopQueuedMessages()
        {
            using (var context = new CorpSMSEntities())
            {
                var query = ( from item in context.MessageQueues orderby item.Accessed ascending select item).Take(1000);         

                List<MessageQueueEntity> messageList = new List<MessageQueueEntity>();
                foreach (var message in query)
                {
                    messageList.Add(MessageQueueToEntity(message));
                    message.Accessed = DateTime.Now;                    
                }

                context.SaveChanges();

                return messageList;
            }
        }

        // Dispose() calls Dispose(true)
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
            }

            // free native resources if there are any.            
        }
    }
}
