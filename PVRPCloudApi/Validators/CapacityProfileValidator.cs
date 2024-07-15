﻿using FluentValidation;
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
            .NotEmpty()
            .NotNull()
            .Must(IsUnique(ids)).WithMessage(PVRPCloudMessages.ERR_ID_UNIQUE)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.Capacity1)
            .NotNull()
            .WithState(GetIdentifiableId);

        RuleFor(x => x.Capacity2)
            .NotNull()
            .WithState(GetIdentifiableId);
    }
}
