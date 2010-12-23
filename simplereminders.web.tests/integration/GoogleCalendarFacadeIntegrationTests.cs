using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using NUnit.Framework;
using simplereminders.web.models;
using simplereminders.web.models.facades;

namespace simplereminders.web.tests.integration
{
    [TestFixture]
    public class GoogleCalendarFacadeIntegrationTests
    {
        private readonly string _googleUsername = ConfigurationManager.AppSettings["Google_Username"];
        private readonly string _googlePassword = ConfigurationManager.AppSettings["Google_Password"];

        [Test]
        public void GoogleCalendarFacade_CanGetListOfAppointments()
        {
            IGoogleCalendarFacade facadeInstance = new GoogleCalendarFacade(_googleUsername, _googlePassword);

            IList<Appointment> appointments = facadeInstance.GetAppointments();
            
            Assert.That(appointments, Is.Not.Null);
            Assert.That(appointments, Is.Not.Empty);

            Assert.That(appointments[0].Attendees.Any(a => a.Email == _googleUsername), Is.False);
        }

        [Test, Ignore]
        public void GoogleCalendarFacade_CanSpecificEventByUid()
        {
            IGoogleCalendarFacade facadeInstance = new GoogleCalendarFacade(_googleUsername, _googlePassword);

            string eventId = "op5ub7tu721b75e2k6kfravei4";

            Appointment appointment = facadeInstance.GetAppointment(eventId);

            Assert.That(appointment, Is.Not.Null);
        }
    }
}