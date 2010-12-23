using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using simplereminders.web.controllers.viewmodels;
using simplereminders.web.models;
using simplereminders.web.models.database;
using simplereminders.web.models.facades;

namespace simplereminders.web.controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public const string INFO_KEY = "INFO_KEY";
        private IAppointmentsDb _appointmentsDb;
        private IGoogleCalendarFacade _calendarFacade;
        private IGoogleContactsFacade _contactsFacade;

        private string _googleUsername = ConfigurationManager.AppSettings["Google_Username"];
        private string _googlePassword = ConfigurationManager.AppSettings["Google_Password"];

        private string _esendexAccount = ConfigurationManager.AppSettings["Esendex_Account"];
        private string _esendexUsername = ConfigurationManager.AppSettings["Esendex_Username"];
        private string _esendexPassword = ConfigurationManager.AppSettings["Esendex_Password"];
       
        private IEsendexSmsFacade _esendexSmsFacade;
        

        public IGoogleCalendarFacade CalendarFacade
        {
            get { return _calendarFacade ?? (_calendarFacade = new GoogleCalendarFacade(_googleUsername, _googlePassword)); }
            set { _calendarFacade = value; }
        }

        public IEsendexSmsFacade EsendexSmsFacade
        {
            get
            {
                return _esendexSmsFacade ??
                       (_esendexSmsFacade = new EsendexSmsFacade(_esendexAccount, _esendexUsername, _esendexPassword));
            }
            set { _esendexSmsFacade = value; }
        }

        public IGoogleContactsFacade ContactsFacade
        {
            get { return _contactsFacade ?? (_contactsFacade = new GoogleContactsFacade(_googleUsername, _googlePassword)); }
            set { _contactsFacade = value; }
        }

        public IAppointmentsDb AppointmentsDb
        {
            get { return _appointmentsDb ?? (_appointmentsDb = new AppointmentsDb()); }
            set { _appointmentsDb = value; }
        }

        public ActionResult Index()
        {
            IndexViewModel viewModel = GetIndexViewModelForTimePeriod(string.Empty);

            return View("Index", viewModel);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SendRemindersForEvent(string eventId)
        {
            int messagesSentCount = SendRemindersForAppointment(eventId);

            TempData[INFO_KEY] = string.Format("<strong>{0}</strong> reminder SMS message(s) sent", messagesSentCount);

            return RedirectToAction("Index");
        }

        private int SendRemindersForAppointment(string eventId)
        {
            Appointment appointment = CalendarFacade.GetAppointment(eventId);

            string message = GenerateReminderMessage(appointment);

            appointment.Attendees = ContactsFacade.UpdateAttendeesWithMobileNumbers(appointment.Attendees);

            int messagesSentCount = 0;

            foreach (Attendee attendee in appointment.Attendees)
            {
                EsendexSmsFacade.SendSms(attendee.MobileNumber, message);
                messagesSentCount++;
            }

            var appointmentReminder = new AppointmentReminder
                                          {
                                              EventId = appointment.EventId,
                                              RemindersSent = true
                                          };

            ICollection<AttendeeReminder> attendeeReminders =
                appointment.Attendees.Select(attendee => new AttendeeReminder
                                                             {
                                                                 AppointmentReminder = appointmentReminder,
                                                                 EventId = appointment.EventId,
                                                                 Attendee = attendee.Email,
                                                                 Mobile = attendee.MobileNumber,
                                                                 Status = "NoResponse"
                                                             }).ToList();

            appointmentReminder.AttendeeReminders = attendeeReminders;

            AppointmentsDb.AddNewReminderDetails(appointmentReminder);

            return messagesSentCount;
        }

        private static string GenerateReminderMessage(Appointment appointment)
        {
            return
                string.Format(
                    "This is a gentle reminder for your [{0}] appointment today at [{1}] and expected to last [{2}] mins.",
                    appointment.Description,
                    appointment.StartingAt,
                    appointment.Duration);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(string timePeriod)
        {
            IndexViewModel viewModel = GetIndexViewModelForTimePeriod(timePeriod);

            return View("Index", viewModel);
        }

        private IndexViewModel GetIndexViewModelForTimePeriod(string timePeriod)
        {
            DateTime startDateTime;
            DateTime endDateTime;

            GetStartAndEndDates(timePeriod, out startDateTime, out endDateTime);

            IList<Appointment> appointmentList = CalendarFacade.GetAppointments(startDateTime, endDateTime);

            UpdateAttendeeStatuses(appointmentList);

            return new IndexViewModel
                       {
                           AppointmentList = appointmentList,
                           Info = TempData[INFO_KEY] as string
                       };
        }

        private void UpdateAttendeeStatuses(IList<Appointment> appointmentList)
        {
            foreach (Appointment appointment in appointmentList)
            {
                AppointmentReminder reminder = AppointmentsDb.GetReminderForEventId(appointment.EventId);
                if (reminder != null)
                {
                    foreach (Attendee attendee in appointment.Attendees)
                    {
                        AttendeeReminder attendeeReminder =
                            reminder.AttendeeReminders.Where(r => r.Attendee == attendee.Email).FirstOrDefault();
                        attendee.ResponseStatus = attendeeReminder.Status;
                    }
                }
            }
        }

        private void GetStartAndEndDates(string timePeriod, out DateTime startDateTime, out DateTime endDateTime)
        {
            startDateTime = DateTime.Today;
            endDateTime = DateTime.MaxValue;

            switch (timePeriod)
            {
                case "today":
                    startDateTime = DateTime.Today;
                    endDateTime = DateTime.Today.AddDays(1);
                    break;

                case "tomorrow":
                    startDateTime = DateTime.Today.AddDays(1);
                    endDateTime = DateTime.Today.AddDays(2);
                    break;

                case "thisweek":
                    startDateTime = DateTime.Today;
                    endDateTime = DateTime.Today.AddDays(7);
                    break;

                case "thismonth":
                    startDateTime = DateTime.Today;
                    endDateTime = DateTime.Today.AddMonths(1);
                    break;
            }
        }
    }
}