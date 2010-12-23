using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using NUnit.Framework;
using simplereminders.web.models;
using simplereminders.web.models.facades;

namespace simplereminders.web.tests.integration
{
    [TestFixture]
    public class GoogleContactsFacadeIntegrationTests
    {
        private readonly string _googleUsername = ConfigurationManager.AppSettings["Google_Username"];
        private readonly string _googlePassword = ConfigurationManager.AppSettings["Google_Password"];

        [Test]
        public void Get_RetrievesContactForSuppliedEmail()
        {
            var contactsFacade = new GoogleContactsFacade(_googleUsername, _googlePassword);

            string emailAddress = "test.contact@gmail.com";

            Attendee attendee = contactsFacade.Get(emailAddress);

            Assert.That(attendee, Is.Not.Null);
            Assert.That(attendee.Email, Is.EqualTo(emailAddress));
            Assert.That(attendee.MobileNumber, Is.Not.Null);
        }
    }
}
