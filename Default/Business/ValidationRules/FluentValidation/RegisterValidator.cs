using LCW.Core.Entities.Concrete;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace LCW.Business.ValidationRules.FluentValidation
{
    public class RegisterValidator : AbstractValidator<User>
    {
        public RegisterValidator()
        {
            RuleFor(t => t.IdentityNumber).NotNull().NotEmpty().Length(11).WithMessage("TC Kimlik No alanı doğru girilmedi");
            RuleFor(t => t.Email).NotNull().NotEmpty();
            RuleFor(x => x.Email).EmailAddress();
        }
    }
}


