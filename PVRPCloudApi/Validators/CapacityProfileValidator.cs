using FluentValidation;
using PVRPCloud;
using PVRPCloud.Requests;

namespace PVRPCloudApi.Validators;

public sealed class CapacityProfileValidator : AbstractValidator<PVRPCloudCapacityProfile>
{
    public CapacityProfileValidator()
    {
        RuleFor(x => x.ID)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY);

        RuleFor(x => x.Capacity1)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY);


        RuleFor(x => x.Capacity2)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY);
    }
}
