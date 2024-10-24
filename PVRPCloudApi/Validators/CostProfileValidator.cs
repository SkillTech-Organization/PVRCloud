﻿using FluentValidation;
using PVRPCloud;
using PVRPCloud.Models;

namespace PVRPCloudApi.Validators;

using static ValidationHelpers;

public sealed class CostProfileValidator : AbstractValidator<CostProfile>
{
    public CostProfileValidator(Project project)
    {
        var ids = IdsToArray(project.CostProfiles);
        RuleFor(x => x.ID)
            .NotEmpty()
            .NotNull()
            .Must(IsUnique(ids)).WithMessage(Messages.ERR_ID_UNIQUE)
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
