using Entities.Concrete;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.ValidationRules.FluentValidation
{
    public class AssignTaskValidator : AbstractValidator<Expense>
    {
        /*Nesneye girilen değerlerle ilgili validationlar*/
        public AssignTaskValidator()
        {
            //RuleFor(t => t.CreatedOn).NotNull();
            //RuleFor(t => t.CreatedBy).NotNull().NotEqual(Guid.Empty);
            ////RuleFor(t => t.AssignedUserId).NotNull().NotEqual(Guid.Empty).WithMessage("Lütfen atanacak kullanıcı alanını boş geçmeyiniz.");
            //RuleFor(t => t.TaskId).NotNull().NotEqual(Guid.Empty);
        }
    }
}
