using FluentValidation;
using PVRPCloud;
using PVRPCloud.Requests;

namespace PVRPCloudApi.Validators;

public sealed class CostProfileValidator : AbstractValidator<PVRPCloudCostProfile>
{
    public CostProfileValidator()
    {
        RuleFor(x => x.ID)
            .NotEmpty().WithMessage(PVRPCloudMessages.ERR_EMPTY)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY);

        RuleFor(x => x.FixCost)
            .GreaterThanOrEqualTo(0).WithMessage(PVRPCloudMessages.ERR_NEGATIVE);

        RuleFor(x => x.HourCost)
            .GreaterThanOrEqualTo(0).WithMessage(PVRPCloudMessages.ERR_NEGATIVE);

        RuleFor(x => x.KmCost)
            .GreaterThanOrEqualTo(0).WithMessage(PVRPCloudMessages.ERR_NEGATIVE);
    }
}
