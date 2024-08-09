﻿using FluentValidation;
using PVRPCloud;
using PVRPCloud.Requests;

namespace PVRPCloudApi.Validators;

using static ValidationHelpers;

public sealed class ClientValidator : AbstractValidator<Client>
{
    public ClientValidator(Project project)
    {
        var ids = IdsToArray(project.Clients);
        RuleFor(x => x.ID)
            .NotEmpty()
            .NotNull()
            .Must(IsUnique(ids)).WithMessage(Messages.ERR_ID_UNIQUE)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.ClientName)
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
    }
}
