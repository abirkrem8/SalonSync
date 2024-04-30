using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.Logic.Load.LoadClientList
{
    public class LoadClientListValidator : AbstractValidator<LoadClientListItem>
    {
        public LoadClientListValidator()
        {
            RuleFor(r => r).NotNull().NotEmpty();
        }
    }
}
