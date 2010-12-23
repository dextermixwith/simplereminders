using System.Net;
using System.Web.Mvc;
using simplereminders.web.models.database;

namespace simplereminders.web.controllers
{
    public class NotificationsController : Controller
    {
        private IAppointmentsDb _appointmentsDb;


        public IAppointmentsDb AppointmentsDb
        {
            get { return _appointmentsDb ?? (_appointmentsDb = new AppointmentsDb()); }
            set { _appointmentsDb = value; }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(FormCollection formCollection)
        {
            string notificationType = formCollection["NotificationType"];

            if (notificationType == "MessageReceived")
            {
                string originator = formCollection["originator"];
                string body = formCollection["body"];

                if (!string.IsNullOrEmpty(originator) && !string.IsNullOrEmpty(body))
                {
                    AppointmentsDb.UpdateAttendeeReminder(originator, body);
                    return Content(string.Format("OK : MessageRecieved from [{0}] with body [{1}]", originator, body));
                }
            }

            Response.StatusCode = (int) HttpStatusCode.BadRequest;

            return Content("BadRequest");
        }
    }
}