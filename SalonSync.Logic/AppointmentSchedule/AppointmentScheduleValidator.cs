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
            RuleFor(r => r.AppointmentType).NotNull();
            RuleFor(r => r.ClientHairLength).NotNull().NotEmpty().Unless(x => !x.IsNewClient);
            RuleFor(r => r.ClientHairTexture).NotNull().NotEmpty().Unless(x => !x.IsNewClient);
            RuleFor(r => r.DateOfAppointment.Date).NotNull().NotEmpty().GreaterThanOrEqualTo(DateTime.Now.Date).Unless(x => x.HistoricalAppointmentSchedule);
            RuleFor(r => r.DateOfAppointment.Date).NotNull().NotEmpty().LessThanOrEqualTo(DateTime.Now.Date.AddDays(30));
            RuleFor(r => r.TimeOfAppointment.TimeOfDay).NotNull().NotEmpty().InclusiveBetween(new TimeSpan(8, 0, 0), new TimeSpan(16, 0, 0));
        }
    }
}
