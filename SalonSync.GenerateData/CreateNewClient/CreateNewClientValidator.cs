using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HairApplication.Logic.CreateNewClient
{
    public class CreateNewClientValidator : AbstractValidator<CreateNewClientItem>
    {
        public CreateNewClientValidator()
        {

        }
    }
}
