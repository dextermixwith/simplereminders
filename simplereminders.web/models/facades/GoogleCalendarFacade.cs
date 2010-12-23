using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.GData.Extensions;
using Google.GData.Calendar;

namespace simplereminders.web.models.facades
{
    public class GoogleCalendarFacade : GoogleFacadeBase, IGoogleCalendarFacade
    {
        private Uri _calendarUri = new Uri("http://www.google.com/calendar/feeds/default/private/full");

        public GoogleCalendarFacade(string googleUsername, string googlePassword)
            : base(googleUsername, googlePassword)
        {
            
        }

        public Uri CalendarUri
        {
            get { return _calendarUri; }
            set { _calendarUri = value; }
        }

        public IList<Appointment> GetAppointments()
        {

            List<Appointment> appointments = GetListOfAppointments(DateTime.Today, DateTime.MaxValue);

            return appointments.OrderBy(a => a.StartingAt).ToList();
        }

        private List<Appointment> GetListOfAppointments(DateTime startDateTime, DateTime endDateTime)
        {
            var appointments = new List<Appointment>();

            var query = new EventQuery();

            CalendarService calendarService = GetCalendarService(query, null, startDateTime, endDateTime);

            var calFeed = calendarService.Query(query);

            while (calFeed != null && calFeed.Entries.Count > 0)
            {
                appointments.AddRange(from EventEntry entry in calFeed.Entries
                                      select GetAppointmentFromEventEntry(entry));
                if (calFeed.NextChunk != null)
                {
                    query.Uri = new Uri(calFeed.NextChunk);
                    calFeed = calendarService.Query(query);
                }
                else
                {
                    calFeed = null;
                }
            }
            return appointments;
        }

        private Appointment GetAppointmentFromEventEntry(EventEntry entry)
        {
            return new Appointment
                       {
                           EventId = entry.EventId,
                           Description = entry.Title.Text, 
                           StartingAt = entry.Times[0].StartTime, 
                           EndingAt = entry.Times[0].EndTime,
                           Attendees = GetAppointmentAttendees(entry)
                       };
        }

        private CalendarService GetCalendarService(EventQuery query, string eventId, DateTime startDateTime, DateTime endDateTime)
        {
            query.Uri = CalendarUri;
            query.Uri = new Uri(string.Concat(query.Uri.ToString(), !string.IsNullOrEmpty(eventId) ? string.Concat("/", eventId) : string.Empty));

            query.StartTime = startDateTime;
            query.EndTime = endDateTime;

            var calendarService = new CalendarService(_applicationName);
            calendarService.setUserCredentials(_googleUsername, _googlePassword);
            calendarService.QueryClientLoginToken();
            return calendarService;
        }

        public Appointment GetAppointment(string eventId)
        {
            Appointment appointment = null;

            var query = new EventQuery();

            CalendarService calendarService = GetCalendarService(query, eventId, DateTime.MinValue, DateTime.MaxValue);

            var calFeed = calendarService.Query(query);
            if (calFeed.Entries.Any())
            {
                var entry = (EventEntry) calFeed.Entries.First();
                appointment = GetAppointmentFromEventEntry(entry);
            }

            return appointment;
        }

        public IList<Appointment> GetAppointments(DateTime startDateTime, DateTime endDateTime)
        {

            List<Appointment> appointments = GetListOfAppointments(startDateTime, endDateTime);

            return appointments.OrderBy(a => a.StartingAt).ToList();
        }

        private IList<Attendee> GetAppointmentAttendees(EventEntry eventEntry)
        {
            return eventEntry.Participants.Where(p => p.Email != _googleUsername)
                                          .Select(participant => new Attendee
                                                                     {
                                                                         Email = participant.Email
                                                                     }).ToList();
        }
    }
}