using Onyx.Models.Domain;
using FluentValidation;

namespace Onyx.Services.Validators
{
    public class ProcessDataModelValidator : AbstractValidator<ProcessDataModel>
    {
        public ProcessDataModelValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.DUT).SetValidator(new DUTHeaderModelValidator());
        }
    }
}
