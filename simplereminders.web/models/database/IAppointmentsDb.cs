namespace simplereminders.web.models.database
{
    public interface IAppointmentsDb
    {
        void AddNewReminderDetails(AppointmentReminder appointmentReminder);
        AppointmentReminder GetReminderForEventId(string eventId);
        void UpdateAttendeeReminder(string mobileNumber, string responseText);
    }
}