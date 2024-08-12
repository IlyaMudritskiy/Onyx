using FluentValidation;
using Onyx.Models.Domain.AcousticData;

namespace Onyx.Services.Validators.AcousticData
{
    public class AcousticDutModelValidator : AbstractValidator<AcousticDutModel>
    {
        public class DUTModelProcessValidator : AbstractValidator<AcousticDutModel>
        {
            public DUTModelProcessValidator()
            {
                RuleFor(x => x.SerialNr)
                    .NotEmpty().WithMessage("serial_nr is required.")
                    .Length(15, 15).WithMessage("serial_nr must be exactly 15 characters long.");

                RuleFor(x => x.TypeID).NotEmpty().WithMessage("type_id is required.");
                RuleFor(x => x.TypeName).NotEmpty().WithMessage("typename is required.");
                RuleFor(x => x.TestSystem).NotEmpty().WithMessage("system is required.");
                RuleFor(x => x.WorkOrder).NotEmpty().WithMessage("workorder is required.");
                RuleFor(x => x.RunningNr).NotEmpty().WithMessage("runningnr is required.");
                RuleFor(x => x.ExecutionTime).NotEmpty().WithMessage("executiontime(s) is required.");
                RuleFor(x => x.DutTime).NotEmpty().WithMessage("duttime is required.");

                RuleFor(x => x)
                    .Custom((model, context) =>
                    {
                        var lastTwoChars = model.SerialNr.Substring(model.SerialNr.Length - 2);
                        if (!lastTwoChars.All(c => c == '1' || c == '2'))
                            context.AddFailure("serial_nr is incorrect (track and press).");
                    });
            }
        }
    }
}
