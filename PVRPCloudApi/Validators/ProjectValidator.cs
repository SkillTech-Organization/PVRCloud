using FluentValidation;
using PVRPCloud;
using PVRPCloud.Requests;

namespace PVRPCloudApi.Validators;

public sealed class ProjectValidator : AbstractValidator<PVRPCloudProject>
{
    public ProjectValidator()
    {
        RuleFor(x => x.ProjectName)
            .NotEmpty().WithMessage(PVRPCloudMessages.ERR_EMPTY)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY);

        RuleFor(x => x.MinTime).NotNull();

        RuleFor(x => x.MaxTime)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_EMPTY)
            .GreaterThan(x => x.MinTime).WithMessage(PVRPCloudMessages.ERR_DATEINTERVAL);

        RuleFor(x => x.MaxTourDuration)
            .GreaterThan(0).WithMessage(PVRPCloudMessages.ERR_ZERO);

        RuleFor(x => x.DistanceLimit)
            .GreaterThanOrEqualTo(0).WithMessage(PVRPCloudMessages.ERR_NEGATIVE);

        RuleFor(x => x.CostProfiles)
            .NotEmpty().WithMessage(PVRPCloudMessages.ERR_EMPTY);

        RuleFor(x => x.Trucks)
            .NotEmpty().WithMessage(PVRPCloudMessages.ERR_EMPTY);

        RuleFor(x => x.Depots)
            .NotEmpty().WithMessage(PVRPCloudMessages.ERR_EMPTY);

        RuleFor(x => x.Clients)
            .NotEmpty().WithMessage(PVRPCloudMessages.ERR_EMPTY);

        RuleFor(x => x.Orders)
            .NotEmpty().WithMessage(PVRPCloudMessages.ERR_EMPTY);
    }
}
