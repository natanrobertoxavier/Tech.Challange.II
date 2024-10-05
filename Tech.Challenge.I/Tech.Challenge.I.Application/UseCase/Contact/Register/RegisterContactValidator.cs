using FluentValidation;
using System.Text.RegularExpressions;
using Tech.Challenge.I.Communication.Request;
using Tech.Challenge.I.Exceptions;

namespace Tech.Challenge.I.Application.UseCase.Contact.Register;
public class RegisterContactValidator : AbstractValidator<RequestContactJson>
{
    public RegisterContactValidator()
    {
        RuleFor(p => p.FirstName)
            .NotEmpty()
            .WithMessage(ErrorsMessages.BlankFirstName);

        RuleFor(p => p.LastName)
            .NotEmpty()
            .WithMessage(ErrorsMessages.BlankLastName);

        RuleFor(p => p.DDD)
            .NotNull()
            .WithMessage(ErrorsMessages.BlankDDD);

        RuleFor(p => p.PhoneNumber)
            .NotEmpty()
            .WithMessage(ErrorsMessages.BlankPhoneNumber);

        RuleFor(p => p.Email)
            .NotEmpty()
            .WithMessage(ErrorsMessages.BlankEmail);

        When(c => !string.IsNullOrEmpty(c.Email), () =>
        {
            RuleFor(c => c.Email).Custom((email, context) =>
            {
                string pattern = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
                var isMatch = Regex.IsMatch(email, pattern);

                if (!isMatch)
                {
                    context.AddFailure(new FluentValidation.Results
                        .ValidationFailure(nameof(email), ErrorsMessages.InvalidEmail));
                }
            });
        });

        When(c => !string.IsNullOrEmpty(c.PhoneNumber), () =>
        {
            RuleFor(c => c.PhoneNumber).Custom((telephone, context) =>
            {
                var telephonePattern = @"^\d{5}-\d{4}$";
                var isMatch = Regex.IsMatch(telephone, telephonePattern);

                if (!isMatch)
                {
                    context.AddFailure(new FluentValidation.Results
                        .ValidationFailure(nameof(telephone), ErrorsMessages.InvalidPhoneNumber));
                }
            });
        });
    }
}
