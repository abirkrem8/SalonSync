﻿using SalonSync.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.Logic.AppointmentSchedule
{
    public class AppointmentScheduleItem
    {
        // if true, add client to DB
        public bool IsNewClient { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfAppointment { get; set; }
        public DateTime TimeOfAppointment { get; set; }
        public string HairStylistId { get; set; }
        public AppointmentType AppointmentType { get; set; }
        public HairTexture ClientHairTexture { get; set; }
        public HairLength ClientHairLength { get; set; }

        public bool HistoricalAppointmentSchedule { get; set; }

    }
}
