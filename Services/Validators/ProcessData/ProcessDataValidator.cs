using FluentValidation;
using Onyx.Models.Domain.ProcessData;

namespace Onyx.Services.Validators.ProcessData
{
    public class ProcessDataModelValidator : AbstractValidator<ProcessDataModel>
    {
        public ProcessDataModelValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.DUT).SetValidator(new DUTModelProcessValidator());
        }
    }
}
