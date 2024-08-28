using FluentValidation;
using Tech.Challenge.I.Exceptions;

namespace Tech.Challenge.I.Application.UseCase.User;

public class PasswordValidator : AbstractValidator<string>
{
    public PasswordValidator()
    {
        RuleFor(password => password).NotEmpty().WithMessage(ErrorsMessages.BlankUserPassword);
        When(password => !string.IsNullOrWhiteSpace(password), () =>
        {
            RuleFor(password => password.Length).GreaterThanOrEqualTo(6).WithMessage(ErrorsMessages.MinimumSixCharacters);
        });
    }
}
