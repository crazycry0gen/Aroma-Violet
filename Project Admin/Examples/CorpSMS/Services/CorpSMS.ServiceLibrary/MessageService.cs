using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.Text;
using StratCorp.CorpSMS.Entities;
using StratCorp.CorpSMS.Logic;
using StratCorp.CorpSMS.ServiceLibrary.DataContracts;
using StratCorp.CorpSMS.ServiceLibrary.Translators;

namespace StratCorp.CorpSMS.ServiceLibrary
{
    public class MessageService : IMessageService
    {
        private static readonly string SOURCE = "CorpSMS Service Library";

        public ICollection<Message> SendSMS(Message smsEntity)
        {
            var smsList = new List<Message>() { smsEntity };

            return SendSMS(smsList);
        }

        public ICollection<Message> SendSMS(ICollection<Message> smsCollection)
        {
            return SafeExecutePublicMethod<ICollection<Message>>(() =>
                {
                    List<MessageEntity> list = new List<MessageEntity>();
                    smsCollection.ForAll(s => list.Add(MessageTranslator.MessageContractToMessageEntity(s)));

                    MessageFacade.SendSMS(list);
                    List<Message> returnList = new List<Message>();
                    list.ForAll(m =>
                        {
                            returnList.Add(MessageTranslator.MessageEntityToSendMessageContract(m));
                        });
                    return returnList;
                });
        }

        public void SendQueuedMessages(Guid guid)
        {
            SafeExecutePublicMethod(() =>
                {
                    MessageFacade.SendQueuedMessages(guid);
                });
        }

        public void SendAllQueuedMessages()
        {
            SafeExecutePublicMethod(() =>
            {
                MessageFacade.SendAllQueuedMessages();
            });
        }

        public void SendSMS(int batchId)
        {
        }

        public void RefreshDeliveryReport()
        {
            SafeExecutePublicMethod(() =>
                {
                    MessageFacade.RefreshDeliveryReport();
                });
        }

        public void RefreshCredits()
        {
            SafeExecutePublicMethod(() =>
                 {
                     MessageFacade.RefreshCredits();
                 });
        }

        public void RefreshReplies()
        {
            SafeExecutePublicMethod(() =>
                {
                    MessageFacade.RefreshReplies();
                });
        }

        public void RefreshShortCodes()
        {
            SafeExecutePublicMethod(() =>
                 {
                     MessageFacade.RefreshShortCodes();
                 });
        }

        public ICollection<DeliveryReport> GetDeliveryReports()
        {
            return SafeExecutePublicMethod<ICollection<DeliveryReport>>(() =>
                {
                    List<DeliveryReport> messages = new List<DeliveryReport>();
                    MessageFacade.GetDeliveryReports().ForAll(entity =>
                        {
                            messages.Add(MessageTranslator.MessageEntityToDeliveryReport(entity));
                        });

                    return messages;
                });
        }

        public ICollection<DeliveryReport> GetDeliveryReportByOriginId(int originId, int uniqueId)
        {
            return SafeExecutePublicMethod<ICollection<DeliveryReport>>(() =>
                {
                    List<DeliveryReport> messages = new List<DeliveryReport>();
                    MessageFacade.GetDeliveryReportByOriginId(originId, uniqueId).ForAll(entity =>
                    {
                        messages.Add(MessageTranslator.MessageEntityToDeliveryReport(entity));
                    });

                    return messages;
                });
        }

        public ICollection<Message> GetShortCodeMessages(DateTime startTime, DateTime endTime)
        {
            return SafeExecutePublicMethod<ICollection<Message>>(() =>
                {
                    List<Message> messages = new List<Message>();
                    MessageFacade.GetShortCodeMessages(startTime, endTime).ForAll(entity =>
                        {
                            messages.Add(MessageTranslator.MessageEntityToSendMessageContract(entity));
                        });

                    return messages;
                });
        }

        private void SafeExecutePublicMethod(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (TimeoutException timeoutException)
            {
                HandleException(ExceptionType.TimeoutException, timeoutException);
            }
            catch (FaultException faultException)
            {
                HandleException(ExceptionType.UnkownException, faultException);
            }
            catch (CommunicationException communicationException)
            {
                HandleException(ExceptionType.CommunicationException, communicationException);
            }
            catch (Exception exception)
            {
                HandleException(ExceptionType.GeneralException, exception);
            }
        }

        private T SafeExecutePublicMethod<T>(Func<T> function)
        {
            try
            {
                return function.Invoke();
            }
            catch (TimeoutException timeoutException)
            {
                HandleException(ExceptionType.TimeoutException, timeoutException);
            }
            catch (FaultException faultException)
            {
                HandleException(ExceptionType.UnkownException, faultException);
            }
            catch (CommunicationException communicationException)
            {
                HandleException(ExceptionType.CommunicationException, communicationException);
            }
            catch (Exception exception)
            {
                HandleException(ExceptionType.GeneralException, exception);
            }

            return default(T);
        }

        private static void HandleException(ExceptionType exceptionType, Exception exception)
        {
            using (var eventLog = new EventLog())
            {
                if (!System.Diagnostics.EventLog.SourceExists(SOURCE))
                {
                    System.Diagnostics.EventLog.CreateEventSource(SOURCE, "Application");
                }
                eventLog.Source = SOURCE;
                eventLog.Log = "Application";

                StringBuilder message = new StringBuilder();
                message.AppendLine("A " + SOURCE + " " + exceptionType.ToString() + " occurred with the following details:");
                message.AppendLine();
                message.AppendLine(exception.Message);
                if (exception.InnerException != null)
                {
                    message.AppendLine();
                    message.AppendLine("InnerException:");
                    message.AppendLine(exception.InnerException.ToString());
                }
                message.AppendLine();
                message.AppendLine("StackTrace:");
                message.AppendLine(exception.StackTrace);

                eventLog.WriteEntry(message.ToString(), System.Diagnostics.EventLogEntryType.Error);

                StringBuilder returnMessage = new StringBuilder();
                returnMessage.AppendLine("Exception:");
                returnMessage.AppendLine(exception.Message);
                if (exception.InnerException != null)
                {
                    returnMessage.AppendLine();
                    returnMessage.AppendLine("Inner Exception:");
                    returnMessage.AppendLine(exception.InnerException.Message);
                }

                throw new FaultException<ServiceCallFault>(new ServiceCallFault(returnMessage.ToString(), exception));
            }
        }
    }
}
