using System;
using System.Collections.Generic;
using simplereminders.web.models;

namespace simplereminders.web.controllers.viewmodels
{
    public class IndexViewModel
    {
        private IList<Appointment> _appointmentList = new List<Appointment>();

        public IList<Appointment> AppointmentList
        {
            get { return _appointmentList; }
            set { _appointmentList = value; }
        }

        public string Info { get; set; }
    }
}