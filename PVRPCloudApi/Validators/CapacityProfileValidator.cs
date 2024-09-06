using FluentValidation;
using PVRPCloud;
using PVRPCloud.Models;

namespace PVRPCloudApi.Validators;

using static ValidationHelpers;

public sealed class CapacityProfileValidator : AbstractValidator<CapacityProfile>
{
    public CapacityProfileValidator(Project project)
    {
        var ids = IdsToArray(project.CapacityProfiles);
        RuleFor(x => x.ID)
            .NotEmpty()
            .NotNull()
            .Must(IsUnique(ids)).WithMessage(Messages.ERR_ID_UNIQUE)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.Capacity1)
            .NotNull()
            .WithState(GetIdentifiableId);

        RuleFor(x => x.Capacity2)
            .NotNull()
            .WithState(GetIdentifiableId);
    }
}
