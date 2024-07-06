using FluentValidation;
using PVRPCloud;
using PVRPCloud.Requests;

namespace PVRPCloudApi.Validators;

using static ValidationHelpers;

public sealed class DepotValidator : ValidatorBase<PVRPCloudDepot>
{
    public DepotValidator(PVRPCloudProject project)
    {
        RuleFor(x => x.ID)
            .NotEmpty().WithMessage(PVRPCloudMessages.ERR_EMPTY)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.DepotName)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .NotEmpty().WithMessage(PVRPCloudMessages.ERR_EMPTY)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.Lat)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .InclusiveBetween(-90, 90).WithMessage(PVRPCloudMessages.ERR_RANGE)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.Lng)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .InclusiveBetween(-180, 190).WithMessage(PVRPCloudMessages.ERR_RANGE)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.ServiceFixTime)
            .GreaterThanOrEqualTo(0).WithMessage(PVRPCloudMessages.ERR_DATEINTERVAL)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.ServiceVarTime)
            .GreaterThanOrEqualTo(0).WithMessage(PVRPCloudMessages.ERR_DATEINTERVAL)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.DepotMinTime)
            .GreaterThanOrEqualTo(project.MinTime).WithMessage(PVRPCloudMessages.ERR_DATEINTERVAL)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.DepotMaxTime)
            .LessThanOrEqualTo(project.MinTime).WithMessage(PVRPCloudMessages.ERR_DATEINTERVAL)
            .WithState(GetIdentifiableId);
    }
}
