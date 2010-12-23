using System;
using com.esendex.sdk.csharp;

namespace simplereminders.web.models.facades
{
    public class EsendexSmsFacade : IEsendexSmsFacade
    {
        private readonly MessageDispatcherService _messageDispatcherService;

        public EsendexSmsFacade(string account, string username, string password)
        {
            var sessionService = new SessionService();
            Guid sessionId = sessionService.GetSessionID(username, password);

            _messageDispatcherService = new MessageDispatcherService(account, sessionId);
        }

        #region IEsendexSmsFacade Members

        public void SendSms(string recipient, string message)
        {
            var smsMessage = new SmsMessage {Body = message, Originator = "Appointment", Recipients = recipient};

            _messageDispatcherService.SendMessage(smsMessage, false);
        }

        #endregion
    }
}