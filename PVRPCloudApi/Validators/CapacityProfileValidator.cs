using FluentValidation;
using PVRPCloud.Requests;

namespace PVRPCloudApi.Validators;

public sealed class CapacityProfileValidator : AbstractValidator<PVRPCloudCapacityProfile>
{
    public CapacityProfileValidator()
    {
        RuleFor(x => x.ID).NotEqual(0);

        RuleFor(x => x.Capacity1).NotEqual(0);

        RuleFor(x => x.Capacity2).NotEqual(0);
    }
}
