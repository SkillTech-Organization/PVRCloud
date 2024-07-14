﻿using FluentValidation;
using PVRPCloud;
using PVRPCloud.Requests;
using static PVRPCloudApi.Validators.ValidationHelpers;

namespace PVRPCloudApi.Validators;

public sealed class TruckValidator : AbstractValidator<PVRPCloud.Requests.PVRPCloudTruck>
{
    public TruckValidator(PVRPCloudProject project)
    {
        var truckIds = IdsToArray(project.Trucks);
        RuleFor(x => x.ID)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .Must(IsUnique(truckIds)).WithMessage(PVRPCloudMessages.ERR_ID_UNIQUE)
            .WithState(GetIdentifiableId);

        var truckTypeIds = IdsToArray(project.TruckTypes);
        RuleFor(x => x.TruckTypeID)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .Must(Contains(truckTypeIds)).WithMessage(PVRPCloudMessages.ERR_NOT_FOUND)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.TruckName)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .NotEmpty()
            .WithState(GetIdentifiableId);

        RuleFor(x => x.ArrDepotMaxTime)
            .GreaterThan(0)
            .GreaterThanOrEqualTo(project.MinTime)
            .LessThanOrEqualTo(project.MaxTime)//.WithMessage(PVRPCloudMessages.ERR_DATEINTERVAL)
            .WithState(GetIdentifiableId);

        var capacityProfileIds = IdsToArray(project.CapacityProfiles);
        RuleFor(x => x.CapacityProfileID)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .Must(Contains(capacityProfileIds)).WithMessage(PVRPCloudMessages.ERR_NOT_FOUND)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.MaxWorkTime)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .GreaterThan(0)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.EarliestStart)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .GreaterThanOrEqualTo(0)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.LatestStart)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .GreaterThan(x => x.EarliestStart)
            .LessThan(project.MaxTime).WithMessage(PVRPCloudMessages.ERR_LESS_THAN)
            .WithState(GetIdentifiableId);
    }
}
