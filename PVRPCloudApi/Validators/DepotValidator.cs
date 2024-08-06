using FluentValidation;
using PVRPCloud;
using PVRPCloud.Requests;

namespace PVRPCloudApi.Validators;

using static ValidationHelpers;

public sealed class DepotValidator : AbstractValidator<Depot>
{
    public DepotValidator(Project project)
    {
        RuleFor(x => x.ID)
            .NotEmpty()
            .NotNull()
            .WithState(GetIdentifiableId);

        RuleFor(x => x.DepotName)
            .NotNull()
            .NotEmpty()
            .WithState(GetIdentifiableId);

        RuleFor(x => x.Lat)
            .NotNull()
            .InclusiveBetween(-90, 90)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.Lng)
            .NotNull()
            .InclusiveBetween(-180, 190)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.ServiceFixTime)
            .GreaterThanOrEqualTo(0)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.ServiceVarTime)
            .GreaterThanOrEqualTo(0)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.DepotMinTime)
            .GreaterThanOrEqualTo(project.MinTime)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.DepotMaxTime)
            .LessThanOrEqualTo(project.MaxTime)
            .WithState(GetIdentifiableId);
    }
}
