﻿using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.Logic.Load.LoadIndexScreen
{
    public class LoadIndexScreenValidator : AbstractValidator<LoadIndexScreenItem>
    {
        public LoadIndexScreenValidator()
        {
            RuleFor(r => r).NotNull().NotEmpty();
        }
    }
}
