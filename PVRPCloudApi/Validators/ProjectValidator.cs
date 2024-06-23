using FluentValidation;
using PVRPCloud.Requests;

namespace PVRPCloudApi.Validators;

public sealed class ProjectValidator : AbstractValidator<PVRPCloudProject>
{
    public ProjectValidator()
    {
        RuleFor(x => x.ProjectName)
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.MinTime).NotNull();

        RuleFor(x => x.MaxTime)
            .NotNull()
            .GreaterThan(x => x.MinTime);

        RuleFor(x => x.DistanceLimit).GreaterThanOrEqualTo(0);

        RuleFor(x => x.CostProfiles).NotEmpty();

        RuleFor(x => x.Trucks).NotEmpty();

        RuleFor(x => x.Depots).NotEmpty();

        RuleFor(x => x.Clients).NotEmpty();

        RuleFor(x => x.Orders).NotEmpty();
    }
}
