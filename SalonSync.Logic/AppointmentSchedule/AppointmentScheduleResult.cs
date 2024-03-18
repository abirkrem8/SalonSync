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
        public AppointmentScheduleResultStatus AppointmentScheduleResultStatus { get; set; }
        public List<Error> AppointmentScheduleResultErrors { get; set; } = new List<Error>();
    }



    public enum AppointmentScheduleResultStatus
    {
        Success,
        DatabaseError,
        ValidationError
    }
}

