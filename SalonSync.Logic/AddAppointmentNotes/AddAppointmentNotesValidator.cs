using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.Logic.AddAppointmentNotes
{
    public class AddAppointmentNotesValidator : AbstractValidator<AddAppointmentNotesItem>
    {
        public AddAppointmentNotesValidator()
        {
            RuleFor(x => x).NotNull().NotEmpty();
            RuleFor(x => x.AppointmentId).NotNull().NotEmpty();
            RuleFor(x => x.NoteText).NotNull().NotEmpty();
        }
    }
}
