using FluentValidation;
using FluentValidation.Results;

namespace PVRPCloudApi.Validators;

public abstract class ValidatorBase<T> : AbstractValidator<T> where T : class, new()
{
    public override ValidationResult Validate(ValidationContext<T> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        ValidationResult result = base.Validate(context);

        if (!result.IsValid)
        {
            throw new ValidationException("", result.Errors);
        }

        return result;
    }

    public override async Task<ValidationResult> ValidateAsync(ValidationContext<T> context, CancellationToken cancellation = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        ValidationResult result = await base.ValidateAsync(context, cancellation);

        if (!result.IsValid)
        {
            throw new ValidationException("", result.Errors);
        }

        return result;
    }
}
