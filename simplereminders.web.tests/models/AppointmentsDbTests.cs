using System.Data.Entity.Database;
using NUnit.Framework;
using simplereminders.web.models.database;

namespace simplereminders.web.tests.models
{
    [TestFixture]
    public class AppointmentsDbTests
    {
        public AppointmentsDbTests()
        {
            DbDatabase.SetInitializer(new AppointmentsDbInitialiser());
        }

        [Test]
        public void AddAttendeeReminder_AddsNewRecord()
        {
            using (var db = new AppointmentsDb())
            {

                string eventId = "woohoo123";

                var appointmentReminder = new AppointmentReminder
                                              {
                                                  EventId = eventId,
                                                  RemindersSent = true
                                              };

                var attendeeReminder = new AttendeeReminder
                                           {
                                               EventId = eventId,
                                               Attendee = "me@you.com",
                                               Mobile = "+44123456789",
                                               Status = "Confirmed",
                                               AppointmentReminder = appointmentReminder
                                           };

                AttendeeReminder newAttendeeReminder = db.AttendeeReminders.Add(attendeeReminder);
                db.SaveChanges();

                AttendeeReminder retrievedAttendee = db.AttendeeReminders.Find(newAttendeeReminder.AttendeeReminderID);

                Assert.That(retrievedAttendee, Is.Not.Null);
                Assert.That(retrievedAttendee.EventId, Is.EqualTo(attendeeReminder.EventId));
                Assert.That(retrievedAttendee.Attendee, Is.EqualTo(attendeeReminder.Attendee));
                Assert.That(retrievedAttendee.Mobile, Is.EqualTo(attendeeReminder.Mobile));
                Assert.That(retrievedAttendee.Status, Is.EqualTo(attendeeReminder.Status));
                Assert.That(retrievedAttendee.AppointmentReminder.EventId, Is.EqualTo(appointmentReminder.EventId));
                Assert.That(retrievedAttendee.AppointmentReminder.RemindersSent,
                            Is.EqualTo(appointmentReminder.RemindersSent));
            }
        }

        [Test]
        public void AppointmentsDb_AddAppointmentReminder_AddNewRecord()
        {
            var db = new AppointmentsDb();

            var appointmentReminder = new AppointmentReminder
                                          {
                                              EventId = "xyz",
                                              RemindersSent = true
                                          };
            AppointmentReminder newReminder = db.AppointmentReminders.Add(appointmentReminder);
            db.SaveChanges();

            AppointmentReminder retrievedReminder = db.AppointmentReminders.Find(newReminder.AppointmentReminderID);

            Assert.That(retrievedReminder, Is.Not.Null);
            Assert.That(retrievedReminder.EventId, Is.EqualTo(appointmentReminder.EventId));
            Assert.That(retrievedReminder.RemindersSent, Is.EqualTo(appointmentReminder.RemindersSent));
        }
    }
}