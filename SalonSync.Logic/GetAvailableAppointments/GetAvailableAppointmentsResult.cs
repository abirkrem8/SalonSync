using SalonSync.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.Logic.GetAvailableAppointments
{
    public class GetAvailableAppointmentsResult
    {
        public List<DateTime> AvailableAppointments { get; set; }
        public GetAvailableAppointmentsResultStatus GetAvailableAppointmentsResultStatus { get; set; }
        public List<Error> GetAvailableAppointmentsResultErrors { get; set; }
    }



    public enum GetAvailableAppointmentsResultStatus
    {
        Success,
        ValidationError
    }
}
