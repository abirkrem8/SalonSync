﻿using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.Logic.Load.LoadAppointmentScheduleForm
{
    public class LoadAppointmentScheduleFormValidator : AbstractValidator<LoadAppointmentScheduleFormItem>
    {
        public LoadAppointmentScheduleFormValidator()
        {
            RuleFor(r => r).NotNull().NotEmpty();

        }
    }
}
