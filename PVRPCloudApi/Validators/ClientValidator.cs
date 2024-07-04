using FluentValidation;
using PVRPCloud;
using PVRPCloud.Requests;

namespace PVRPCloudApi.Validators;

public sealed class ClientValidator : ValidatorBase<PVRPCloudClient>
{
    public ClientValidator()
    {
        RuleFor(x => x.ID)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY);

        RuleFor(x => x.ClientName)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .NotEmpty().WithMessage(PVRPCloudMessages.ERR_EMPTY);

        RuleFor(x => x.Lat)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .InclusiveBetween(-90, 90).WithMessage(PVRPCloudMessages.ERR_RANGE);

        RuleFor(x => x.Lng)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .InclusiveBetween(-180, 190).WithMessage(PVRPCloudMessages.ERR_RANGE);

        RuleFor(x => x.FixService)
            .GreaterThanOrEqualTo(0).WithMessage(PVRPCloudMessages.ERR_DATEINTERVAL);
    }
}
