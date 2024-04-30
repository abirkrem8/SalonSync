using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalonSync.Models.Shared;

namespace SalonSync.Logic.AddAppointmentNotes
{
    public class AddAppointmentNotesResult
    {
        public AddAppointmentNotesResultStatus AddAppointmentNotesResultStatus { get; set; }
        public List<Error> AddAppointmentNotesResultErrors { get; set; }


    }



    public enum AddAppointmentNotesResultStatus
    {
        Success,
        ValidationError,
        DatabaseError
    }
}
