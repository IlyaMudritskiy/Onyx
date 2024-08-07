using FluentValidation;
using Onyx.Models.Domain;

namespace Onyx.Services.Validators
{
    public class DUTHeaderModelValidator : AbstractValidator<DUTHeaderModel>
    {
        public DUTHeaderModelValidator()
        {
            RuleFor(x => x.SerialNr)
                .NotEmpty().WithMessage("serial_nr is required.")
                .Length(15, 15).WithMessage("serial_nr must be exactly 15 characters long.");

            RuleFor(x => x.TypeID).NotEmpty().WithMessage("type_id is required.");
            RuleFor(x => x.SystemType).NotEmpty().WithMessage("system_type is required.");
            RuleFor(x => x.Line).NotEmpty().WithMessage("machine_id is required.");

            RuleFor(x => x.Track).Must(BeWithinValidRange).WithMessage("track_nr must be 1 or 2.");
            RuleFor(x => x.Press).Must(BeWithinValidRange).WithMessage("ps01_press_nr must be 1 or 2.");

            RuleFor(x => x.WpcNumber).NotNull().WithMessage("wpc_number is required.");

            RuleFor(x => x)
                .Custom((model, context) =>
                {
                    if (model.Track.HasValue && model.Press.HasValue)
                    {
                        var serialLastTwoChars = model.SerialNr.Substring(model.SerialNr.Length - 2);
                        var trackLastChar = model.Track.Value.ToString();
                        var pressLastChar = model.Press.Value.ToString();

                        if (serialLastTwoChars != $"{trackLastChar}{pressLastChar}")
                        {
                            context.AddFailure("serial_nr does not match Track and Press.");
                        }
                    }
                });
        }

        private bool BeWithinValidRange(int? value)
        {
            return value.HasValue && (value.Value == 1 || value.Value == 2);
        }
    }
}
