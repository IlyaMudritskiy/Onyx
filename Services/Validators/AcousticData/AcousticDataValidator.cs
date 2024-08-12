using FluentValidation;
using Onyx.Models.Domain.AcousticData;

namespace Onyx.Services.Validators.AcousticData
{
    public class AcousticDataValidator : AbstractValidator<AcousticDataModel>
    {
        public AcousticDataValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.DUT).SetValidator(new AcousticDutModelValidator());
        }
    }
}
