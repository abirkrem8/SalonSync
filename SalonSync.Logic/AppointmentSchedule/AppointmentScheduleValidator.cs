using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.Logic.AppointmentSchedule
{
    public class AppointmentScheduleValidator : AbstractValidator<AppointmentScheduleItem>
    {
        public AppointmentScheduleValidator()
        {
            RuleFor(r => r).NotNull();
            RuleFor(r => r.FirstName).NotNull().NotEmpty().Unless(r => !r.IsNewClient);
            RuleFor(r => r.LastName).NotNull().NotEmpty().Unless(r => !r.IsNewClient);
            RuleFor(r => r.HairStylistId).NotNull().NotEmpty();
            RuleFor(r => r.PhoneNumber).NotNull().NotEmpty();
        }
    }
}
