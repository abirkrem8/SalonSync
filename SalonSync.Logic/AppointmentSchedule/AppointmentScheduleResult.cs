using SalonSync.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.Logic.AppointmentSchedule
{
    public class AppointmentScheduleResult
    {
        public string ClientFullName { get; set; }
        public string StylistFullName { get; set; }
        public DateTime TimeOfAppointment { get; set; }

        public string AppointmentId { get; set; }


        public AppointmentScheduleResultStatus AppointmentScheduleResultStatus { get; set; }
        public List<Error> AppointmentScheduleResultErrors { get; set; } = new List<Error>();
    }



    public enum AppointmentScheduleResultStatus
    {
        Success,
        DatabaseError,
        ValidationError,
        NoExistingClient
    }
}

