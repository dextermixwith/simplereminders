using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using simplereminders.web.controllers;
using simplereminders.web.controllers.viewmodels;
using simplereminders.web.models;
using simplereminders.web.models.database;
using simplereminders.web.models.facades;

namespace simplereminders.web.tests.controllers
{
    [TestFixture]
    public class HomeControllerTests
    {
        #region Setup/Teardown

        [SetUp]
        public void TestSetup()
        {
            _mockGoogleCalendarFacade = new Mock<IGoogleCalendarFacade>();
            _mockEsendexSmsFacade = new Mock<IEsendexSmsFacade>();
            _mockGoogleContactsFacade = new Mock<IGoogleContactsFacade>();

            _controller = new HomeController
                              {
                                  CalendarFacade = _mockGoogleCalendarFacade.Object,
                                  EsendexSmsFacade = _mockEsendexSmsFacade.Object,
                                  ContactsFacade = _mockGoogleContactsFacade.Object,
                                  AppointmentsDb = _mockAppointmentsDb.Object
                              };
        }

        [TearDown]
        public void TestTearDown()
        {
            _mockGoogleCalendarFacade.VerifyAll();
            _mockGoogleContactsFacade.VerifyAll();
            _mockEsendexSmsFacade.VerifyAll();
            _mockAppointmentsDb.VerifyAll();
        }

        #endregion

        private HomeController _controller;
        private Mock<IGoogleCalendarFacade> _mockGoogleCalendarFacade;
        private Mock<IEsendexSmsFacade> _mockEsendexSmsFacade;
        private Mock<IGoogleContactsFacade> _mockGoogleContactsFacade;
        private readonly IList<Appointment> _sampleAppointments;
        private readonly Mock<IAppointmentsDb> _mockAppointmentsDb = new Mock<IAppointmentsDb>();

        public HomeControllerTests()
        {
            _sampleAppointments = new List<Appointment>
                                      {
                                          new Appointment
                                              {
                                                  Description = "Appointment 1",
                                                  Location = "Room 1 - Our Office",
                                                  StartingAt = new DateTime(2010, 10, 1, 12, 0, 0),
                                                  EndingAt = new DateTime(2010, 10, 1, 13, 0, 0),
                                                  Attendees = new List<Attendee>
                                                                  {
                                                                      new Attendee
                                                                          {
                                                                              Email = "mrx@acompany.com",
                                                                              ResponseStatus = "Confirmed"
                                                                          }
                                                                  }
                                              },
                                          new Appointment
                                              {
                                                  Description = "Appointment 2",
                                                  Location = "Room 2 - Our Office",
                                                  StartingAt = new DateTime(2010, 10, 1, 13, 0, 0),
                                                  EndingAt = new DateTime(2010, 10, 1, 14, 0, 0)
                                              },
                                          new Appointment
                                              {
                                                  Description = "Appointment 3",
                                                  Location = "Room 1 - Our Office",
                                                  StartingAt = new DateTime(2010, 10, 1, 14, 0, 0),
                                                  EndingAt = new DateTime(2010, 10, 1, 15, 0, 0)
                                              }
                                      };
        }

        private ActionResult RunBasicSendRemindersForEventTest()
        {
            string eventId = "op5ub7tu721b75e2k6kfravei4";

            var attendee = new Attendee
                               {
                                   Email = "bob@esendex.com"
                               };
            var attendees = new List<Attendee> {attendee};
            var attendeesWithMobileNumbers = new List<Attendee>
                                                 {
                                                     new Attendee
                                                         {
                                                             Email = "bob@esendex.com",
                                                             MobileNumber = "0123456789"
                                                         }
                                                 };


            var appointment = new Appointment
                                  {
                                      EventId = eventId,
                                      Description = "Appointment 3",
                                      Location = "Room 1 - Our Office",
                                      StartingAt = new DateTime(2010, 10, 1, 14, 0, 0),
                                      EndingAt = new DateTime(2010, 10, 1, 15, 0, 0),
                                      Attendees = attendees
                                  };

            var numbers = new List<string> {"0123456789"};

            _mockGoogleCalendarFacade
                .Setup(c => c.GetAppointment(eventId))
                .Returns(appointment);

            _mockGoogleContactsFacade
                .Setup(c => c.UpdateAttendeesWithMobileNumbers(attendees))
                .Returns(attendeesWithMobileNumbers);

            _mockEsendexSmsFacade
                .Setup(s => s.SendSms(numbers[0], It.IsAny<string>()));

            _mockAppointmentsDb
                .Setup(
                    a =>
                    a.AddNewReminderDetails(
                        It.Is<AppointmentReminder>(
                            r =>
                            r.EventId == eventId && r.RemindersSent &&
                            r.AttendeeReminders.First().Mobile == attendeesWithMobileNumbers[0].MobileNumber &&
                            r.AttendeeReminders.First().EventId == eventId)));

            return _controller.SendRemindersForEvent(eventId);
        }

