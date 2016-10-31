using System;
using System.Runtime.Serialization;

namespace StratCorp.CorpSMS
{
    [Serializable]
    public class SMSServiceException : Exception
    {
        public SMSServiceException()
            : base()
        {
        }

        public SMSServiceException(string message)
            : base(message)
        {
        }

        public SMSServiceException(string message, Exception exception)
            : base(message, exception)
        {
        }

        protected SMSServiceException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }
    }
}
