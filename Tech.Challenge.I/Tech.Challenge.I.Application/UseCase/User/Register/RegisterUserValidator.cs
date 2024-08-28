using FluentValidation;
using Tech.Challenge.I.Communication.Request;
using Tech.Challenge.I.Exceptions;

namespace Tech.Challenge.I.Application.UseCase.User.Register;

public class RegisterUserValidator : AbstractValidator<RequestRegisterUserJson>
{
    public RegisterUserValidator()
    {
        RuleFor(c => c.Name).NotEmpty().WithMessage(ErrorsMessages.BlankUserName);
        RuleFor(c => c.Email).NotEmpty().WithMessage(ErrorsMessages.BlankUserEmail);
        RuleFor(c => c.Password).SetValidator(new PasswordValidator());
        When(c => !string.IsNullOrWhiteSpace(c.Email), () =>
        {
            RuleFor(c => c.Email).EmailAddress().WithMessage(ErrorsMessages.InvalidUserEmail);
        });
    }
}
