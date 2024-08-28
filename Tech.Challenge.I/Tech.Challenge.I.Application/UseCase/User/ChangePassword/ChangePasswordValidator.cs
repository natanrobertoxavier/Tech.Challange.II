using FluentValidation;
using Tech.Challenge.I.Communication.Request;

namespace Tech.Challenge.I.Application.UseCase.User.ChangePassword;
public class ChangePasswordValidator : AbstractValidator<RequestChangePasswordJson>
{
    public ChangePasswordValidator()
    {
        RuleFor(c => c.NewPassword).SetValidator(new PasswordValidator());
    }
}