        [Test]
        public void Index_Post_WithAppointmentTimePeriodToday_OnlyShowsAppointmentsForToday()
        {
            string timePeriod = "today";

            DateTime beginningOfToday = DateTime.Today;
            DateTime endOfToday = DateTime.Today.AddDays(1);

            _mockGoogleCalendarFacade
                .Setup(c => c.GetAppointments(beginningOfToday, endOfToday))
                .Returns(_sampleAppointments);

            var model = (IndexViewModel) ((ViewResult) _controller.Index(timePeriod)).ViewData.Model;

            Assert.That(model.AppointmentList, Is.Not.Null);
            Assert.That(model.AppointmentList, Is.InstanceOf(typeof (IList<Appointment>)));
            Assert.That(model.AppointmentList, Is.Not.Empty);
        }

        [Test]
        public void Index_ReturnsIndexViewResult()
        {
            _mockGoogleCalendarFacade
                .Setup(c => c.GetAppointments(DateTime.Today, DateTime.MaxValue))
                .Returns(_sampleAppointments);

            ActionResult result = _controller.Index();

            Assert.That(result, Is.InstanceOf(typeof (ViewResult)));

            var resultAsViewResult = (ViewResult) result;

            Assert.That(resultAsViewResult.ViewName, Is.EqualTo("Index"));
        }

        [Test]
        public void Index_ViewModel_ContainsListOfAppopintments()
        {
            _mockGoogleCalendarFacade
                .Setup(c => c.GetAppointments(DateTime.Today, DateTime.MaxValue))
                .Returns(_sampleAppointments);

            var model = (IndexViewModel) ((ViewResult) _controller.Index()).ViewData.Model;

            Assert.That(model.AppointmentList, Is.Not.Null);
            Assert.That(model.AppointmentList, Is.InstanceOf(typeof (IList<Appointment>)));
            Assert.That(model.AppointmentList.Count, Is.EqualTo(3));

            Assert.That(model.AppointmentList[0].Description, Is.EqualTo(_sampleAppointments[0].Description));
            Assert.That(model.AppointmentList[0].Location, Is.EqualTo(_sampleAppointments[0].Location));
            Assert.That(model.AppointmentList[0].StartingAt, Is.EqualTo(_sampleAppointments[0].StartingAt));
            Assert.That(model.AppointmentList[0].EndingAt, Is.EqualTo(_sampleAppointments[0].EndingAt));
            Assert.That(model.AppointmentList[0].Attendees.Count, Is.EqualTo(1));
            Assert.That(model.AppointmentList[0].Attendees[0].Email,
                        Is.EqualTo(_sampleAppointments[0].Attendees[0].Email));
            Assert.That(model.AppointmentList[0].Attendees[0].ResponseStatus,
                        Is.EqualTo(_sampleAppointments[0].Attendees[0].ResponseStatus));
        }

        [Test]
        public void Index_ViewModel_IsIndexViewModel()
        {
            _mockGoogleCalendarFacade
                .Setup(c => c.GetAppointments(DateTime.Today, DateTime.MaxValue))
                .Returns(_sampleAppointments);

            var result = (ViewResult) _controller.Index();

            Assert.That(result.ViewData.Model, Is.InstanceOf(typeof (IndexViewModel)));
        }

        [Test]
        public void Index_WithInfoInTempData_TempDataIsPopulatedInIndexViewModel()
        {
            const string info = "Test mofo string";
            _controller.TempData[HomeController.INFO_KEY] = info;

            _mockGoogleCalendarFacade
                .Setup(c => c.GetAppointments(DateTime.Today, DateTime.MaxValue))
                .Returns(_sampleAppointments);

            var result = (ViewResult) _controller.Index();

            Assert.That(((IndexViewModel) result.ViewData.Model).Info, Is.EqualTo(info));
        }

        [Test]
        public void SendRemindersForEvent_Post_RetrievesCalendarEventAndContactThenSendsMessageToContacts()
        {
            RunBasicSendRemindersForEventTest();
        }

        [Test]
        public void SendRemindersForEvent_Post_ReturnsRedirectToRouteIndexAndAddsMessageToTempData()
        {
            ActionResult result = RunBasicSendRemindersForEventTest();

            Assert.That(result, Is.InstanceOf(typeof (RedirectToRouteResult)));
            Assert.That(((RedirectToRouteResult) result).RouteValues["action"], Is.EqualTo("Index"));
            Assert.That(_controller.TempData[HomeController.INFO_KEY], Is.Not.Null);
        }
    }
}