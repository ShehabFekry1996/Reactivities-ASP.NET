using Application.Profiles.Commands;
using FluentValidation;

namespace Application.Profiles.Validators
{
    public class EditProfileValidator : AbstractValidator<EditProfile.Command>
    {
        public EditProfileValidator()
        {
            RuleFor(x => x.DisplayName).NotEmpty().WithMessage("Display name is required")
                .MaximumLength(50).WithMessage("Display name must not exceed 50 characters");
            RuleFor(x => x.Bio).MaximumLength(500).WithMessage("Bio must not exceed 500 characters");
        }
    }
}
