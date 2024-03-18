﻿using SalonSync.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace SalonSync.MVC.Models
{
    public class AppointmentEntryViewModel
    {
        public List<HairStylist> AvailableStylists { get; set; }

        [Display(Name = "Hair Stylist")]
        public string SelectedStylist { get; set; }
        [Display(Name = "Client First Name")]
        public string ClientFirstName { get; set; }

        [Display(Name = "Client Last Name")]
        public string ClientLastName { get; set; }

        [Display(Name = "Client Phone Number")]
        public string ClientPhoneNumber { get; set; }

        [Display(Name = "New Client?")]
        public bool IsNewClient { get; set; }

        [Display(Name = "Date of Appointment")]
        public DateTime DateOfAppointment { get; set; }

        [Display(Name = "Time of Appointment")]
        public DateTime TimeOfAppointment { get; set; }
    }
}