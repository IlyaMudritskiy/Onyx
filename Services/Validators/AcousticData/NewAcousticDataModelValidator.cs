using FluentValidation;
using Onyx.Models.Domain.AcousticData;

namespace Onyx.Services.Validators.AcousticData
{
    public class NewAcousticDataModelValidator : AbstractValidator<NewAcousticDataModel>
    {
        public NewAcousticDataModelValidator()
        {
            RuleFor(x => x.DUT).SetValidator(new AcousticDutModelValidator());
        }
    }
}
