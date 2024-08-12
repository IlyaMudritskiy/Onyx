using FluentValidation;
using Onyx.Models.Domain.ProcessData;

namespace Onyx.Services.Validators.ProcessData
{
    public class NewProcessDataModelValidator : AbstractValidator<NewProcessDataModel>
    {
        public NewProcessDataModelValidator()
        {
            RuleFor(x => x.DUT).SetValidator(new DUTModelProcessValidator());
        }
    }
}
