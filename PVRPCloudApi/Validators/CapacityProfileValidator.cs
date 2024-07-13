using FluentValidation;
using PVRPCloud;
using PVRPCloud.Requests;

namespace PVRPCloudApi.Validators;

using static ValidationHelpers;

public sealed class CapacityProfileValidator : AbstractValidator<PVRPCloudCapacityProfile>
{
    public CapacityProfileValidator(PVRPCloudProject project)
    {
        var ids = IdsToArray(project.CapacityProfiles);
        RuleFor(x => x.ID)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .Must(IsUnique(ids)).WithMessage(PVRPCloudMessages.ERR_ID_UNIQUE)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.Capacity1)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .WithState(GetIdentifiableId);


        RuleFor(x => x.Capacity2)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .WithState(GetIdentifiableId);
    }
}
