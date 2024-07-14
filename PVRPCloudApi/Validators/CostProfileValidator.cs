using FluentValidation;
using PVRPCloud;
using PVRPCloud.Requests;

namespace PVRPCloudApi.Validators;

using static ValidationHelpers;

public sealed class CostProfileValidator : AbstractValidator<PVRPCloudCostProfile>
{
    public CostProfileValidator(PVRPCloudProject project)
    {
        var ids = IdsToArray(project.CostProfiles);
        RuleFor(x => x.ID)
            .NotEmpty()
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .Must(IsUnique(ids)).WithMessage(PVRPCloudMessages.ERR_ID_UNIQUE)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.FixCost)
            .GreaterThanOrEqualTo(0)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.HourCost)
            .GreaterThanOrEqualTo(0)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.KmCost)
            .GreaterThanOrEqualTo(0)
            .WithState(GetIdentifiableId);
    }
}
