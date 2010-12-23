using System;
using System.Collections.Generic;

namespace simplereminders.web.models.database
{
    public class AppointmentReminder
    {
        public int AppointmentReminderID { get; set; }

        public string EventId { get; set; }

        public bool RemindersSent { get; set; }

        public virtual ICollection<AttendeeReminder> AttendeeReminders { get; set; }
    }
}