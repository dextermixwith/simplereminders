using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace simplereminders.web.models.facades
{
    public interface IEsendexSmsFacade
    {
        void SendSms(string recipient, string message);
    }
}
