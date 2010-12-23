using System;
using System.Collections.Specialized;
using System.Net;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using simplereminders.web.controllers;
using simplereminders.web.models.database;

namespace simplereminders.web.tests.controllers
{
    [TestFixture]
    public class NotificationsControllerTests
    {
        private NotificationsController _controller;


        [SetUp]
        public void SetupTest()
        {
            _controller = new NotificationsController();
        }


        [Test]
        public void Index_PostWithMessageRecievedNotificationType_RespondsWithContentResultOK()
        {
            NameValueCollection formValues = new NameValueCollection();

            string messageId = Guid.NewGuid().ToString();
            
            formValues.Add("NotificationType", "MessageReceived");
            formValues.Add("originator", "012345567");
            formValues.Add("body", "confirmed");

            _controller.ControllerContext = new ControllerContext();                          
   
            var formCollection = new FormCollection(formValues);

            ActionResult result = _controller.Index(formCollection);

            Assert.That(result, Is.InstanceOf(typeof(ContentResult)));

            var resultAsContentResult = (ContentResult) result;

            Assert.That(resultAsContentResult.Content.StartsWith("OK"), Is.True);
        }

        [Test]
        public void Index_PostWithMessageRecievedNotificationType_UpdatesAntendeeReminder()
        {
            NameValueCollection formValues = new NameValueCollection();

            var mobileNumber = "4412345567";
            var responseText = "confirmed";

            formValues.Add("NotificationType", "MessageReceived");

            formValues.Add("originator", mobileNumber);
            formValues.Add("body", responseText);

            var mockAppointmentsDb = new Mock<IAppointmentsDb>();

            mockAppointmentsDb.Setup(a => a.UpdateAttendeeReminder(mobileNumber, responseText));
            _controller.AppointmentsDb = mockAppointmentsDb.Object;
            _controller.ControllerContext = new ControllerContext();

            var formCollection = new FormCollection(formValues);

            _controller.Index(formCollection);

            mockAppointmentsDb.VerifyAll();

        }

        //[Test]
        //public void Index_PostWithMessageDelieveredNotificationType_RespondsWithContentResultBadRequest()
        //{
        //    NameValueCollection formValues = new NameValueCollection();

        //    string messageId = Guid.NewGuid().ToString();

        //    formValues.Add("NotificationType", "MessageDelivered");

        //    _controller.ControllerContext = new ControllerContext();

        //    var formCollection = new FormCollection(formValues);

        //    ActionResult result = _controller.Index(formCollection);

        //    Assert.That(result, Is.InstanceOf(typeof(ContentResult)));

        //    var resultAsContentResult = (ContentResult)result;

        //    Assert.That(resultAsContentResult.Content.StartsWith("X"), Is.True);
        //}
    }
}