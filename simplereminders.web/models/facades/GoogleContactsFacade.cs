using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Google.Contacts;
using Google.GData.Client;
using Google.GData.Contacts;

namespace simplereminders.web.models.facades
{
    public class GoogleContactsFacade : GoogleFacadeBase, IGoogleContactsFacade
    {
        private Feed<Contact> _contactsFeed;
        private string _applicationName = "EsendexSimpleReminders";

        public GoogleContactsFacade(string googleUsername, string googlePassword)
            : base(googleUsername, googlePassword)
        {
            
        }

        public Feed<Contact> ContactsFeed
        {
            get
            {
                if (_contactsFeed == null)
                {
                    var query = new ContactsQuery(ContactsQuery.CreateContactsUri(HttpUtility.UrlEncode("default")));

                    var requestQuestSettings = new RequestSettings(_applicationName, _googleUsername, _googlePassword);
                    var contactsRequest = new ContactsRequest(requestQuestSettings);

                    _contactsFeed = contactsRequest.Get<Contact>(query);
                }
                return _contactsFeed;
            }
        }

        public Attendee Get(string emailAddress)
        {

            Attendee attendee = null;

            if (ContactsFeed.Entries.Count(e => e.Emails.Any(a => a.Address == emailAddress)) >= 1)
            {
                var contact = ContactsFeed.Entries.First(c => c.Emails.Any(a => a.Address == emailAddress));
                attendee = new Attendee
                               {
                                   Email = contact.Emails.Count > 0 ? contact.Emails[0].Address : string.Empty,
                                   MobileNumber = contact.Phonenumbers.Count > 0 ? contact.Phonenumbers[0].Value : string.Empty
                               };
            }

            return attendee;
        }

        public IList<Attendee> UpdateAttendeesWithMobileNumbers(IList<Attendee> existingAttendees)
        {

            Attendee retrievedAttendee = null;
            foreach (var attendee in existingAttendees)
            {
                retrievedAttendee = Get(attendee.Email);
                if (retrievedAttendee != null && !string.IsNullOrEmpty(retrievedAttendee.MobileNumber))
                {
                    attendee.MobileNumber = retrievedAttendee.MobileNumber;
                }
            }

            return existingAttendees;
        }

    }
}