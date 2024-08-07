using FluentValidation;
using Onyx.Models.Domain;

namespace Onyx.Services.Validators
{
    public class NewProcessDataModelValidator : AbstractValidator<NewProcessDataModel>
    {
        public NewProcessDataModelValidator()
        {
            RuleFor(x => x.DUT).SetValidator(new DUTHeaderModelValidator());
        }
    }
}
