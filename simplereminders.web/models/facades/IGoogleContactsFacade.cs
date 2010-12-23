using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace simplereminders.web.models.facades
{
    public interface IGoogleContactsFacade
    {
        Attendee Get(string emailAddress);
        IList<Attendee> UpdateAttendeesWithMobileNumbers(IList<Attendee> existingAttendees);
    }
}
