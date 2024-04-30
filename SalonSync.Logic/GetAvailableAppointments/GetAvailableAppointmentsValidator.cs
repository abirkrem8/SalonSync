using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.Logic.GetAvailableAppointments
{
    public class GetAvailableAppointmentsValidator : AbstractValidator<GetAvailableAppointmentsItem>
    {
        public GetAvailableAppointmentsValidator()
        {
            RuleFor(r => r).NotNull().NotEmpty();
            RuleFor(r => r.StylistId).NotNull().NotEmpty();
            RuleFor(r => r.StartDate).NotNull().NotEmpty();
            RuleFor(r => r.EndDate).NotNull().NotEmpty();
            RuleFor(r => r.EndDate).NotNull().NotEmpty().GreaterThan(x => x.StartDate);
        }
    }
}
