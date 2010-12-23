using System;
using System.Data.Entity;
using System.Linq;

namespace simplereminders.web.models.database
{
    public class AppointmentsDb : DbContext, IAppointmentsDb
    {
        public DbSet<AppointmentReminder> AppointmentReminders { get; set; }
        public DbSet<AttendeeReminder> AttendeeReminders { get; set; }

        public void AddNewReminderDetails(AppointmentReminder appointmentReminder)
        {
            AppointmentReminders.Add(appointmentReminder);
            SaveChanges();
        }

        public AppointmentReminder GetReminderForEventId(string eventId)
        {
            return AppointmentReminders.ToList().Where(a => a.EventId == eventId).FirstOrDefault();
        }

        public void UpdateAttendeeReminder(string mobileNumber, string responseText)
        {
            var normaliseMobileNumber = NormaliseMobileNumber(mobileNumber);
            var attendeeReminder =
                AttendeeReminders.Where(r => r.Mobile == normaliseMobileNumber && r.Status.ToLower() == "noresponse").OrderBy(
                    r => r.AttendeeReminderID).FirstOrDefault();

            if (attendeeReminder != null)
            {
                attendeeReminder.Status = GetAttendeStatusFromResponseText(responseText);
                SaveChanges();
            }
        }

        private string NormaliseMobileNumber(string mobileNumber)
        {
            // TODO : this is very very bad!
            return mobileNumber.Replace("44", "0");
        }

        private string GetAttendeStatusFromResponseText(string responseText)
        {
            switch (responseText.ToLower().Trim())
            {
                case "cancel":
                    return "cancelled";
                case "confirm":
                    return "confirmed";
            }

            return "noresponse";
        }
    }
}