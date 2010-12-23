using System.Collections.Generic;

namespace simplereminders.web.models.database
{
    public class AttendeeReminder
    {
        public int AttendeeReminderID { get; set; }

        public string EventId { get; set; }

        public string Attendee { get; set; }

        public string Mobile { get; set; }

        public string Status { get; set; }

        public virtual AppointmentReminder AppointmentReminder { get; set; }
        
    }
}