using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Text;
using System.Timers;
using StratCorp.CorpSMS.Service.MessageService;

namespace StratCorp.CorpSMS.Service
{
    public static class ServiceHelpers
    {
        internal static MessageServiceClient ProxyService()
        {
            return new StratCorp.CorpSMS.Service.MessageService.MessageServiceClient("BasicHttpBinding_IMessageService");
        }

        internal static void SafeExecutePublicMethod(Action action, string source, Timer timer)
        {
            try
            {
                timer.Enabled = false;
                action.Invoke();
            }
            catch (TimeoutException timeoutException)
            {
                HandleException(ExceptionType.TimeoutException, timeoutException, source);
            }
            catch (FaultException<MessageService.ServiceCallFault> messageServiceException)
            {
                HandleException(ExceptionType.ServiceException, messageServiceException.Detail.Exception, source);
            }
            catch (FaultException faultException)
            {
                HandleException(ExceptionType.UnkownException, faultException, source);
            }
            catch (CommunicationException communicationException)
            {
                HandleException(ExceptionType.CommunicationException, communicationException, source);
            }
            catch (Exception exception)
            {
                HandleException(ExceptionType.GeneralException, exception, source);
            }
            finally
            {
                timer.Enabled = true;
            }
        }

        private static void HandleException(ExceptionType exceptionType, Exception exception, string source)
        {
            using (var eventLog = new EventLog())
            {
                if (!System.Diagnostics.EventLog.SourceExists(source))
                {
                    System.Diagnostics.EventLog.CreateEventSource(source, "Application");
                }
                eventLog.Source = source;
                eventLog.Log = "Application";

                StringBuilder message = new StringBuilder();
                message.AppendLine("A " + source + " " + exceptionType.ToString() + " occurred with the following details:");
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
            }
        }
    }
}
