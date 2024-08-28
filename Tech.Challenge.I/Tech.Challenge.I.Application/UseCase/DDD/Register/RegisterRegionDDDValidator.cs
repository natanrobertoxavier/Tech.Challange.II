using FluentValidation;
using Tech.Challenge.I.Communication.Request;
using Tech.Challenge.I.Exceptions;

namespace Tech.Challenge.I.Application.UseCase.DDD.Register;
public class RegisterRegionDDDValidator : AbstractValidator<RequestRegionDDDJson>
{
    public RegisterRegionDDDValidator()
    {
        RuleFor(p => p.Region)
            .NotNull()
            .WithMessage(ErrorsMessages.RegionNotEmpty);

        RuleFor(p => p.Region)
            .IsInEnum()
            .WithMessage(ErrorsMessages.InvalidRegion);
        RuleFor(p => p.DDD)
            .NotEmpty()
            .WithMessage(ErrorsMessages.DDDNotFound);

        RuleFor(p => p.DDD)
            .InclusiveBetween(10, 99)
            .WithMessage(ErrorsMessages.DDDBetweenTenNinetyNine);
    }
}
