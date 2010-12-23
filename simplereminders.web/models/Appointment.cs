using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace simplereminders.web.models
{
    public class Appointment
    {
        private IList<Attendee> _attendees = new List<Attendee>();

        public string Description { get; set; }

        public DateTime StartingAt { get; set; }

        public DateTime EndingAt { get; set; }

        public string Location { get; set; }

        public IList<Attendee> Attendees
        {
            get {
                return _attendees;
            }
            set {
                _attendees = value;
            }
        }

        public int Duration
        {
            get { return Convert.ToInt32((EndingAt - StartingAt).TotalMinutes); }
        }

        public string AttendeesAsCsv
        {
            get 
            { 
                var csvBuilder = new StringBuilder();

                // TODO: Should be name really
                foreach (var attendee in Attendees)
                {
                    var attendeeName = string.Format("{0} ({1})", attendee.Email, (!string.IsNullOrEmpty(attendee.ResponseStatus) ? attendee.ResponseStatus : "No Reminder Sent")).Replace("NoResponse", "No Response");
                    csvBuilder.Append(string.Concat(csvBuilder.Length > 0 ? "," : string.Empty, attendeeName));
                }

                return csvBuilder.ToString();

            }
        }

        public string EventId { get; set; }
    }
}