using FluentValidation;
using PVRPCloud.Requests;

namespace PVRPCloudApi.Validators;

public sealed class ClientValidator : AbstractValidator<PVRPCloudClient>
{
    public ClientValidator()
    {
        RuleFor(x => x.ID).NotEqual(0);

        RuleFor(x => x.ClientName)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Lat).InclusiveBetween(-90, 90);

        RuleFor(x => x.Lng).InclusiveBetween(-180, 190);

        RuleFor(x => x.FixService).GreaterThanOrEqualTo(0);
    }
}
