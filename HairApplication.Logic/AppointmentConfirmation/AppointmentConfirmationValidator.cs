using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HairApplication.Logic.AppointmentConfirmation
{
    public class AppointmentConfirmationValidator : AbstractValidator<AppointmentConfirmationItem>
    {
        public AppointmentConfirmationValidator()
        {

        }
    }
}
