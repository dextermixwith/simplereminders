using System;
using System.Collections.Generic;

namespace simplereminders.web.models.facades
{
    public interface IGoogleCalendarFacade
    {
        IList<Appointment> GetAppointments();
        Appointment GetAppointment(string eventId);
        IList<Appointment> GetAppointments(DateTime startDateTime, DateTime endDateTime);
    }
}