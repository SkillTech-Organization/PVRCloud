using FluentValidation;
using PVRPCloud;
using PVRPCloud.Requests;

namespace PVRPCloudApi.Validators;

using static ValidationHelpers;

public sealed class DepotValidator : AbstractValidator<PVRPCloudDepot>
{
    public DepotValidator(PVRPCloudProject project)
    {
        RuleFor(x => x.ID)
            .NotEmpty()
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.DepotName)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .NotEmpty()
            .WithState(GetIdentifiableId);

        RuleFor(x => x.Lat)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .InclusiveBetween(-90, 90)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.Lng)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
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
            .LessThanOrEqualTo(project.MaxTime).WithMessage(PVRPCloudMessages.ERR_DATEINTERVAL)
            .WithState(GetIdentifiableId);
    }
}
